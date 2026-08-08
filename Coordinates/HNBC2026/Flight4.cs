using Competition;
using Competition.Tasks;
using Competition.Validation;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace HNBC2026;

internal class Flight4
{
    private ILogger<Flight4> _logger = LogConnector.LoggerFactory.CreateLogger<Flight4>();
    private double _separationAltitude = CoordinateHelpers.ConvertToMeter(2400);
    private Flight _flight = Flight.GetInstance();

    private Coordinate _goalTask16 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 480170, 5373089, CoordinateHelpers.ConvertToMeter(1733));
    private Coordinate _goalTask17 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 479380, 5371250, CoordinateHelpers.ConvertToMeter(3000));
    private Coordinate _goalTask18A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 477642, 5368474, CoordinateHelpers.ConvertToMeter(1556));
    private Coordinate _goalTask18B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 478562, 5368433, CoordinateHelpers.ConvertToMeter(1589));
    private Coordinate _goalTask18C = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 479067, 5368987, 452);

    internal void ScoreFlight()
    {
        _logger.LogInformation("Scoring Flight 4");
        _flight.FlightNumber = 4;
        _flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(3500));
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Dropbox\2026-08-05 Horb Scoring\2026 Horb Tracks\Flight 4\F4 IGC", true, _goalTask16))
        {
            _logger.LogError("Error parsing track files");
            return;
        }
        if (!_flight.MapPilotNamesToTracks(@"C:\Users\micechle\Dropbox\2026-08-05 Horb Scoring\2026-Horb Documents\HNBC_26_Pilots_Mapping.csv"))
        {
            _logger.LogError("Error mapping pilot names to tracks");
            return;
        }
        _flight.Tracks = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber)];


    }

    private void CheckILP2Goals()
    {
        foreach (var track in _flight.Tracks)
        {
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                _logger.LogWarning("Pilot {pilotNumber} could not estimate launch and landing time", track.Pilot.PilotNumber);
                continue;
            }

            var ilp2Goals = TrackHelpers.Calculate2DDistanceBetweenLaunchPointAndJudeDeclaredGoals(launchPoint,
                [(nameof(_goalTask16), _goalTask16),
                (nameof(_goalTask17), _goalTask17),
                (nameof(_goalTask18A), _goalTask18A),
                (nameof(_goalTask18B), _goalTask18B),
                (nameof(_goalTask18C), _goalTask18C),
                ]);

            foreach (var (goalName, distance) in ilp2Goals)
            {
                if (distance < 1000)
                {
                    _logger.LogWarning("Pilot {pilotNumber} launch point is within 1km of {goalName}", track.Pilot.PilotNumber, goalName);
                }
            }

            if (launchPoint.TimeStamp < new DateTime(2026, 08, 07, 17, 30, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} launch point is before the deadline", track.Pilot.PilotNumber);
            }
            if (launchPoint.TimeStamp > new DateTime(2026, 08, 07, 18, 30, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} launch point is after the deadline", track.Pilot.PilotNumber);
            }
        }
    }

    private void Task16_FIN()
    {
        foreach (Track track in _flight.Tracks)
        {
            MarkerDrop? marker1 = track.GetFirstMarkerDrop(1);
            if (marker1 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 1", track.Pilot.PilotNumber);
                continue;
            }
            if (marker1.MarkerLocation.TimeStamp > new DateTime(2026, 08, 07, 18, 50, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 1 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            double distanceToGoal = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(_goalTask16, marker1.MarkerLocation, _separationAltitude, true);
            double result = Math.Round(Math.Max(distanceToGoal, 50), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task16: {result}m ({distanceToGoal})", track.Pilot.PilotNumber, result, distanceToGoal);
        }
    }

    private void Task17_JDG()
    {
        foreach (Track track in _flight.Tracks)
        {
            MarkerDrop? marker1 = track.GetFirstMarkerDrop(1);
            MarkerDrop? marker2 = track.GetFirstMarkerDrop(2);

            if (marker1 is not null && marker2 is not null)
            {
                if (marker1.MarkerLocation.TimeStamp > marker2.MarkerLocation.TimeStamp)
                {
                    _logger.LogWarning("Pilot {pilotNumber} dropped marker 1 after marker 2", track.Pilot.PilotNumber);
                    continue;
                }
            }

            if (marker2 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 2", track.Pilot.PilotNumber);
                continue;
            }
            if (marker2.MarkerLocation.TimeStamp > new DateTime(2026, 08, 07, 18, 50, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 2 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            double distanceToGoal = CoordinateHelpers.Calculate3DDistance(_goalTask17, marker2.MarkerLocation, true);
            double result = Math.Round(distanceToGoal, 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task17: {result}m ({distanceToGoal})", track.Pilot.PilotNumber, result, distanceToGoal);
        }
    }

    private void Task18_HWZ()
    {
        foreach (var track in _flight.Tracks)
        {
            MarkerDrop? marker2 = track.GetFirstMarkerDrop(2);
            MarkerDrop? marker3 = track.GetFirstMarkerDrop(3);
            if (marker2 is not null && marker3 is not null)
            {
                if (marker2.MarkerLocation.TimeStamp > marker3.MarkerLocation.TimeStamp)
                {
                    _logger.LogWarning("Pilot {pilotNumber} dropped marker 2 after marker 3", track.Pilot.PilotNumber);
                    continue;
                }
            }
            if (marker3 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 3", track.Pilot.PilotNumber);
                continue;
            }
            if (marker3.MarkerLocation.TimeStamp > new DateTime(2026, 08, 07, 18, 50, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 3 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            HesitationWaltzTask hesitationWaltzTask = new HesitationWaltzTask();
            hesitationWaltzTask.SetupHWZ(18, [_goalTask18A, _goalTask18B, _goalTask18C], 3, DistanceCalculationType.WithSeparationAlitude, null, ValidationStrictnessType.First);
            hesitationWaltzTask.SeparationAltitude = _separationAltitude;
            if (!hesitationWaltzTask.CalculateResults(track, true, out double distance))
            {
                _logger.LogWarning("Failed to calculate distance for Pilot {pilotNumber} at Task18", track.Pilot.PilotNumber);
            }
            double result = Math.Round(Math.Max(distance, 50), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task18: {result}m ({distance})", track.Pilot.PilotNumber, result, distance);
        }
    }
}
