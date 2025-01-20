using System.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Donatas.Core.Configuration;

namespace Donatas.Core.WebApi.Extensions
{
    public static class CoreSwaggerWebApplicationExtensions
    {
        public static WebApplication UseCoreSwagger(this WebApplication app)
        {
            var coreApplicationOptions = app.Configuration.GetSection("CoreApplication").Get<CoreApplicationOptions>() ?? throw new ConfigurationErrorsException("CoreApplication section is missing from configuration file");

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                app
                    .Services
                    .GetRequiredService<IApiVersionDescriptionProvider>()
                    .ApiVersionDescriptions
                    .Where(x => x.ApiVersion.MajorVersion >= 1)
                    .Reverse()
                    .ToList()
                    .ForEach(x =>
                    {
                        var description = $"{coreApplicationOptions.Name} - {x.GroupName.ToUpperInvariant()}";
                        if (x.IsDeprecated)
                            description = $"[Deprecated] { description }";
                        options.SwaggerEndpoint($"/swagger/{x.GroupName}/swagger.json", description);
                    });
            });

            app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));

            return app;
        }
    }
}

