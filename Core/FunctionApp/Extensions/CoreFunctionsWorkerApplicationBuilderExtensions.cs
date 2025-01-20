using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Hosting;
using Donatas.Core.FunctionApp.Middleware;

namespace Donatas.Core.FunctionApp.Extensions
{
    public static class CoreFunctionsWorkerApplicationBuilderExtensions
    {
        public static IFunctionsWorkerApplicationBuilder UseCoreFunctionApplication(this IFunctionsWorkerApplicationBuilder builder)
        {
            builder.UseNewtonsoftJson();
            builder.UseMiddleware<CoreFunctionAppExceptionMiddleware>();
            return builder;
        }
    }
}
