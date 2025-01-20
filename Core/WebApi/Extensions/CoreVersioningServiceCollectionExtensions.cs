using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Donatas.Core.WebApi.Extensions
{
    public static class CoreVersioningServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(_ =>
            {
                _.DefaultApiVersion = new ApiVersion(1, 0);
                _.AssumeDefaultVersionWhenUnspecified = true;
                _.ReportApiVersions = true;
                _.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("x-api-version"),
                    new MediaTypeApiVersionReader("x-api-version"),
                    new QueryStringApiVersionReader("x-api-version")
                );
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}
