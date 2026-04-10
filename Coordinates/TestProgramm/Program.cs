using Competition.Penalty;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace TestProgramm;

internal class Program
{


    private static void Main(string[] args)
    {
         
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        LogConnector.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        EuropeansWieselburg2025 competition = new();
        
        
        competition.Score(false, 9,1010);
        //competition.Score(false, 99,1005, @"C:\Users\Jan\Nextcloud2\shared\Ballonveranstaltungen\2025 Wieselburg\scoring\flights\flight_08\tracks\special\#36");
        
    }
    
    
    




    private static string ToProperText(CoordinateSharp.CoordinatePart part)
    {
        string text = part.Degrees + "° " + part.Minutes + "ʹ " + Math.Round(part.Seconds, 2, MidpointRounding.AwayFromZero) + "ʺ";
        return text;
    }

}
