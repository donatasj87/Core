using FluentValidation;
using System.Text;

namespace Donatas.Core.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Generates a full stack trace
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string ToDetailedException(this Exception exception)
        {
            var innerException = exception;
            var sb = new StringBuilder();

            do
            {
                sb.AppendLine(innerException.ToString());
                sb.AppendLine("-------------------------");
                innerException = innerException.InnerException;
            }
            while (innerException != null);
            return sb.ToString();
        }

        /// <summary>
        /// Generates all messages from the stack trace
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string ToDetailedMessage(this Exception exception)
        {
            var innerException = exception;
            var sb = new StringBuilder();

            do
            {
                sb.Append($"{innerException.Message} ");
                innerException = innerException.InnerException;
            }
            while (innerException != null);
            return sb.ToString();
        }

        public static IEnumerable<string> GetErrorMessages(this ValidationException ex) =>
            ex.Errors.Select(_ => $"{_.PropertyName}: {_.ErrorMessage}");
    }
}
