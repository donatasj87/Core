using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Donatas.Core.Mediatr;

namespace Donatas.Core.WebApi.Extensions
{
    public static class CoreMediatrServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreMediatr(this IServiceCollection services)
        {
            // mediatr
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

                Assembly.GetEntryAssembly()?.GetReferencedAssemblies()
                    .Where(assembly => File.Exists(Path.Combine(AppContext.BaseDirectory, $"{assembly.Name}.dll")))
                    .ToList()
                    .ForEach(x => cfg.RegisterServicesFromAssemblies(Assembly.Load(x)));
            });

            // behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

            return services;
        }
    }
}
