using Microsoft.Extensions.Logging;

namespace Competition.Configuration;
public class CompetitionLoggingConnector
{
    public static ILoggerFactory LoggerFactory
    {
        get
        {
            _loggerFactory ??= new LoggerFactory();
            return _loggerFactory;
        }
        private set
        {
            _loggerFactory = value;
        }
    }

    private static ILoggerFactory _loggerFactory;


    public CompetitionLoggingConnector(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }
}
