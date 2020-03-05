using Microsoft.Extensions.DependencyInjection;

namespace OswaldTechnologies.Multitenancy
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMultitenancy(this IServiceCollection services)
        {
            services.AddTransient<ITenantService, TenantService>();

            return services;
        }
    }
}