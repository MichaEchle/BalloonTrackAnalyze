using Competition;
using Competition.Tasks;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace DM_Tegernsee_2026;

internal class Flight5
{
    private readonly ILogger<Flight5> Logger = LogConnector.LoggerFactory.CreateLogger<Flight5>();
    internal readonly Flight _flight = Flight.GetInstance();
    internal readonly double _separationAltitude = CoordinateHelpers.ConvertToMeter(3500);
    internal Coordinate Task17_JDG = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 703958, 5293492, 769);
    internal Coordinate Task19_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 700677, 5294447, 773);
    internal Coordinate Task19_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 701201, 5293832, 786);
    internal Coordinate Task19_HWZ_C = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 700169, 5293881, 767);

    const int _minEasting = 688000;
    const int _maxEasting = 724000;
    const int _minNorthing = 5281000;
    const int _maxNorthing = 5307000;

    internal void ScoreFlight()
    {
        Logger.LogInformation("Scoring flight 5");
        _flight.FlightNumber = 5;
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\flight_05\tracks\scoring", true, null, defaultGoalAltitude: _separationAltitude))
            Console.WriteLine("Failed to parse track files");
        _flight.MapPilotNamesToTracks(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\software\DM_Tegernsee2026_Pilots.csv");
        List<Track> tracks = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber)];

        //TODO check if marker drops are in correct order
        //foreach (Track track in tracks)
        //{
        //    List<MarkerDrop> markers = [.. track.MarkerDrops.OrderBy(x => x.MarkerLocation.TimeStamp)];
        //    for (int i = 0; i < markers.Count - 1; i++)
        //    {
        //        if (markers[i].MarkerNumber > markers[i + 1].MarkerNumber)
        //        {
        //            Logger.LogWarning("Marker number '{markerNumber1}' dropped after marker number '{markerNumber2}' for Pilot {pilotNumber}", markers[i].MarkerNumber, markers[i + 1].MarkerNumber, track.Pilot.PilotNumber);
        //        }
        //    }
        //}

        Task20(tracks);
    }
    private void Task16(List<Track> tracks)
    {
        foreach (Track track in tracks)
        {
            Declaration declaration = track.GetLatestDeclaration(1);
            if (declaration is null)
            {
                Logger.LogWarning("No declaration found for goal number 1 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            //TODO check if pilot did not declared altitude
            if (!declaration.HasPilotDelaredGoalAltitude)
            {
                Logger.LogWarning("Pilot did not declare goal altitude for goal number 1 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                Logger.LogWarning("Failed to estimate launch point for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO check launch before 03:30 or after 05:00 UTC
                if (launchPoint.TimeStamp < new DateTime(2026, 5, 23, 3, 30, 0))
                {
                    Logger.LogWarning("Launch point estimated before 03:30 UTC for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                }
                if (launchPoint.TimeStamp > new DateTime(2026, 5, 23, 5, 0, 0))
                {
                    Logger.LogWarning("Launch point estimated after 05:00 UTC for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                }
                //TODO check ILP to goal <1000m
                double distanceILPToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, declaration.DeclaredGoal);
                if (distanceILPToGoal < 1000)
                {
                    Logger.LogWarning("Distance between estimated launch point and declared goal is less than 1000m for Pilot {pilotNumber}: {distance}", track.Pilot.PilotNumber, distanceILPToGoal);
                }
                //TODO check declaration before launch
                if (launchPoint.TimeStamp < declaration.PositionAtDeclaration.TimeStamp)
                {
                    Logger.LogWarning("Declaration of goal 1 is estimated after launch for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                }
            }
            //TODO check distance between declaration and goal <1000m
            double distanceDeclarationToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.PositionAtDeclaration, declaration.DeclaredGoal);
            if (distanceDeclarationToGoal < 1000)
            {
                Logger.LogWarning("Distance between declaration and goal is less than 1000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            //TODO check distance of goal 1 to Task 17 JDG <1000m
            double distanceDeclarationToTask17 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.PositionAtDeclaration, Task17_JDG);
            if (distanceDeclarationToTask17 < 1000)
            {
                Logger.LogWarning("Distance between declaration and Task 17 JDG is less than 1000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            //TODO check distance of goal 1 to Task 19 HWZ A-C <1000m
            double distanceDeclarationToTask19A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.PositionAtDeclaration, Task19_HWZ_A);
            if (distanceDeclarationToTask19A < 1000)
            {
                Logger.LogWarning("Distance between declaration and Task 19 HWZ A is less than 1000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            double distanceDeclarationToTask19B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.PositionAtDeclaration, Task19_HWZ_B);
            if (distanceDeclarationToTask19B < 1000)
            {
                Logger.LogWarning("Distance between declaration and Task 19 HWZ B is less than 1000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            double distanceDeclarationToTask19C = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.PositionAtDeclaration, Task19_HWZ_C);
            if (distanceDeclarationToTask19C < 1000)
            {
                Logger.LogWarning("Distance between declaration and Task 19 HWZ C is less than 1000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            MarkerDrop marker1 = track.MarkerDrops.Where(x => x.MarkerNumber == 1).FirstOrDefault();
            if (marker1 is null)
            {
                Logger.LogWarning("No marker drop found for marker number 1 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            //TODO check marker 1 dropped after 05:30 UTC
            if (marker1.MarkerLocation.TimeStamp > new DateTime(2026, 5, 23, 5, 30, 0))
            {
                Logger.LogWarning("Marker 1 dropped after 05:30 UTC for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            //TODO calculate 3D distance between marker 1 and goal 1
            double distanceMarkerToGoal = CoordinateHelpers.Calculate3DDistance(marker1.MarkerLocation, declaration.DeclaredGoal, true);
            Logger.LogInformation("Task 16 result for Pilot {pilotNumber}: {result}m", track.Pilot.PilotNumber, Math.Round(distanceMarkerToGoal, 0, MidpointRounding.AwayFromZero));
        }
    }

    private void Task17(List<Track> tracks)
    {
        foreach (Track track in tracks)
        {
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                Logger.LogWarning("Failed to estimate launch point for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO ILP to Task 17 JDG <1000m
                double distanceILPToTask17 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, Task17_JDG);
                if (distanceILPToTask17 < 1000)
                {
                    Logger.LogWarning("Distance between estimated launch point and Task 17 JDG is less than 1000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                }
            }
            MarkerDrop marker2 = track.MarkerDrops.Where(x => x.MarkerNumber == 2).FirstOrDefault();
            if (marker2 is null)
            {
                Logger.LogWarning("No marker drop found for marker number 2 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            //TODO check marker 2 dropped after 5:45 UTC
            if (marker2.MarkerLocation.TimeStamp > new DateTime(2026, 5, 23, 5, 45, 0))
            {
                Logger.LogWarning("Marker 2 dropped after 05:45 UTC for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            //TODO calculate distance with separation altitude between marker 2 and Task 17 JDG, best result can not be better than 50 m
            double distanceMarkerToTask17 = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(marker2.MarkerLocation, Task17_JDG, _separationAltitude, true);
            double result = Math.Max(50, distanceMarkerToTask17);
            Logger.LogInformation("Task 17 result for Pilot {pilotNumber}: {result}m ({distanceMarkerToTask17})", track.Pilot.PilotNumber, Math.Round(result, 0, MidpointRounding.AwayFromZero), distanceMarkerToTask17);
        }
    }

    private void Task18(List<Track> tracks)
    {
        foreach (var track in tracks)
        {
            Declaration declaration = track.GetLatestDeclaration(2);
            if (declaration is null)
            {
                Logger.LogWarning("No declaration found for goal number 2 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            //TODO check pilot has not declared altitude
            if (!declaration.HasPilotDelaredGoalAltitude)
            {
                Logger.LogWarning("Pilot did not declare goal altitude for goal number 2 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                Logger.LogWarning("Failed to estimate launch point for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO check ILP to goal <1000m
                double distanceILPToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, declaration.DeclaredGoal);
                if (distanceILPToGoal < 1000)
                {
                    Logger.LogWarning("Distance between estimated launch point and declared goal is less than 1000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                }
            }
            //TODO check distance between declaration and goal <1000m
            double distanceDeclarationToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.PositionAtDeclaration, declaration.DeclaredGoal);
            if (distanceDeclarationToGoal < 1000)
            {
                Logger.LogWarning("Distance between declaration and goal is less than 1000m for Pilot {pilotNumber}: {distance}", track.Pilot.PilotNumber, distanceDeclarationToGoal);
            }
            //TODO check distance of goal 2 to Task 17 JDG <1000m
            double distanceDeclarationToTask17 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, Task17_JDG);
            if (distanceDeclarationToTask17 < 1000)
            {
                Logger.LogWarning("Distance between declaration and Task 17 JDG is less than 1000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            //TODO check distance of goal 2 to Task 19 HWZ A-C <1000m
            double distanceDeclarationToTask19A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, Task19_HWZ_A);
            if (distanceDeclarationToTask19A < 1000)
            {
                Logger.LogWarning("Distance between declaration and Task 19 HWZ A is less than 1000m for Pilot {pilotNumber}: {distance}", track.Pilot.PilotNumber, distanceDeclarationToTask19A);
            }
            double distanceDeclarationToTask19B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, Task19_HWZ_B);
            if (distanceDeclarationToTask19B < 1000)
            {
                Logger.LogWarning("Distance between declaration and Task 19 HWZ B is less than 1000m for Pilot {pilotNumber}: {distance}", track.Pilot.PilotNumber, distanceDeclarationToTask19B);
            }
            double distanceDeclarationToTask19C = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, Task19_HWZ_C);
            if (distanceDeclarationToTask19C < 1000)
            {
                Logger.LogWarning("Distance between declaration and Task 19 HWZ C is less than 1000m for Pilot {pilotNumber}: {distance}", track.Pilot.PilotNumber, distanceDeclarationToTask19C);
            }
            MarkerDrop marker3 = track.MarkerDrops.Where(x => x.MarkerNumber == 3).FirstOrDefault();
            if (marker3 is null)
            {
                Logger.LogWarning("No marker drop found for marker number 3 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            //TODO check marker 3 dropped after 05:45 UTC
            if (marker3.MarkerLocation.TimeStamp > new DateTime(2026, 5, 23, 5, 45, 0))
            {
                Logger.LogWarning("Marker 3 dropped after 05:45 UTC for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            //TODO calculate 3D distance between marker 3 and goal 2
            double distanceMarkerToGoal = CoordinateHelpers.Calculate3DDistance(marker3.MarkerLocation, declaration.DeclaredGoal, true);
            Logger.LogInformation("Task 18 result for Pilot {pilotNumber}: {result}m", track.Pilot.PilotNumber, Math.Round(distanceMarkerToGoal, 0, MidpointRounding.AwayFromZero));
        }
    }

    private void Task19(List<Track> tracks)
    {
        HesitationWaltzTask task19 = new();
        task19.SetupHWZ(19, [Task19_HWZ_A, Task19_HWZ_B, Task19_HWZ_C], 4, DistanceCalculationType.WithSeparationAlitude, null, Competition.Validation.ValidationStrictnessType.First);
        task19.SeparationAltitude = _separationAltitude;
        foreach (var track in tracks)
        {
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                Logger.LogWarning("Failed to estimate launch point for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO check ILP to goals <1000m
                double distanceILPToTask19A = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, Task19_HWZ_A);
                if (distanceILPToTask19A < 1000)
                {
                    Logger.LogWarning("Distance between estimated launch point and Task 19 HWZ A is less than 1000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                }
                double distanceILPToTask19B = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, Task19_HWZ_B);
                if (distanceILPToTask19B < 1000)
                {
                    Logger.LogWarning("Distance between estimated launch point and Task 19 HWZ B is less than 1000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                }
                double distanceILPToTask19C = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, Task19_HWZ_C);
                if (distanceILPToTask19C < 1000)
                {
                    Logger.LogWarning("Distance between estimated launch point and Task 19 HWZ C is less than 1000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                }
            }
            MarkerDrop marker = track.MarkerDrops.Where(x => x.MarkerNumber == 4).FirstOrDefault();
            if (marker is null)
            {
                Logger.LogWarning("No marker drop found for marker number 4 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            //TODO check marker 4 dropped after 06:00 UTC
            if (marker.MarkerLocation.TimeStamp > new DateTime(2026, 5, 23, 6, 0, 0))
            {
                Logger.LogWarning("Marker 4 dropped after 06:00 UTC for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            //TODO calculate distance with separation altitude between marker 4 and Task 19 HWZ A-C, take the best result, but final result cannot be better than 50 m
            if (!task19.CalculateResults(track, true, out double result))
            {
                Logger.LogWarning("Failed to calculate result for Task 19 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                double finalResult = Math.Max(50, result);
                Logger.LogInformation("Task 19 result for Pilot {pilotNumber}: {finalResult}m ({result})", track.Pilot.PilotNumber, Math.Round(finalResult, 0, MidpointRounding.AwayFromZero), result);
            }
        }
    }

    private void Task20(List<Track> tracks)
    {
        foreach (var track in tracks)
        {
            MarkerDrop marker5 = track.MarkerDrops.Where(x => x.MarkerNumber == 5).FirstOrDefault();
            if (marker5 is null)
            {
                Logger.LogWarning("No marker drop found for marker number 5 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            MarkerDrop marker6 = track.MarkerDrops.Where(x => x.MarkerNumber == 6).FirstOrDefault();
            if (marker6 is null)
            {
                Logger.LogWarning("No marker drop found for marker number 6 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            (string zone, int easting, int northing) = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(marker5.MarkerLocation);
            //TODO check Marker 5 dropped with easting <699500
            if (easting < 699500)
            {
                Logger.LogWarning("Marker 5 dropped with easting less than 699500 for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            if (easting < _minEasting || easting > _maxEasting || northing < _minNorthing || northing > _maxNorthing)
            {
                Logger.LogWarning("Marker 5 dropped outside of competition area for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            //TODO check marker 6 dropped in competition area (easting between 688000 and 724000, northing between 5281000 and 5307000)
            (zone, easting, northing) = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(marker6.MarkerLocation);
            if (easting < _minEasting || easting > _maxEasting || northing < _minNorthing || northing > _maxNorthing)
            {
                Logger.LogWarning("Marker 6 dropped outside of competition area for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            //TODO check if marker 6 dropped after 06:00 UTC
            if (marker6.MarkerLocation.TimeStamp > new DateTime(2026, 5, 23, 6, 0, 0))
            {
                Logger.LogWarning("Marker 6 dropped after 06:00 UTC for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }

            //TODO check distance between marker 5 and marker 6 <3000m or >5000m
            double distanceMarker5ToMarker6 = CoordinateHelpers.Calculate2DDistanceHavercos(marker5.MarkerLocation, marker6.MarkerLocation);
            if (distanceMarker5ToMarker6 < 3000)
            {
                Logger.LogWarning("Distance between marker 5 and marker 6 is less than 3000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            if (distanceMarker5ToMarker6 > 5000)
            {
                Logger.LogWarning("Distance between marker 5 and marker 6 is greater than 5000m for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            //TODO calculate the bearing between marker 5 and marker 6, than calculate the difference to 300 degree. (if angle is less than 300, add 360 to it before calculating the difference)
            double angle = CoordinateHelpers.CalculateInitalBearing(marker5.MarkerLocation, marker6.MarkerLocation);
            double result = Math.Abs(angle - 300);
            Logger.LogInformation("Task 20 result for Pilot {pilotNumber}: {result}°", track.Pilot.PilotNumber, Math.Round(result, 2, MidpointRounding.AwayFromZero));
        }
    }
}

