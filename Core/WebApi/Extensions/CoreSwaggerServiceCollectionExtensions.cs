using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Donatas.Core.Configuration;
using Donatas.Core.WebApi.Swagger;
using System.Configuration;

namespace Donatas.Core.WebApi.Extensions
{
    public static class CoreSwaggerServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var coreApplicationOptions = configuration.GetSection("CoreApplication").Get<CoreApplicationOptions>() ??
                throw new ConfigurationErrorsException("CoreApplication section is missing from application configuration");

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            services.AddSwaggerGen(options =>
            {
                options.DocumentFilter<SwaggerFilter>();
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                options.IncludeXmlComments($"{Path.GetDirectoryName(assembly.Location)}\\{assembly.GetName().Name}.xml");

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    var applicationDescription = $"<b>{coreApplicationOptions.Environment}</b>";
                    if (coreApplicationOptions.Environment == CoreEnvironment.Local)
                    {
                        applicationDescription += "<br/><a target=\"_blank\" href=\"/api/SignIn\">Sign in</a></br><a target=\"_blank\" href=\"/api/SignOut\">Sign out</a>";
                    }
                    if (description.IsDeprecated)
                    {
                        applicationDescription += "<br/>This API version has been <b>deprecated</b>. Please use the newer versions";
                    }

                    System.Reflection.Assembly.GetEntryAssembly()?.GetReferencedAssemblies()
                        .Select(assembly => Path.Combine(AppContext.BaseDirectory, $"{assembly.Name}.xml"))
                        .Where(path => File.Exists(path))
                        .ToList()
                        .ForEach(x => options.IncludeXmlComments(x));

                    options.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = coreApplicationOptions.Name,
                        Version = description.ApiVersion.ToString(),
                        Description = applicationDescription
                    });
                }
            });

            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }
    }
}
