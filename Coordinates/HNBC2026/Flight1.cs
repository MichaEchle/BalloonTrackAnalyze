using Competition;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace HNBC2026;

internal class Flight1
{
    private ILogger<Flight1> _logger = LogConnector.LoggerFactory.CreateLogger<Flight1>();
    private double _separationAltitude = CoordinateHelpers.ConvertToMeter(2400);
    private Flight _flight = Flight.GetInstance();


    internal void ScoreFlight()
    {
        _logger.LogInformation("Scoring Flight 1");
        _flight.FlightNumber = 1;
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Dropbox\2026-08-05 Horb Scoring\2026 Horb Tracks", true))
        {
            _logger.LogError("Error parsing track files");
            return;
        }
        if (_flight.MapPilotNamesToTracks(@"C:\Users\micechle\Dropbox\2026-08-05 Horb Scoring"))
        {
            _logger.LogError("Error mapping pilot names to tracks");
            return;
        }
        List<Track> tracks = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber)];

    }

    private void Task1()
    {
        //TODO check launch timing
        //TODO check distance launch to goals
        //TODO check declaration to declared goals
        //TODO check task order
        //TODO check task timings
        //TODO calculate result
    }
}
