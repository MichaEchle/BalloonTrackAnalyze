using Competition;
using Competition.Tasks;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace HNBC2026;

internal class Flight1
{
    private ILogger<Flight1> _logger = LogConnector.LoggerFactory.CreateLogger<Flight1>();
    private double _separationAltitude = CoordinateHelpers.ConvertToMeter(2400);
    private Flight _flight = Flight.GetInstance();
    //32U 477171 5365722 // Coordinates Horb
    private Coordinate _goalTask1 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 470887, 5363463,457);
    private Coordinate _goalTask2 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 473311, 5363286,CoordinateHelpers.ConvertToMeter(1309));
    private Coordinate _goalTask3A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 474901, 5364190,CoordinateHelpers.ConvertToMeter(1285));
    private Coordinate _goalTask3B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 474989, 5364120,CoordinateHelpers.ConvertToMeter(1286));
    private Coordinate _goalTask4A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 476800, 5364201,CoordinateHelpers.ConvertToMeter(1747));
    private Coordinate _goalTask4B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 477147, 5364839,517);

    internal void ScoreFlight()
    {
        _logger.LogInformation("Scoring Flight 1");
        _flight.FlightNumber = 1;
        _flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(3500));
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Dropbox\2026-08-05 Horb Scoring\2026 Horb Tracks\Flight 1\F1 IGC", true))
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

        Task6_ANG();

    }

    private void Task1_FIN()
    {
        foreach (Track track in _flight.Tracks)
        {
            //TODO check launch timing
            //TODO check distance launch to goals
            //TODO check declaration to declared goals
            //TODO check task order
            //TODO check task timings
            //TODO calculate result

            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                _logger.LogError("Error estimating launch and landing time for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            if (launchPoint.TimeStamp < new DateTime(2026, 08, 06, 04, 04, 00) || launchPoint.TimeStamp > new
            DateTime(2026, 08, 06, 05, 15, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} launched outside of launch window: {launchTime}", track.Pilot.PilotNumber, launchPoint.TimeStamp);
            }
            double distanceLaunchToGoal1 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, _goalTask1);
            if (distanceLaunchToGoal1 < 1000)
            {
                _logger.LogWarning("Pilot {pilotNumber} launch to goal 1 distance is out of bounds: {distance} meters", track.Pilot.PilotNumber, distanceLaunchToGoal1);
            }
            MarkerDrop? marker1 = track.GetFirstMarkerDrop(1);
            if (marker1 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 1", track.Pilot.PilotNumber);
                continue;
            }

            if (marker1.MarkerLocation.TimeStamp > new DateTime(2026, 08, 06, 05, 45, 0))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 1 outside of allowed time: {markerTime}", track.Pilot.PilotNumber, marker1.MarkerLocation.TimeStamp);
            }
            double distanceMarker1ToGoal1 = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(marker1.MarkerLocation, _goalTask1, _separationAltitude, true);
            double result = Math.Round(Math.Max(40, distanceMarker1ToGoal1), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} distance from marker 1 to goal 1: {result}m ({distance})", track.Pilot.PilotNumber, result, distanceMarker1ToGoal1);

        }
    }

    private void Task2_JDG()
    {
        foreach (Track track in _flight.Tracks)
        {
            //TODO check distance launch to goals
            //TODO check task order
            //TODO check task timings
            //TODO calculate result
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                _logger.LogError("Error estimating launch and landing time for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            double distanceLaunchToGoal2 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, _goalTask2);
            if (distanceLaunchToGoal2 < 1000)
            {
                _logger.LogWarning("Pilot {pilotNumber} launch to goal 2 distance is out of bounds: {distance} meters", track.Pilot.PilotNumber, distanceLaunchToGoal2);
            }
            MarkerDrop? marker1 = track.GetFirstMarkerDrop(1);
            MarkerDrop? marker2 = track.GetFirstMarkerDrop(2);
            if (marker1 is not null && marker2 is not null)
            {
                if (marker1.MarkerLocation.TimeStamp > marker2.MarkerLocation.TimeStamp)
                {
                    _logger.LogWarning("Pilot {pilotNumber} dropped marker 1 after marker 2: {marker1Time} > {marker2Time}", track.Pilot.PilotNumber, marker1.MarkerLocation.TimeStamp, marker2.MarkerLocation.TimeStamp);
                }
            }
            if (marker2 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 2", track.Pilot.PilotNumber);
                continue;
            }

            if (marker2.MarkerLocation.TimeStamp > new DateTime(2026, 08, 06, 06, 00, 0))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 2 outside of allowed time: {markerTime}", track.Pilot.PilotNumber, marker2.MarkerLocation.TimeStamp);
            }
            double distanceMarker2ToGoal2 = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(marker2.MarkerLocation, _goalTask2, _separationAltitude, true);
            double result = Math.Round(Math.Max(50, distanceMarker2ToGoal2), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} distance from marker 2 to goal 2: {result}m ({distance})", track.Pilot.PilotNumber, result, distanceMarker2ToGoal2);
        }
    }

    private void Task3_CRT()
    {
        foreach (Track track in _flight.Tracks)
        {
            //TODO check distance launch to goals
            //TODO check task order
            //TODO check task timings
            //TODO calculate result
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                _logger.LogError("Error estimating launch and landing time for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            double distanceLaunchToGoal3A = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, _goalTask3A);
            if (distanceLaunchToGoal3A < 1000)
            {
                _logger.LogWarning("Pilot {pilotNumber} launch to goal 3A distance is out of bounds: {distance} meters", track.Pilot.PilotNumber, distanceLaunchToGoal3A);
            }
            double distanceLaunchToGoal3B = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, _goalTask3B);
            if (distanceLaunchToGoal3B < 1000)
            {
                _logger.LogWarning("Pilot {pilotNumber} launch to goal 3B distance is out of bounds: {distance} meters", track.Pilot.PilotNumber, distanceLaunchToGoal3B);
            }
        }
    }

    private void Task4_HWZ()
    {
        foreach (Track track in _flight.Tracks)
        {
            HesitationWaltzTask hesitationWaltzTask = new();
            hesitationWaltzTask.SetupHWZ(4, [_goalTask4A, _goalTask4B], 3, DistanceCalculationType.WithSeparationAlitude, null, Competition.Validation.ValidationStrictnessType.First);

            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                _logger.LogError("Error estimating launch and landing time for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            double distanceLaunchToGoal4A = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, _goalTask4A);
            if (distanceLaunchToGoal4A < 1000)
            {
                _logger.LogWarning("Pilot {pilotNumber} launch to goal 4A distance is out of bounds: {distance} meters", track.Pilot.PilotNumber, distanceLaunchToGoal4A);
            }
            double distanceLaunchToGoal4B = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, _goalTask4B);
            if (distanceLaunchToGoal4B < 1000)
            {
                _logger.LogWarning("Pilot {pilotNumber} launch to goal 4B distance is out of bounds: {distance} meters", track.Pilot.PilotNumber, distanceLaunchToGoal4B);
            }
            MarkerDrop? marker2 = track.GetFirstMarkerDrop(2);
            MarkerDrop? marker3 = track.GetFirstMarkerDrop(3);
            if (marker2 is not null && marker3 is not null)
            {
                if (marker2.MarkerLocation.TimeStamp > marker3.MarkerLocation.TimeStamp)
                {
                    _logger.LogWarning("Pilot {pilotNumber} dropped marker 2 after marker 3: {marker2Time} > {marker3Time}", track.Pilot.PilotNumber, marker2.MarkerLocation.TimeStamp, marker3.MarkerLocation.TimeStamp);
                }
            }
            if (marker3 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 3", track.Pilot.PilotNumber);
                continue;
            }

            if (marker3.MarkerLocation.TimeStamp > new DateTime(2026, 08, 06, 06, 45, 0))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 3 outside of allowed time: {markerTime}", track.Pilot.PilotNumber, marker3.MarkerLocation.TimeStamp);
            }
            if (!hesitationWaltzTask.CalculateResults(track, true, out double distance))
            {
                _logger.LogWarning("Pilot {pilotNumber} did not successfully calculate results for hesitation waltz task", track.Pilot.PilotNumber);
            }
            double result = Math.Round(Math.Max(50, distance), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} distance from marker 3 to goal 4: {result}m ({distance})", track.Pilot.PilotNumber, result, distance);

            //TODO check distance launch to goals
            //TODO check task order
            //TODO check task timings
            //TODO calculate result
        }
    }

    private void Task5_FON()
    {
        foreach (Track track in _flight.Tracks)
        {
            //TODO check distance launch to goals
            //TODO check task order
            //TODO check task timings
            //TODO calculate result
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                _logger.LogError("Error estimating launch and landing time for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }

            List<Declaration> declarations = [.. track.Declarations.Where(x => x.GoalNumber == 1).OrderByDescending(x => x.PositionAtDeclaration.TimeStamp)];
            Declaration? validDeclaration = null;
            foreach (Declaration declaration in declarations)
            {
                (string zone, double easting, double northing) = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM_Precise(declaration.PositionAtDeclaration);
                if (easting > 480000)
                {
                    continue;
                }
                if (declaration.OrignalEastingDeclarationUTM != 8100)
                {
                    continue;
                }
                double defaultAltitude = CoordinateHelpers.ConvertToMeter(3500);
                validDeclaration = new(declaration.GoalNumber, new Coordinate(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude, defaultAltitude, defaultAltitude, declaration.DeclaredGoal.TimeStamp), declaration.PositionAtDeclaration, true, declaration.OrignalEastingDeclarationUTM, declaration.OrignalNorhtingDeclarationUTM);
                break;
            }
            if (validDeclaration is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not have a valid declaration for task 5", track.Pilot.PilotNumber);
                continue;
            }
            double distanceLaunchToGoal5 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, validDeclaration.DeclaredGoal);
            if (distanceLaunchToGoal5 < 1000)
            {
                _logger.LogWarning("Pilot {pilotNumber} launch to goal 5 distance is out of bounds: {distance} meters", track.Pilot.PilotNumber, distanceLaunchToGoal5);
            }

            MarkerDrop? marker3 = track.GetFirstMarkerDrop(3);
            MarkerDrop? marker4 = track.GetFirstMarkerDrop(4);
            if (marker3 is not null && marker4 is not null)
            {
                if (marker3.MarkerLocation.TimeStamp > marker4.MarkerLocation.TimeStamp)
                {
                    _logger.LogWarning("Pilot {pilotNumber} dropped marker 3 after marker 4: {marker3Time} > {marker4Time}", track.Pilot.PilotNumber, marker3.MarkerLocation.TimeStamp, marker4.MarkerLocation.TimeStamp);
                }
            }
            if (marker4 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 4", track.Pilot.PilotNumber);
                continue;
            }
            if (marker4.MarkerLocation.TimeStamp > new DateTime(2026, 08, 06, 07, 00, 0))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 4 outside of allowed time: {markerTime}", track.Pilot.PilotNumber, marker4.MarkerLocation.TimeStamp);
            }
            double distanceMarker4ToGoal5 = CoordinateHelpers.Calculate3DDistance(marker4.MarkerLocation, validDeclaration.DeclaredGoal, true);
            double result = Math.Round(distanceMarker4ToGoal5, 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} distance from marker 4 to goal 5: {result}m ({distance})", track.Pilot.PilotNumber, result, distanceMarker4ToGoal5);
        }
    }

    private void Task6_ANG()
    {
        foreach (Track track in _flight.Tracks)
        {
            MarkerDrop? marker4 = track.GetFirstMarkerDrop(4);
            MarkerDrop? marker5 = track.GetFirstMarkerDrop(5);
            if (marker4 is null || marker5 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 4 or 5", track.Pilot.PilotNumber);
                continue;
            }
            if (marker4.MarkerLocation.TimeStamp > marker5.MarkerLocation.TimeStamp)
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 4 after marker 5: {marker4Time} > {marker5Time}", track.Pilot.PilotNumber, marker4.MarkerLocation.TimeStamp, marker5.MarkerLocation.TimeStamp);
            }
            double distanceMarker4ToMarker5 = CoordinateHelpers.Calculate2DDistanceHavercos(marker4.MarkerLocation, marker5.MarkerLocation);
            if (distanceMarker4ToMarker5 < 1000 || distanceMarker4ToMarker5 > 3000)
            {
                _logger.LogWarning("Pilot {pilotNumber} distance from marker 4 to marker 5 is out of bounds: {distance} meters", track.Pilot.PilotNumber, distanceMarker4ToMarker5);
            }
            if(marker5.MarkerLocation.TimeStamp > new DateTime(2026, 08, 06, 07, 00, 0))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 5 outside of allowed time: {markerTime}", track.Pilot.PilotNumber, marker5.MarkerLocation.TimeStamp);
            }

            double angle = CoordinateHelpers.CalculateInitialBearing(marker4.MarkerLocation, marker5.MarkerLocation);
            double result = Math.Round(Math.Abs(angle - 90), 2, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} angle from marker 4 to marker 5: {result} ({angle})", track.Pilot.PilotNumber, result, angle);
        }
    }

}
