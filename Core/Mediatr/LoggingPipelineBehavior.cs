using System.Diagnostics;
using MediatR;
using Newtonsoft.Json;
using Donatas.Core.Logger;

namespace Donatas.Core.Mediatr
{
    public sealed class LoggingPipelineBehavior<TRequest, TResponse>(ILoggerService logger) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(next);
            ArgumentNullException.ThrowIfNull(request);

            var requestName = $"[{request.GetType().Name}]. [{Guid.NewGuid()}]";
            var stopwatch = new Stopwatch();
            TResponse? response = default;

            // if the JsonConvert.SerializeObject raises an exception, we log the request without the request object
            // so the next() call is always executed
            // TODO: remove the try-catch block and log all serialized parameters in request
            var isCoreRequest = request as CoreRequest<TResponse>;
            var shouldLog = isCoreRequest?.CreateLog ?? true;
            var message = isCoreRequest?.Message ?? string.Empty;

            if (shouldLog)
            {
                if (!string.IsNullOrEmpty(message))
                    logger.Log(new LogEntry { Message = $"[START] {requestName}", MessageDetails = message });
                else
                {
                    try
                    {
                        logger.Log(new LogEntry { Message = $"[START] {requestName}", MessageDetails = JsonConvert.SerializeObject(request) });
                    }
                    catch
                    {
                        logger.Log(new LogEntry { Message = $"[START] {requestName}", MessageDetails = $"{request.GetType().Name}{{ {string.Join(", ", request.GetType().GetProperties().Select(x => x.Name))} }}" });
                    }
                }
            }

            try
            {
                stopwatch.Start();
                response = await next();
            }
            finally
            {
                stopwatch.Stop();

                if (shouldLog)
                    logger.Log(new LogEntry { Message = $"[END] {requestName}. {stopwatch.ElapsedMilliseconds} ms" });
            }

            return response;
        }
    }
}
