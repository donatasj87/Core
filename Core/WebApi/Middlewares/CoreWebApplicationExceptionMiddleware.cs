using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Donatas.Core.Extensions;
using Donatas.Core.WebApi.Responses;
using Donatas.Core.Logger;
using Donatas.Core.Configuration;

namespace Donatas.Core.WebApi.Middlewares
{
    internal class CoreWebApplicationExceptionMiddleware(ILoggerService logger, ICoreResponseFactory coreResponseFactory) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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

                await SendCoreResponseAsync(context, ex);
            }
        }

        private async Task SendCoreResponseAsync(HttpContext context, Exception ex)
        {
            var coreResponse = coreResponseFactory.GetCoreResponse(ex);

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)coreResponse.StatusCode;

            await context.Response.WriteAsync(JsonConvert.SerializeObject(coreResponse));
        }
    }
}
