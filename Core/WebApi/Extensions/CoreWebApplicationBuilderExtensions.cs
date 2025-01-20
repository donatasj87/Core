using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Donatas.Core.RestClient;
using Donatas.Core.WebApi.Middlewares;
using Donatas.Core.WebApi.Responses;

namespace Donatas.Core.WebApi.Extensions
{
    public static class CoreWebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddCoreWebApplication(this WebApplicationBuilder builder)
        {
            // HttpContext accessor service
            builder.Services.AddHttpContextAccessor();

            // CoreResponse factory
            builder.Services.AddTransient(typeof(ICoreResponseFactory), typeof(CoreResponseFactory));

            // mediator
            builder.Services.AddCoreMediatr();

            // Middlewares
            builder.Services.AddTransient<CoreWebApplicationExceptionMiddleware>();

            // authentication
            builder.AddCoreAuthentication();

            // versioning
            builder.Services.AddCoreVersioning();

            // logger
            builder.AddCoreLogger();

            // validation
            builder.Services.AddCoreValidation();

            // IRest client for communication with Azure
            builder.Services.AddTransient<IRestClient, RestClient.RestClient>();

            // automapper
            builder.Services.AddCoreAutomapper();

            // swagger
            builder.Services.AddCoreSwagger(builder.Configuration);

            // controllers
            builder.Services.AddControllers()
                .AddNewtonsoftJson(_ =>
                {
                    _.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    _.SerializerSettings.Formatting = Formatting.Indented;
                    _.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddApplicationPart(Assembly.GetExecutingAssembly())
                .AddControllersAsServices();

            return builder;
        }
    }
}
