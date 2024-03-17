using Microsoft.Extensions.Logging;

namespace UILoggingProvider;
public class UILogger : ILogger
{
    private string CategoryName
    {
        get;
    }

    public UILogger(string categoryName)
    {
        CategoryName = categoryName;
    }

    public static event EventHandler<UILogEventArgs>? LogEvent;

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
        string eventText=string.Empty;
        if (!string.IsNullOrWhiteSpace(eventId.Name))
        {
            eventText = $" [{eventId.Name}]";
        }
        string source = $"{CategoryName}{eventText}";
        LogEvent?.Invoke(this, new UILogEventArgs(dateTime, logLevel, message, source));
    }
}


