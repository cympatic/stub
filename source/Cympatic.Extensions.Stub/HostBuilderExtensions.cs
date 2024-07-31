using Cympatic.Extensions.Stub.Internal;
using Cympatic.Extensions.Stub.Internal.Collections;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Cympatic.Extensions.Stub;

public static class HostBuilderExtensions
{
    private const string LocalHost = "localhost";

    public static IHostBuilder AddStubServer(this IHostBuilder builder)
    {
        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.ConfigureServices((services) =>
            {
                services.AddSingleton<ReceivedRequestCollection>();
                services.AddSingleton<ResponseSetupCollection>();
            });
        });

        return builder;
    }

    public static IHostBuilder UseStubServer(this IHostBuilder builder)
    {
        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 0, endpoint =>
                {
                    var certificate = GetCertificate();
                    endpoint.UseHttps(certificate);
                });
            });

            webHostBuilder.Configure(app =>
            {
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapSetupResponse();
                    endpoints.MapReceivedRequest();
                });

                app.UseStub();
            });
        });

        return builder;
    }

    private static X509Certificate2 GetCertificate()
    {
        using var store = new X509Store(StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        var certificate = store.Certificates
            .Find(X509FindType.FindByIssuerName, LocalHost, false)
            .Where(cert => cert.NotBefore <= DateTime.Now.Date && cert.NotAfter >= DateTime.Now.Date)
            .OrderByDescending(cert => cert.NotAfter)
            .FirstOrDefault();
        store.Close();

        if (certificate is null)
        {
            throw new InvalidOperationException("IIS Express Development Certificate cannot be found!");
        }

        return certificate;
    }
}
