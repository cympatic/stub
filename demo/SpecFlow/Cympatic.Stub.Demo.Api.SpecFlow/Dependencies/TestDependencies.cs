using Cympatic.Extensions.Configuration;
using Cympatic.Stub.Connectivity.Extensions;
using Cympatic.Stub.Demo.Api.SpecFlow.Services;
using Cympatic.Stub.Demo.Api.SpecFlow.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SolidToken.SpecFlow.DependencyInjection;
using System;
using System.Net.Http.Headers;

namespace Cympatic.Stub.Demo.Api.SpecFlow.Dependencies
{
    public static class TestDependencies
    {
        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();

            // Add test dependencies
            var configuration = Configure();
            services.AddSingleton(configuration);

            services.AddStubConnectivity();

            services
               .AddOptions<DemoApiSettings>()
               .Configure<IConfiguration>((options, configuration) => configuration.GetSection(nameof(DemoApiSettings)).Bind(options));

            services.AddHttpClient<DemoApiService>((serviceProvider, config) =>
            {
                var demoSettings = serviceProvider.GetRequiredService<IOptions<DemoApiSettings>>().Value;

                if (string.IsNullOrWhiteSpace(demoSettings.Url))
                {
                    throw new ArgumentException($"{nameof(demoSettings.Url)} must be provided");
                }

                config.BaseAddress = new Uri(demoSettings.Url);
                config.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            return services;
        }

        private static IConfiguration Configure()
        {
            return new ConfigurationBuilder()
                .AddAvailableConfigurations()
                .Build();
        }
    }
}
