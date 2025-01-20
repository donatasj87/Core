using Donatas.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Donatas.Core.Logger
{
    public class LogEntry
    {
        public LogEntry() { }

        public LogEntry(Level level, string message, string messageDetails = "")
        {
            Level = level;
            Message = message;
            MessageDetails = messageDetails;
        }

        /// <summary>
        /// ApplicationName can also be set up by calling Configure()
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Optional field, is set automatically
        /// </summary>
        public DateTime TimeGeneratedReal { get; set; } = DateTime.Now;

        [JsonConverter(typeof(StringEnumConverter))]
        public Level Level { get; set; } = Level.Informational;

        public string Message { get; set; }

        public string MessageDetails { get; set; } = string.Empty;
    }
}
