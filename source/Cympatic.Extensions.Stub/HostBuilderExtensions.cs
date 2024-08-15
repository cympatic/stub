using Cympatic.Extensions.Stub.Internal;
using Cympatic.Extensions.Stub.Internal.Collections;
using Cympatic.Extensions.Stub.Internal.Interfaces;
using Cympatic.Extensions.Stub.Internal.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

namespace Cympatic.Extensions.Stub;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddStubServer(this IHostBuilder builder)
    {
        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.ConfigureServices((services) =>
            {
                services.AddRouting();

                services.AddSingleton<IReceivedRequestCollection, ReceivedRequestCollection>();
                services.AddSingleton<IResponseSetupCollection, ResponseSetupCollection>();
            });
        });

        return builder;
    }

    public static IHostBuilder UseLocalhost(this IHostBuilder builder, bool useSsl = false)
    {
        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 0, endpoint =>
                {
                    if (useSsl)
                    {
                        var certificate = GetCertificate();
                        endpoint.UseHttps(certificate);
                    }
                });
            });
        });

        return builder;
    }

    public static IHostBuilder UseStubServer(this IHostBuilder builder)
    {
        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.Configure(app =>
            {
                app.UseRouting();

                app.UseEndpoints(endpointBuilder =>
                {
                    endpointBuilder.MapSetupResponse();
                    endpointBuilder.MapReceivedRequest();
                });

                app.UseMiddleware<StubMiddleware>();
            });
        });

        return builder;
    }

    public static IHostBuilder AddApiService<TApiService>(this IHostBuilder builder, bool useSsl = false)
        where TApiService : class
    {
        HttpClientHandler configureHandler()
        {
            return useSsl
                ? new HttpClientHandler
                {
                    UseProxy = false,
                    UseDefaultCredentials = true,
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                }
                : new HttpClientHandler();
        }

        builder.AddApiService<TApiService>(configureHandler);

        return builder;
    }


    public static IHostBuilder AddApiService<TApiService>(this IHostBuilder builder, Func<HttpClientHandler>? configureHandler)
        where TApiService : class
    {
        builder.ConfigureServices(services =>
        {
            services.AddHttpClient<TApiService>((serviceProvider, config) =>
            {
                var baseAddress = serviceProvider.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First();

                config.BaseAddress = new Uri(baseAddress);
                config.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .ConfigurePrimaryHttpMessageHandler(() => configureHandler?.Invoke() ?? new HttpClientHandler());
        });

        return builder;
    }

    private static X509Certificate2 GetCertificate()
    {
        const string localHost = "localhost";

        using var store = new X509Store(StoreName.My, OperatingSystem.IsWindows() ? StoreLocation.LocalMachine : StoreLocation.CurrentUser, OpenFlags.ReadOnly);
        store.Open(OpenFlags.ReadOnly);
        var certificate = store.Certificates
            .Find(X509FindType.FindByIssuerName, localHost, false)
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
