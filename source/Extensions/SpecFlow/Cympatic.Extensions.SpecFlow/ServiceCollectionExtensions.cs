using Microsoft.Extensions.DependencyInjection;

namespace Cympatic.Extensions.SpecFlow
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSpecFlowStub(this IServiceCollection services)
        {
            services.AddScoped<StubServerHelper>();

            return services;
        }
    }
}
