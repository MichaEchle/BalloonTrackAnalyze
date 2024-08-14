using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace UILoggingProvider;
public class UILoggerProvider : ILoggerProvider
{

    private readonly ConcurrentDictionary<string, UILogger> Loggers = new();

    private readonly Channel<LogItem> LogItemChannel = Channel.CreateUnbounded<LogItem>(new() { SingleReader = true, SingleWriter = false, AllowSynchronousContinuations = false });

    public ChannelReader<LogItem> LogItemReader => LogItemChannel.Reader;

    private static readonly UILoggerProvider _instance = new();

    public static UILoggerProvider Instance => _instance;

    private UILoggerProvider()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return Loggers.GetOrAdd(categoryName, new UILogger(categoryName, LogItemChannel.Writer));
    }

    public void Dispose()
    {
        Loggers.Clear();
        GC.SuppressFinalize(this);
    }
}
