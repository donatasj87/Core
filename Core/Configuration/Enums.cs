namespace Donatas.Core.Configuration
{
    /// <summary>
    /// Describes the log level with different meanings
    /// </summary>
    public enum Level
    {
        // Application got an issue and can not continue
        Critical,
        // Application got an error, but can continue.
        Error,
        // Application got a not important issue, and can continue
        Warning,
        // Used for storing auditing logs
        Informational,
        // Used for storing debugging logs
        Verbose
    }
}
