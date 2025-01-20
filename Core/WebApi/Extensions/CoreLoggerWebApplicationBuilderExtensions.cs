using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Donatas.Core.Logger;

namespace Donatas.Core.WebApi.Extensions
{
    public static class CoreLoggerWebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddCoreLogger(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.Services.AddSingleton<ILoggerService, LoggerService>();

            return builder;
        }
    }
}
