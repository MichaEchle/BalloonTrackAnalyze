using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace UILoggingProvider;
public class UILoggerProvider : ILoggerProvider
{

    private readonly ConcurrentDictionary<string, UILogger> Loggers = new();

    public ILogger CreateLogger(string categoryName)
    {
        return Loggers.GetOrAdd(categoryName, new UILogger(categoryName));
    }

    public void Dispose()
    {
        Loggers.Clear();
        GC.SuppressFinalize(this);
    }
}
