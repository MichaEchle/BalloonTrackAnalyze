using HNBC2026;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System.Globalization;

internal class Program
{
    private static void Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        LogConnector.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        Flight3 flight3 = new();
        flight3.ScoreFlight();
    }
}