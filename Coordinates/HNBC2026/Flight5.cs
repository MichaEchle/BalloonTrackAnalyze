using Competition;
using Competition.Tasks;
using Competition.Validation;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace HNBC2026;

internal class Flight5
{
    private ILogger<Flight5> _logger = LogConnector.LoggerFactory.CreateLogger<Flight5>();
    private double _separationAltitude = CoordinateHelpers.ConvertToMeter(2400);
    private Flight _flight = Flight.GetInstance();

    private Coordinate _goalTask23 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 482230, 5368710, CoordinateHelpers.ConvertToMeter(1570));
    private Coordinate _goalTask24 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 474415, 5367861, CoordinateHelpers.ConvertToMeter(1792));
    private Coordinate _goalTask25A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 477643, 5368474, CoordinateHelpers.ConvertToMeter(1567));
    private Coordinate _goalTask25B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 478565, 5368435, CoordinateHelpers.ConvertToMeter(1593));

    internal void ScoreFlight()
    {
        _logger.LogInformation("Scoring Flight 5");
        _flight.FlightNumber = 5;
        _flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(3500));
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Dropbox\2026-08-05 Horb Scoring\2026 Horb Tracks\Flight 5\F5 IGC", true, _goalTask23))
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

        Task23_FIN();
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
                [(nameof(_goalTask23), _goalTask23),
                (nameof(_goalTask24), _goalTask24),
                (nameof(_goalTask25A), _goalTask25A),
                (nameof(_goalTask25B), _goalTask25B),
                ]);

            foreach (var (goalName, distance) in ilp2Goals)
            {
                if (distance < 750)
                {
                    _logger.LogWarning("Pilot {pilotNumber} launch point is within 750m of {goalName}", track.Pilot.PilotNumber, goalName);
                }
            }

            if (launchPoint.TimeStamp < new DateTime(2026, 08, 08, 17, 30, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} launch point is before the deadline", track.Pilot.PilotNumber);
            }
            if (launchPoint.TimeStamp > new DateTime(2026, 08, 08, 18, 30, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} launch point is after the deadline", track.Pilot.PilotNumber);
            }
        }
    }

    private void Task23_FIN()
    {
        foreach (Track track in _flight.Tracks)
        {
            MarkerDrop? marker1 = track.GetFirstMarkerDrop(1);
            if (marker1 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 1", track.Pilot.PilotNumber);
                continue;
            }
            if (marker1.MarkerLocation.TimeStamp > new DateTime(2026, 08, 08, 18, 50, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 1 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            double distance2D=CoordinateHelpers.Calculate2DDistanceHavercos(_goalTask23, marker1.MarkerLocation);
            double distance3D = CoordinateHelpers.Calculate3DDistance(_goalTask23, marker1.MarkerLocation,true);
            double distanceToGoal = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(_goalTask23, marker1.MarkerLocation, _separationAltitude, true);
            double result = Math.Round(Math.Max(distanceToGoal, 50), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task23: {result}m ({distanceToGoal}, 2D {distance2D}, 3D {distance3D})", track.Pilot.PilotNumber, result, distanceToGoal, distance2D, distance3D);
        }
    }

    private void Task24_WSD()
    {
        foreach (Track track in _flight.Tracks)
        {
            MarkerDrop? marker1 = track.GetFirstMarkerDrop(1);
            MarkerDrop? marker2 = track.GetFirstMarkerDrop(2);
            if (marker1 is not null && marker2 is not null)
            {
                if(marker1.MarkerLocation.TimeStamp > marker2.MarkerLocation.TimeStamp)
                {
                    _logger.LogWarning("Pilot {pilotNumber} dropped marker 1 after marker 2", track.Pilot.PilotNumber);
                }
            }
            if (marker2 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 2", track.Pilot.PilotNumber);
                continue;
            }
            if (marker2.MarkerLocation.TimeStamp > new DateTime(2026, 08, 08, 18, 50, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 2 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            double distanceToGoal = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(_goalTask24, marker2.MarkerLocation, _separationAltitude, true);
            double result = Math.Round(Math.Max(distanceToGoal, 30), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task24: {result}m ({distanceToGoal})", track.Pilot.PilotNumber, result, distanceToGoal);
        }
    }

    private void Task25_HWZ()
    {
        foreach (Track track in _flight.Tracks)
        {
            MarkerDrop? marker3 = track.GetFirstMarkerDrop(3);
            if (marker3 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 3", track.Pilot.PilotNumber);
                continue;
            }
            if (marker3.MarkerLocation.TimeStamp > new DateTime(2026, 08, 08, 18, 50, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 3 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            HesitationWaltzTask hesitationWaltzTask = new ();
            hesitationWaltzTask.SetupHWZ(25, [_goalTask25A,_goalTask25B], 3, DistanceCalculationType.WithSeparationAlitude, null, ValidationStrictnessType.First);
            hesitationWaltzTask.SeparationAltitude = _separationAltitude;
            if(!hesitationWaltzTask.CalculateResults(track, true, out double distanceToGoal))
            {
                _logger.LogWarning("Pilot {pilotNumber} could not calculate results for Task25", track.Pilot.PilotNumber);
                continue;
            }
            double result = Math.Round(Math.Max(distanceToGoal, 50), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task25: {result}m ({distanceToGoal})", track.Pilot.PilotNumber, result, distanceToGoal);
        }
    }

}
