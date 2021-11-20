using Cympatic.Extensions.Configuration;
using Cympatic.Extensions.Stub.SpecFlow;
using Cympatic.Stub.Connectivity.Extensions;
using Cympatic.Stub.Example.Api.SpecFlow.Services;
using Cympatic.Stub.Example.Api.SpecFlow.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SolidToken.SpecFlow.DependencyInjection;
using System;
using System.Net.Http.Headers;

namespace Cympatic.Stub.Example.Api.SpecFlow.Dependencies
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

            services.AddSpecFlowStub();
            services.AddStubConnectivity();

            services
               .AddOptions<ExampleApiServiceSettings>()
               .Configure<IConfiguration>((options, configuration) => configuration.GetSection(nameof(ExampleApiServiceSettings)).Bind(options));

            services.AddHttpClient<ExampleApiService>((serviceProvider, config) =>
            {
                var exampleApiServiceSettings = serviceProvider.GetRequiredService<IOptions<ExampleApiServiceSettings>>().Value;

                if (string.IsNullOrWhiteSpace(exampleApiServiceSettings.Url))
                {
                    throw new ArgumentException($"{nameof(exampleApiServiceSettings.Url)} must be provided");
                }

                config.BaseAddress = new Uri(exampleApiServiceSettings.Url);
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
