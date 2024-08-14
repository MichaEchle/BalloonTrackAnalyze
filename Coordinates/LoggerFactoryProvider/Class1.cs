using Microsoft.Extensions.Logging;

namespace LoggerFactoryProvider;

public static class LoggerFactoryProvider
{
    public static ILoggerFactory LoggerFactory
    {
        get; set;
    }
}
