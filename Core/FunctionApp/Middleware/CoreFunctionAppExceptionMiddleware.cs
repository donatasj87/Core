using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using Donatas.Core.Extensions;
using FluentValidation;
using Donatas.Core.WebApi.Responses;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Donatas.Core.Logger;
using Donatas.Core.Configuration;

namespace Donatas.Core.FunctionApp.Middleware
{
    internal sealed class CoreFunctionAppExceptionMiddleware(ILoggerService logger, ICoreResponseFactory coreResponseFactory, IConfiguration configuration) : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                if (ex is ValidationException validationException)
                {
                    logger.Log(new LogEntry(Level.Warning, validationException.Message, string.Join(Environment.NewLine, validationException.GetErrorMessages())));
                }
                else
                {
                    logger.Log(ex);
                }

                var httpReqData = await context.GetHttpRequestDataAsync();

                if (httpReqData == null)
                    return;

                var response = coreResponseFactory.GetCoreResponse(ex);
                var httpResponse = httpReqData.CreateResponse(response.StatusCode);
                httpResponse.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                await httpResponse.WriteStringAsync(JsonConvert.SerializeObject(response));
                if (bool.TryParse(configuration.GetValue<string>("Debug"), out var debug) && debug)
                    await httpResponse.WriteStringAsync($"{Environment.NewLine} {ex.ToDetailedException()}");
                var invocationResult = context.GetInvocationResult();
                var httpOutputBindingFromMultipleOutputBindings = context.GetOutputBindings<HttpResponseData>().FirstOrDefault(_ => _.BindingType == "http" && _.Name != "$return");

                if (httpOutputBindingFromMultipleOutputBindings is not null)
                    httpOutputBindingFromMultipleOutputBindings.Value = httpResponse;
                else
                    invocationResult.Value = httpResponse;
            }
        }
    }
}
