using Cympatic.Stub.Connectivity.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;

namespace Cympatic.Stub.Connectivity.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddStubConnectivity(this IServiceCollection services)
    {
        services
            .AddOptions<StubConnectivitySettings>()
            .Configure<IConfiguration>((options, configuration) => configuration.GetSection(nameof(StubConnectivitySettings)).Bind(options)); 

        services.AddHttpClient<SetupClientApiService>((serviceProvider, config) =>
        {
            var connectivitySettings = serviceProvider.GetRequiredService<IOptions<StubConnectivitySettings>>().Value;

            if (string.IsNullOrWhiteSpace(connectivitySettings.BaseAddress))
            {
                throw new ArgumentException($"{nameof(connectivitySettings.BaseAddress)} must be provided");
            }

            config.BaseAddress = new Uri(connectivitySettings.BaseAddress);
            config.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });
        services.AddHttpClient<SetupResponseApiService>((serviceProvider, config) =>
        {
            var connectivitySettings = serviceProvider.GetRequiredService<IOptions<StubConnectivitySettings>>().Value;

            if (string.IsNullOrWhiteSpace(connectivitySettings.BaseAddress))
            {
                throw new ArgumentException($"{nameof(connectivitySettings.BaseAddress)} must be provided");
            }

            config.BaseAddress = new Uri(connectivitySettings.BaseAddress);
            config.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });
        services.AddHttpClient<VerifyRequestApiService>((serviceProvider, config) =>
        {
            var connectivitySettings = serviceProvider.GetRequiredService<IOptions<StubConnectivitySettings>>().Value;

            if (string.IsNullOrWhiteSpace(connectivitySettings.BaseAddress))
            {
                throw new ArgumentException($"{nameof(connectivitySettings.BaseAddress)} must be provided");
            }

            config.BaseAddress = new Uri(connectivitySettings.BaseAddress);
            config.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}
