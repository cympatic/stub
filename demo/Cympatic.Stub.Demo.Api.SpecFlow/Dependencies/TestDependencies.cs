using Cympatic.Extensions.Configuration;
using Cympatic.Stub.Connectivity.Extensions;
using Cympatic.Stub.Demo.Api.SpecFlow.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddHttpClient<DemoApiService>(cfg =>
            {
                cfg.BaseAddress = new Uri("http://localhost:32200");
                cfg.DefaultRequestHeaders
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
