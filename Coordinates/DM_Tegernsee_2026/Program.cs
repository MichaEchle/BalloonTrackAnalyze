using DM_Tegernsee_2026;
using LoggingConnector;
using Microsoft.Extensions.Logging;

internal class Program
{
    private static void Main(string[] args)
    {
        //Coordinate clp_Rottach = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 708614, 5284917, 741);
        LogConnector.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        Flight3 flight3 = new();
        flight3.ScoreFlight();

        
    }
}