using Competition;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace HNBC2026;

internal class Flight3
{
    private ILogger<Flight3> _logger = LogConnector.LoggerFactory.CreateLogger<Flight3>();
    private double _separationAltitude = CoordinateHelpers.ConvertToMeter(2400);
    private Flight _flight = Flight.GetInstance();

    private Coordinate _centerTask10 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 475600, 5364800, 0);

    private Coordinate _goalTask11 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 477950, 5367402, 481);
    private Coordinate _goalTask12 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 475781, 5365869, CoordinateHelpers.ConvertToMeter(1784));
    private Coordinate _goalTask13 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 474903, 5364191, 0);
    private Coordinate _goalTask14 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 473313, 5363286, CoordinateHelpers.ConvertToMeter(1306));

    internal void ScoreFlight()
    {
        _logger.LogInformation("Scoring Flight 3");
        _flight.FlightNumber = 3;
        _flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(3500));
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Dropbox\2026-08-05 Horb Scoring\2026 Horb Tracks\Flight 3\F3 IGC", true,_goalTask14))
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

        Task15_RTA();
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
                [(nameof(_centerTask10), _centerTask10),
                (nameof(_goalTask11), _goalTask11),
                (nameof(_goalTask12), _goalTask12),
                (nameof(_goalTask13), _goalTask13),
                (nameof(_goalTask14), _goalTask14),
                ]);

            foreach (var (goalName, distance) in ilp2Goals)
            {
                if (distance < 1000)
                {
                    _logger.LogWarning("Pilot {pilotNumber} launch point is within 1km of {goalName}", track.Pilot.PilotNumber, goalName);
                }
            }
        }
    }

    private void Task10_XID()
    {
        foreach (var track in _flight.Tracks)
        {
            List<Coordinate> trackPoints = [..track.TrackPoints.SkipWhile(x=>
                CoordinateHelpers.Calculate2DDistanceHavercos(x,_centerTask10) > 3600)
                .TakeWhile(x =>
                CoordinateHelpers.Calculate2DDistanceHavercos(x, _centerTask10) <= 3600
                && x.TimeStamp <= new DateTime(2026,08,07,06,30,00))];

            double distance = CoordinateHelpers.Calculate2DDistanceBetweenPoints(trackPoints);
            double result = Math.Round(distance/1000.0, 2, MidpointRounding.AwayFromZero);

            //Coordinate entryPoint = trackPoints.First();
            //DateTime entryTime = entryPoint.TimeStamp;
            //(string zone, int easting, int northing) entryLoc = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(entryPoint);

            //Coordinate exitPoint = trackPoints.Last();
            //DateTime exitTime = exitPoint.TimeStamp;
            //(string zone, int easting, int northing) exitLoc = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(exitPoint);

            //int count = trackPoints.Count;

            //_logger.LogInformation("{pilotnumber},{entryTime},{entryEast},{entryNorth},{exitTime},{exitEast},{exitNorth},{count},{result}", track.Pilot.PilotNumber, entryTime, entryLoc.easting, entryLoc.northing, exitTime, exitLoc.easting, exitLoc.northing, count, result);
            _logger.LogInformation("Pilot {pilotNumber} result for Task 10 XID: {result}km ({distance}m)", track.Pilot.PilotNumber, result, distance);
        }
    }

    private void Task11_FIN()
    {
        foreach (var track in _flight.Tracks)
        {
            MarkerDrop? marker1 = track.GetFirstMarkerDrop(1);

            List<Coordinate> trackPoints = [..track.TrackPoints.SkipWhile(x=>
                CoordinateHelpers.Calculate2DDistanceHavercos(x,_centerTask10) > 3600)
                .TakeWhile(x =>
                CoordinateHelpers.Calculate2DDistanceHavercos(x, _centerTask10) <= 3600)];
            if (marker1 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 1", track.Pilot.PilotNumber);
                continue;
            }
            if (marker1.MarkerLocation.TimeStamp > new DateTime(2026, 08, 07, 05, 30, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 1 after the deadline", track.Pilot.PilotNumber);
                continue;
            }

            if (marker1.MarkerLocation.TimeStamp > trackPoints.Last().TimeStamp)
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 1 after leaving the 3.6km radius", track.Pilot.PilotNumber);
                continue;
            }

            double distanceToGoal = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(_goalTask11, marker1.MarkerLocation, _separationAltitude, true);
            double result = Math.Round(Math.Max(distanceToGoal, 30), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task11: {result}m ({distanceToGoal})", track.Pilot.PilotNumber, result, distanceToGoal);
        }
    }

    private void Task12_JDG()
    {
        foreach (var track in _flight.Tracks)
        {
            List<Coordinate> trackPoints = [..track.TrackPoints.SkipWhile(x=>
                CoordinateHelpers.Calculate2DDistanceHavercos(x,_centerTask10) > 3600)
                .TakeWhile(x =>
                CoordinateHelpers.Calculate2DDistanceHavercos(x, _centerTask10) <= 3600)];

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
            if (marker2.MarkerLocation.TimeStamp > new DateTime(2026, 08, 07, 06, 00, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 2 after the deadline", track.Pilot.PilotNumber);
                continue;
            }

            if (marker2.MarkerLocation.TimeStamp > trackPoints.Last().TimeStamp)
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 2 after leaving the 3.6km radius", track.Pilot.PilotNumber);
                continue;
            }

            double distanceToGoal = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(_goalTask12, marker2.MarkerLocation, _separationAltitude, true);
            double result = Math.Round(Math.Max(distanceToGoal, 50), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task12: {result}m ({distanceToGoal})", track.Pilot.PilotNumber, result, distanceToGoal);
        }
    }

    private void Task14_JDG()
    {
        foreach (var track in _flight.Tracks)
        {
            MarkerDrop? marker3 = track.GetFirstMarkerDrop(3);
            if (marker3 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 3", track.Pilot.PilotNumber);
                continue;
            }
            if (marker3.MarkerLocation.TimeStamp > new DateTime(2026, 08, 07, 06, 45, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 3 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            double distanceToGoal = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(_goalTask14, marker3.MarkerLocation, _separationAltitude, true);
            double result = Math.Round(Math.Max(distanceToGoal, 50), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task14: {result}m ({distanceToGoal})", track.Pilot.PilotNumber, result, distanceToGoal);
        }
    }

    private void Task15_RTA()
    {
        foreach (var track in _flight.Tracks)
        {
            Coordinate? reference = track.TrackPoints.SkipWhile(x =>
                CoordinateHelpers.Calculate2DDistanceHavercos(x, _centerTask10) > 3600)
                .TakeWhile(x =>
                CoordinateHelpers.Calculate2DDistanceHavercos(x, _centerTask10) <= 3600).LastOrDefault();

            if (reference is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} does not have a trackpoint leaving the 3.6km circle", track.Pilot.PilotNumber);
                continue;
            }

            Declaration? declaration = track.Declarations.Where(x => x.GoalNumber == 1 && x.PositionAtDeclaration.TimeStamp < reference.TimeStamp).MaxBy(x => x.PositionAtDeclaration.TimeStamp);

            if (declaration is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} does not have a valid declaration for Task 15", track.Pilot.PilotNumber);
                continue;
            }

            Coordinate? firstOnOrOverGrid = track.TrackPoints.SkipWhile(x => x.TimeStamp <= reference.TimeStamp).FirstOrDefault(x => IsOnOrOverGrid(x));

            if (firstOnOrOverGrid is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} does not have a trackpoint on or over the grid", track.Pilot.PilotNumber);
                continue;
            }

            Coordinate entryPoint = reference;
            DateTime entryTime = entryPoint.TimeStamp;
            (string zone, int easting, int northing) entryLoc = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(entryPoint);

            Coordinate exitPoint = firstOnOrOverGrid;
            DateTime exitTime = exitPoint.TimeStamp;
            (string zone, int easting, int northing) exitLoc = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(exitPoint);

            TimeSpan timeToGrid = firstOnOrOverGrid.TimeStamp - reference.TimeStamp;
            int minutes = (int)Math.Floor(declaration.OrignalNorhtingDeclarationUTM / 100.0);
            int seconds = declaration.OrignalNorhtingDeclarationUTM % 100;
            TimeSpan declaredTime = TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);

            int result = (int)Math.Round(Math.Abs((timeToGrid - declaredTime).TotalSeconds), 0, MidpointRounding.AwayFromZero);

            _logger.LogInformation("{pilotNumber},{entryTime},{entryEasting},{entryNorthing},{exitTime},{exitEasting},{exitNorthing},{timeToGrid},{declaredTime},{result}", track.Pilot.PilotNumber, entryTime, entryLoc.easting, entryLoc.northing, exitTime, exitLoc.easting, exitLoc.northing, timeToGrid, declaredTime, result);

            //_logger.LogInformation("Pilot {pilotNumber} result for Task15: {result}s (timeToGrid: {timeToGrid}, declaredTime: {declaredTime})", track.Pilot.PilotNumber, result, timeToGrid, declaredTime);
        }
    }

    private bool IsOnOrOverGrid(Coordinate coordinate)
    {
        (_, int easting, _) = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(coordinate);
        return easting >= 481000;
    }
}
