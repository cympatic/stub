using Cympatic.Extensions.Stub.SpecFlow.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Cympatic.Extensions.Stub.SpecFlow
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddSpecFlowStub(this IServiceCollection services)
        {
            services.AddScoped<ItemContext>();
            services.AddScoped<StubContext>();

            return services;
        }
    }
}
