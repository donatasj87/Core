using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Donatas.Core.Configuration;
using Donatas.Core.Extensions;
using STG.Connectors.StorageAccount.Queues;
using System.Configuration;

namespace Donatas.Core.Logger
{
    public class LoggerService : ILoggerService
    {
        private readonly IQueueConnector _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CoreApplicationOptions _coreApp;
        private const int _32Kb = 32768;
        private readonly IQueueConnector _queue;

        public LoggerService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _logger = new QueueConnector();
            _httpContextAccessor = httpContextAccessor;
            _coreApp = configuration.GetSection("CoreApplication").Get<CoreApplicationOptions>() ?? throw new ConfigurationErrorsException("No configuration found for CoreApplication");

            _queue = new QueueConnector();
            _queue.Configure(new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 }, "sentinel-log", CoreStorageAccount.GetStorageAccount(_coreApp.Environment));
        }

        public Response<SendReceipt> Log(LogEntry log)
        {
            log.ApplicationName = _coreApp.Name ?? throw new ArgumentException($"Please provide Logger parameter for ApplicationName");
            var result = Task.Run(async () => await _queue.SendMessageAsync(log).ConfigureAwait(false)).Result;
            return result;
        }

        public void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            Log(new LogEntry(Level.Verbose, message));
        }

        public void Log(Exception ex) =>
            Log(new LogEntry { Level = Level.Critical, Message = ex.ToDetailedMessage(), MessageDetails = ex.ToDetailedException() });

        public void LogMethodExecution(object? data = null)
        {
            var user = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(_ => _.Type == "name")?.Value ?? _httpContextAccessor.HttpContext.User.Identity.Name;
            var stack = new System.Diagnostics.StackTrace().GetFrame(2).GetMethod();
            var infoLogEntry = new LogEntry(Level.Verbose, $"{stack.DeclaringType.Name}.{stack.Name}() was executed by user {user}");
            if (data != null)
                infoLogEntry.MessageDetails = JsonConvert.SerializeObject(data);
            Log(infoLogEntry);
        }

        public void LogFunctionError<T>(Func<T> func, Exception ex)
        {
            if ((ex.ToDetailedMessage() + ex.ToDetailedException()).Length > _32Kb)
                ex = new Exception((ex.ToDetailedMessage() + ex.ToDetailedException()).Substring(0, _32Kb));

            var log = new LogEntry
            {
                Level = Level.Error,
                Message = $"Error ocurred while calling function {GetType().Name}.{ex.TargetSite.Name}" +
                    $"({JsonConvert.SerializeObject(func.Target, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })})",
                MessageDetails = $":{Environment.NewLine}{JsonConvert.SerializeObject(ex)}"
            };

            Log(log);
        }
    }
}
