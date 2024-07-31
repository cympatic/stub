using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

namespace Cympatic.Extensions.Stub.Internal;

internal static class HostBuilderExtensions
{
    public static IHostBuilder AddApiService<TApiService>(this IHostBuilder builder)
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
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    UseProxy = false,
                };
            });
        });

        return builder;
    }
}
