using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Donatas.Core.RestClient;
using Donatas.Core.WebApi.Extensions;
using Donatas.Core.WebApi.Responses;
using Donatas.Core.Logger;

namespace Donatas.Core.FunctionApp.Extensions
{
    public static class CoreFunctionAppServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServiceCollections(this IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetService<IConfiguration>();
            serviceCollection.AddSingleton<ILoggerService, LoggerService>();
            serviceCollection.AddTransient<IRestClient, RestClient.RestClient>();
            serviceCollection.AddCoreMediatr();
            serviceCollection.AddCoreValidation();
            serviceCollection.AddCoreAutomapper();
            serviceCollection.AddHttpContextAccessor();
            serviceCollection.AddSingleton<ICoreResponseFactory, CoreResponseFactory>();
            serviceCollection.AddSingleton<IOpenApiConfigurationOptions>(_ =>
            {
                var options = new OpenApiConfigurationOptions()
                {
                    Info = new()
                    {
                        Version = configuration.GetValue<string>("CoreApplication:Version") ?? "1",
                        Title = configuration.GetValue<string>("CoreApplication:Name"),
                        Description = configuration.GetValue<string>("CoreApplication:Environment"),
                    }
                };

                return options;
            });
            return serviceCollection;
        }
    }
}
