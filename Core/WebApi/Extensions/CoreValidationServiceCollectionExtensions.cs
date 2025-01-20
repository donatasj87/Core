using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Donatas.Core.WebApi.Extensions
{
    public static class CoreValidationServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreValidation(this IServiceCollection services)
        {
            // validators
            services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}
