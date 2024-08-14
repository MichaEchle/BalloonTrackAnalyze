using Microsoft.Extensions.Logging;

namespace LoggingConnector;

public static class LogConnector
{
    public static ILoggerFactory LoggerFactory
    {
        get; set;
    }
}
