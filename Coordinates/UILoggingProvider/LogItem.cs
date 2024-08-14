using Microsoft.Extensions.Logging;

namespace UILoggingProvider;

public class LogItem
{
    public LogItem(DateTime timestamp, LogLevel logLevel, string message, string source)
    {
        Timestamp = timestamp;
        LogLevel = logLevel;
        Message = message;
        Source = source;
    }

    public DateTime Timestamp
    {
        get;
    }
    public LogLevel LogLevel
    {
        get;
    }
    public string Message
    {
        get;
    }
    public string Source
    {
        get;
    }
}