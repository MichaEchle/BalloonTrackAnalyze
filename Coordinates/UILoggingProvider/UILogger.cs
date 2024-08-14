using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace UILoggingProvider;
public class UILogger : ILogger
{
    private string CategoryName
    {
        get;
    }

    private ChannelWriter<LogItem> LogWriter
    {
        get;
    }

    public UILogger(string categoryName, ChannelWriter<LogItem> logWriter)
    {
        CategoryName = categoryName;
        LogWriter = logWriter;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string message = formatter(state, exception);
        DateTime dateTime = DateTime.Now;
        string eventText = string.Empty;
        if (!string.IsNullOrWhiteSpace(eventId.Name))
        {
            eventText = $" [{eventId.Name}]";
        }
        string source = $"{CategoryName}{eventText}";
        _ = LogWriter.TryWrite(new LogItem(dateTime, logLevel, message, source));
    }
}


