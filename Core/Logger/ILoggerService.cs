using Azure.Storage.Queues.Models;
using Azure;

namespace Donatas.Core.Logger
{
    /// <summary>
    /// Logger settings should be present in CoreApplication section fields Name and Environment
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Creates a log entry to be sent to queue for processing and sending to custom table in Log analytics
        /// </summary>
        /// <param name="log">Log entry to be sent to repository</param>
        /// <returns>Azure response which provides the send receipt</returns>
        Response<SendReceipt> Log(LogEntry log);

        /// <summary>
        /// Creates an verbose log entry with a simple string message
        /// </summary>
        /// <param name="message">Simple text message</param>
        void Log(string message);

        /// <summary>
        /// Retrieves a full details of exception and logs it as an error
        /// </summary>
        /// <param name="ex"></param>
        void Log(Exception ex);

        /// <summary>
        /// Creates a verbose log about the method execution details
        /// </summary>
        /// <param name="data">(Anonymous) object which could contain method parameters</param>
        void LogMethodExecution(object? data = null);

        /// <summary>
        /// If executing a function that throws an error, this generates the error log with function name, parameters and exception details
        /// Usage example in DMS DevOpsService.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">Function to be invoked</param>
        /// <param name="ex">Exception that was catched while executing the function</param>
        void LogFunctionError<T>(Func<T> func, Exception ex);
    }
}