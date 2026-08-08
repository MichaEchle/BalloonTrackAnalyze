using Competition;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using Shapes.Shapes2D;

namespace HNBC2026;

internal class Flight4
{
    private ILogger<Flight4> _logger = LogConnector.LoggerFactory.CreateLogger<Flight4>();
    private double _separationAltitude = CoordinateHelpers.ConvertToMeter(2400);
    private Flight _flight = Flight.GetInstance();

    private Coordinate _goalTask16 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 482357, 5368412, CoordinateHelpers.ConvertToMeter(1588));
    private Coordinate _goalTask17 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 481200, 5367719, CoordinateHelpers.ConvertToMeter(1629));
    private Coordinate _goalTask18 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 478710, 5366896, CoordinateHelpers.ConvertToMeter(1680));
    private Coordinate _targetTask19 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 482350, 5368420, CoordinateHelpers.ConvertToMeter(1588));

    private static List<Coordinate> _vertices_H =
   [
    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",465000,5369000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",466000,5369000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",466000,5367000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",467000,5367000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",467000,5369000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",468000,5369000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",468000,5364000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",467000,5364000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",467000,5366000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",466000,5366000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",466000,5364000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 465000,5364000,0),
            ];
    private static Polygon _h = new(_vertices_H);

    private static Rectangle _o = new(
CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 469000, 5367000, 0),
CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 472000, 5364000, 0));
    private static Rectangle _o_o = new(
CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 470000, 5366000, 0),
CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 471000, 5365000, 0));

    private static List<Coordinate> _vertices_r =
    [
    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 473000,5367000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",475000,5367000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",475000,5366000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",474000,5366000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",474000,5364000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",473000,5364000,0),
            ];
    private static Polygon _r = new(_vertices_r);

    private static List<Coordinate> _vertices_b =
   [
    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 476000,5369000,0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 477000, 5369000, 0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 477000, 5367000, 0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 479000, 5367000, 0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 479000, 5364000, 0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 476000, 5364000, 0)
    ];
    private static Polygon _b = new(_vertices_b);

    private static Rectangle _b_o = new(
      CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 477000, 5366000, 0),
      CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 478000, 5365000, 0)
        );

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

        Task19_XDD();

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
                (nameof(_goalTask18), _goalTask18),
                ]);

            foreach (var (goalName, distance) in ilp2Goals)
            {
                if (distance < 1000)
                {
                    _logger.LogWarning("Pilot {pilotNumber} launch point is within 1km of {goalName}", track.Pilot.PilotNumber, goalName);
                }
            }

            if (launchPoint.TimeStamp < new DateTime(2026, 08, 08, 04, 07, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} launch point is before the deadline", track.Pilot.PilotNumber);
            }
            if (launchPoint.TimeStamp > new DateTime(2026, 08, 08, 05, 15, 00))
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
            if (marker1.MarkerLocation.TimeStamp > new DateTime(2026, 08, 08, 05, 30, 00))
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
                }
            }
            if (marker2 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 2", track.Pilot.PilotNumber);
                continue;
            }
            if (marker2.MarkerLocation.TimeStamp > new DateTime(2026, 08, 08, 05, 30, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 2 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            double distanceToGoal = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(_goalTask17, marker2.MarkerLocation, _separationAltitude, true);
            double result = Math.Round(Math.Max(distanceToGoal, 50), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task17: {result}m ({distanceToGoal})", track.Pilot.PilotNumber, result, distanceToGoal);
        }
    }

    private void Task19_XDD()
    {
        foreach (Track track in _flight.Tracks)
        {
            MarkerDrop? marker2 = track.GetFirstMarkerDrop(2);
            MarkerDrop? marker3 = track.GetFirstMarkerDrop(3);
            if (marker2 is not null && marker3 is not null)
            {
                if (marker2.MarkerLocation.TimeStamp > marker3.MarkerLocation.TimeStamp)
                {
                    _logger.LogWarning("Pilot {pilotNumber} dropped marker2 after marker3", track.Pilot.PilotNumber);
                }
            }
            if (marker3 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 3", track.Pilot.PilotNumber);
                continue;
            }
            if (marker3.MarkerLocation.TimeStamp > new DateTime(2026, 08, 08, 07, 00, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 3 after the deadline", track.Pilot.PilotNumber);
                continue;
            }

            if(!IsInB(marker3.MarkerLocation))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 3 outside of the 'b' character", track.Pilot.PilotNumber);
                continue;
            }

            double distance = CoordinateHelpers.Calculate2DDistanceHavercos(_targetTask19, marker3.MarkerLocation);
            double result = Math.Round(distance/1000.0, 2, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task19: {result}km ({distance})", track.Pilot.PilotNumber, result, distance);
        }
    }

    private void Task20_ANG()
    {
        foreach (Track track in _flight.Tracks)
        {
            MarkerDrop? marker3 = track.GetFirstMarkerDrop(3);
            MarkerDrop? marker4 = track.GetFirstMarkerDrop(4);
            MarkerDrop? marker5 = track.GetFirstMarkerDrop(5);
            if (marker3 is not null && marker4 is not null)
            {
                if (marker3.MarkerLocation.TimeStamp > marker4.MarkerLocation.TimeStamp)
                {
                    _logger.LogWarning("Pilot {pilotNumber} dropped marker 3 after marker 4", track.Pilot.PilotNumber);
                }
            }
            if (marker4 is not null && marker5 is not null)
            {
                if (marker4.MarkerLocation.TimeStamp > marker5.MarkerLocation.TimeStamp)
                {
                    _logger.LogWarning("Pilot {pilotNumber} dropped marker 4 after marker 5", track.Pilot.PilotNumber);
                }
            }
            if (marker4 is null || marker5 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 4 or marker 5", track.Pilot.PilotNumber);
                continue;
            }
            if (marker4.MarkerLocation.TimeStamp > new DateTime(2026, 08, 08, 07, 00, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 4 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            if (marker5.MarkerLocation.TimeStamp > new DateTime(2026, 08, 08, 07, 00, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 5 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            if(!IsInR(marker4.MarkerLocation))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 4 outside of the 'r' character", track.Pilot.PilotNumber);
                continue;
            }
            if (!IsInR(marker5.MarkerLocation))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 5 outside of the 'r' character", track.Pilot.PilotNumber);
                continue;
            }

            Coordinate? firstInO = track.TrackPoints.FirstOrDefault(x => IsInO(x));
            if (firstInO is not null)
            {
                if (marker4.MarkerLocation.TimeStamp > firstInO.TimeStamp)
                {
                    _logger.LogWarning("Pilot {pilotNumber} dropped marker 4 after entering the 'o' character", track.Pilot.PilotNumber);
                }
                if (marker5.MarkerLocation.TimeStamp > firstInO.TimeStamp)
                {
                    _logger.LogWarning("Pilot {pilotNumber} dropped marker 5 after entering the 'o' character", track.Pilot.PilotNumber);
                }
            }

            double distance = CoordinateHelpers.Calculate2DDistanceHavercos(marker5.MarkerLocation, marker4.MarkerLocation);
            if (distance < 1000)
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 4 and marker 5 within 1km of each other", track.Pilot.PilotNumber);
            }
            double bearing = CoordinateHelpers.CalculateInitialBearing(marker4.MarkerLocation, marker5.MarkerLocation);
            double angle = Math.Abs(bearing - 270) % 180;
            double result = Math.Round(angle, 2, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task20: {result}deg ({bearing}/{angle})", track.Pilot.PilotNumber, result, bearing, angle);
        }
    }

    private void Task21_3D()
    {
        foreach (Track track in _flight.Tracks)
        {
            MarkerDrop? marker6 = track.GetFirstMarkerDrop(6);
            MarkerDrop? marker7 = track.GetFirstMarkerDrop(7);

            List<Coordinate> trackPointsInBigO = [.. track.TrackPoints.Where(x => _o.IsWithin(x))];
            List<Coordinate> trackPointsInHoleOfO = [.. track.TrackPoints.Where(x => _o_o.IsWithin(x))];
            if (marker6 is not null)
            {
                trackPointsInBigO = [.. trackPointsInBigO.Where(x => x.TimeStamp < marker6.MarkerLocation.TimeStamp)];
            }
            if (marker7 is not null)
            {
                trackPointsInBigO = [.. trackPointsInBigO.Where(x => x.TimeStamp < marker7.MarkerLocation.TimeStamp)];
            }

            double distanceInBigO = CoordinateHelpers.Calculate2DDistanceBetweenPoints(trackPointsInBigO);
            double distanceInHoleOfO = CoordinateHelpers.Calculate2DDistanceBetweenPoints(trackPointsInHoleOfO);
            double distance = distanceInBigO - distanceInHoleOfO;
            double result = Math.Round(distance / 1000.0, 2, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task21: {result}km ({distance})", track.Pilot.PilotNumber, result, distance);
        }
    }

    private void Task22_XDD()
    {
        foreach (Track track in _flight.Tracks)
        {
            MarkerDrop? marker6 = track.GetFirstMarkerDrop(6);
            MarkerDrop? marker7 = track.GetFirstMarkerDrop(7);
            //if (marker6 is not null && marker7 is not null)
            //{
            //    if (marker6.MarkerLocation.TimeStamp > marker7.MarkerLocation.TimeStamp)
            //    {
            //        _logger.LogWarning("Pilot {pilotNumber} dropped marker 6 after marker 7", track.Pilot.PilotNumber);
            //    }
            //}
            if (marker6 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 6", track.Pilot.PilotNumber);
                continue;
            }
            if (marker6.MarkerLocation.TimeStamp > new DateTime(2026, 08, 08, 07, 00, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 6 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            if (marker7 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 7", track.Pilot.PilotNumber);
                continue;
            }
            if (marker7.MarkerLocation.TimeStamp > new DateTime(2026, 08, 08, 07, 00, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 7 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            if(!IsInH(marker6.MarkerLocation))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 6 outside of the 'H' character", track.Pilot.PilotNumber);
                continue;
            }
            if (!IsInH(marker7.MarkerLocation))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 7 outside of the 'H' character", track.Pilot.PilotNumber);
                continue;
            }
            double distance = CoordinateHelpers.Calculate2DDistanceHavercos(marker6.MarkerLocation, marker7.MarkerLocation);
            double result = Math.Round(distance/1000.0, 2, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task22: {result}km ({distance})", track.Pilot.PilotNumber, result, distance);
        }
    }

    private bool IsInB(Coordinate coordinate)
    {
        return _b.IsWithin(coordinate) && !_b_o.IsWithin(coordinate);
    }

    private bool IsInR(Coordinate coordinate)
    {
        return _r.IsWithin(coordinate);
    }

    private bool IsInO(Coordinate coordinate)
    {
        bool inO= _o.IsWithin(coordinate);
        bool inO_O = _o_o.IsWithin(coordinate);

        if (inO && inO_O)
        {
            double dummy = 0;
        }

        return _o.IsWithin(coordinate) && !_o_o.IsWithin(coordinate);
    }


    private bool IsInH(Coordinate coordinate)
    {
        return _h.IsWithin(coordinate);
    }

    #region Cancelled tasks
    //private Coordinate _goalTask16 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 480170, 5373089, CoordinateHelpers.ConvertToMeter(1733));
    //private Coordinate _goalTask17 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 479380, 5371250, CoordinateHelpers.ConvertToMeter(3000));
    //private Coordinate _goalTask18A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 477642, 5368474, CoordinateHelpers.ConvertToMeter(1556));
    //private Coordinate _goalTask18B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 478562, 5368433, CoordinateHelpers.ConvertToMeter(1589));
    //private Coordinate _goalTask18C = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 479067, 5368987, 452);

    //internal void ScoreFlight()
    //{
    //    _logger.LogInformation("Scoring Flight 4");
    //    _flight.FlightNumber = 4;
    //    _flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(3500));
    //    if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Dropbox\2026-08-05 Horb Scoring\2026 Horb Tracks\Flight 4\F4 IGC", true, _goalTask16))
    //    {
    //        _logger.LogError("Error parsing track files");
    //        return;
    //    }
    //    if (!_flight.MapPilotNamesToTracks(@"C:\Users\micechle\Dropbox\2026-08-05 Horb Scoring\2026-Horb Documents\HNBC_26_Pilots_Mapping.csv"))
    //    {
    //        _logger.LogError("Error mapping pilot names to tracks");
    //        return;
    //    }
    //    _flight.Tracks = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber)];


    //}

    //private void CheckILP2Goals()
    //{
    //    foreach (var track in _flight.Tracks)
    //    {
    //        if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
    //        {
    //            _logger.LogWarning("Pilot {pilotNumber} could not estimate launch and landing time", track.Pilot.PilotNumber);
    //            continue;
    //        }

    //        var ilp2Goals = TrackHelpers.Calculate2DDistanceBetweenLaunchPointAndJudeDeclaredGoals(launchPoint,
    //            [(nameof(_goalTask16), _goalTask16),
    //            (nameof(_goalTask17), _goalTask17),
    //            (nameof(_goalTask18A), _goalTask18A),
    //            (nameof(_goalTask18B), _goalTask18B),
    //            (nameof(_goalTask18C), _goalTask18C),
    //            ]);

    //        foreach (var (goalName, distance) in ilp2Goals)
    //        {
    //            if (distance < 1000)
    //            {
    //                _logger.LogWarning("Pilot {pilotNumber} launch point is within 1km of {goalName}", track.Pilot.PilotNumber, goalName);
    //            }
    //        }

    //        if (launchPoint.TimeStamp < new DateTime(2026, 08, 07, 17, 30, 00))
    //        {
    //            _logger.LogWarning("Pilot {pilotNumber} launch point is before the deadline", track.Pilot.PilotNumber);
    //        }
    //        if (launchPoint.TimeStamp > new DateTime(2026, 08, 07, 18, 30, 00))
    //        {
    //            _logger.LogWarning("Pilot {pilotNumber} launch point is after the deadline", track.Pilot.PilotNumber);
    //        }
    //    }
    //}

    //private void Task16_FIN()
    //{
    //    foreach (Track track in _flight.Tracks)
    //    {
    //        MarkerDrop? marker1 = track.GetFirstMarkerDrop(1);
    //        if (marker1 is null)
    //        {
    //            _logger.LogWarning("Pilot {pilotNumber} did not drop marker 1", track.Pilot.PilotNumber);
    //            continue;
    //        }
    //        if (marker1.MarkerLocation.TimeStamp > new DateTime(2026, 08, 07, 18, 50, 00))
    //        {
    //            _logger.LogWarning("Pilot {pilotNumber} dropped marker 1 after the deadline", track.Pilot.PilotNumber);
    //            continue;
    //        }
    //        double distanceToGoal = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(_goalTask16, marker1.MarkerLocation, _separationAltitude, true);
    //        double result = Math.Round(Math.Max(distanceToGoal, 50), 0, MidpointRounding.AwayFromZero);
    //        _logger.LogInformation("Pilot {pilotNumber} result for Task16: {result}m ({distanceToGoal})", track.Pilot.PilotNumber, result, distanceToGoal);
    //    }
    //}

    //private void Task17_JDG()
    //{
    //    foreach (Track track in _flight.Tracks)
    //    {
    //        MarkerDrop? marker1 = track.GetFirstMarkerDrop(1);
    //        MarkerDrop? marker2 = track.GetFirstMarkerDrop(2);

    //        if (marker1 is not null && marker2 is not null)
    //        {
    //            if (marker1.MarkerLocation.TimeStamp > marker2.MarkerLocation.TimeStamp)
    //            {
    //                _logger.LogWarning("Pilot {pilotNumber} dropped marker 1 after marker 2", track.Pilot.PilotNumber);
    //                continue;
    //            }
    //        }

    //        if (marker2 is null)
    //        {
    //            _logger.LogWarning("Pilot {pilotNumber} did not drop marker 2", track.Pilot.PilotNumber);
    //            continue;
    //        }
    //        if (marker2.MarkerLocation.TimeStamp > new DateTime(2026, 08, 07, 18, 50, 00))
    //        {
    //            _logger.LogWarning("Pilot {pilotNumber} dropped marker 2 after the deadline", track.Pilot.PilotNumber);
    //            continue;
    //        }
    //        double distanceToGoal = CoordinateHelpers.Calculate3DDistance(_goalTask17, marker2.MarkerLocation, true);
    //        double result = Math.Round(distanceToGoal, 0, MidpointRounding.AwayFromZero);
    //        _logger.LogInformation("Pilot {pilotNumber} result for Task17: {result}m ({distanceToGoal})", track.Pilot.PilotNumber, result, distanceToGoal);
    //    }
    //}

    //private void Task18_HWZ()
    //{
    //    foreach (var track in _flight.Tracks)
    //    {
    //        MarkerDrop? marker2 = track.GetFirstMarkerDrop(2);
    //        MarkerDrop? marker3 = track.GetFirstMarkerDrop(3);
    //        if (marker2 is not null && marker3 is not null)
    //        {
    //            if (marker2.MarkerLocation.TimeStamp > marker3.MarkerLocation.TimeStamp)
    //            {
    //                _logger.LogWarning("Pilot {pilotNumber} dropped marker 2 after marker 3", track.Pilot.PilotNumber);
    //                continue;
    //            }
    //        }
    //        if (marker3 is null)
    //        {
    //            _logger.LogWarning("Pilot {pilotNumber} did not drop marker 3", track.Pilot.PilotNumber);
    //            continue;
    //        }
    //        if (marker3.MarkerLocation.TimeStamp > new DateTime(2026, 08, 07, 18, 50, 00))
    //        {
    //            _logger.LogWarning("Pilot {pilotNumber} dropped marker 3 after the deadline", track.Pilot.PilotNumber);
    //            continue;
    //        }
    //        HesitationWaltzTask hesitationWaltzTask = new HesitationWaltzTask();
    //        hesitationWaltzTask.SetupHWZ(18, [_goalTask18A, _goalTask18B, _goalTask18C], 3, DistanceCalculationType.WithSeparationAlitude, null, ValidationStrictnessType.First);
    //        hesitationWaltzTask.SeparationAltitude = _separationAltitude;
    //        if (!hesitationWaltzTask.CalculateResults(track, true, out double distance))
    //        {
    //            _logger.LogWarning("Failed to calculate distance for Pilot {pilotNumber} at Task18", track.Pilot.PilotNumber);
    //        }
    //        double result = Math.Round(Math.Max(distance, 50), 0, MidpointRounding.AwayFromZero);
    //        _logger.LogInformation("Pilot {pilotNumber} result for Task18: {result}m ({distance})", track.Pilot.PilotNumber, result, distance);
    //    }
    //}

    #endregion
}
