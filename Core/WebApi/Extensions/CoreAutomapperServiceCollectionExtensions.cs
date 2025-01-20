using Microsoft.Extensions.DependencyInjection;

namespace Donatas.Core.WebApi.Extensions
{
    public static class CoreAutomapperServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreAutomapper(this IServiceCollection services)
        {
            // automapper profiles
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}
