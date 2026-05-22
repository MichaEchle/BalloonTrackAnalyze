using Competition;
using Competition.Tasks;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace DM_Tegernsee_2026;

internal class Flight2
{
    private readonly ILogger<Flight2> Logger = LogConnector.LoggerFactory.CreateLogger<Flight2>();
    internal readonly Flight _flight = Flight.GetInstance();
    internal readonly double _separationAltitude = CoordinateHelpers.ConvertToMeter(3500);
    internal Coordinate Task7_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 705220, 5298322, 761);
    internal Coordinate Task7_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 704871, 5298253, 768);
    internal Coordinate Task7_HWZ_C = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 705299, 5297720, 571);
    internal Coordinate Task7_HWZ_D = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 704732, 5297509, 757);
    internal Coordinate ReferenceCoordinate1 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 700000, 5200000, 750);
    internal void ScoreFlight2()
    {
        Logger.LogInformation("Scoring flight 2");
        _flight.FlightNumber = 2;
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\flight_02\tracks\scoring", true, ReferenceCoordinate1, defaultGoalAltitude: _separationAltitude))
            Console.WriteLine("Failed to parse track files");
        _flight.MapPilotNamesToTracks(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\software\DM_Tegernsee2026_Pilots.csv");
        List<Track> tracks = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber)];

        Task7(tracks);
    }

    internal void Task5(List<Track> tracks)
    {
        foreach (Track track in tracks)
        {
            Declaration latestDeclaration = track.GetLatestDeclaration(1);
            if (latestDeclaration is null)
            {
                Logger.LogWarning("No declaration 1 for pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                Logger.LogWarning("Failed to estimate launch time for pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO check declared before take off
                if (launchPoint.TimeStamp < latestDeclaration.PositionAtDeclaration.TimeStamp)
                {
                    Logger.LogWarning("Launch time is before declaration 1 for pilot {pilotNumber}", track.Pilot.PilotNumber);
                }
                //TODO check ILP to declared goal distance <1000m
                double distanceFromLaunchToDeclaredGoal = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, latestDeclaration.DeclaredGoal);
                if (distanceFromLaunchToDeclaredGoal < 1000)
                {
                    Logger.LogWarning("Distance between launch point and declared goal <1000m ({distance}) for {pilotNumber}", Math.Round(distanceFromLaunchToDeclaredGoal, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
                }
            }
            //TODO check declared with altitude
            if (!latestDeclaration.HasPilotDelaredGoalAltitude)
            {
                Logger.LogWarning("Pilot {pilotNumber} did not declare goal altitude for declaration 1", track.Pilot.PilotNumber);
            }
            //TODO check declaration position to declared goal distance <1000m
            double distanceBetweenDeclarationPositionAndDeclaredGoal = CoordinateHelpers.Calculate2DDistanceHavercos(latestDeclaration.PositionAtDeclaration, latestDeclaration.DeclaredGoal);
            if (distanceBetweenDeclarationPositionAndDeclaredGoal < 1000)
            {
                Logger.LogWarning("Distance between declaration position and declared goal <1000m ({distance}) for {pilotNumber}", Math.Round(distanceBetweenDeclarationPositionAndDeclaredGoal, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
            }
            //TODO check declared goal to HWZ distance <1000m
            double distanceT7HWZ_A = CoordinateHelpers.Calculate2DDistanceHavercos(latestDeclaration.DeclaredGoal, Task7_HWZ_A);
            if (distanceT7HWZ_A < 1000)
            {
                Logger.LogWarning("Declared goal is within 1000m ({distance}) of Task 7 HWZ A for pilot {pilotNumber}", Math.Round(distanceT7HWZ_A, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
            }
            double distanceT7HWZ_B = CoordinateHelpers.Calculate2DDistanceHavercos(latestDeclaration.DeclaredGoal, Task7_HWZ_B);
            if (distanceT7HWZ_B < 1000)
            {
                Logger.LogWarning("Declared goal is within 1000m ({distance}) of Task 7 HWZ B for pilot {pilotNumber}", Math.Round(distanceT7HWZ_B, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
            }
            double distanceT7HWZ_C = CoordinateHelpers.Calculate2DDistanceHavercos(latestDeclaration.DeclaredGoal, Task7_HWZ_C);
            if (distanceT7HWZ_C < 1000)
            {
                Logger.LogWarning("Declared goal is within 1000m ({distance}) of Task 7 HWZ C for pilot {pilotNumber}", Math.Round(distanceT7HWZ_C, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
            }
            double distanceT7HWZ_D = CoordinateHelpers.Calculate2DDistanceHavercos(latestDeclaration.DeclaredGoal, Task7_HWZ_D);
            if (distanceT7HWZ_D < 1000)
            {
                Logger.LogWarning("Declared goal is within 1000m ({distance}) of Task 7 HWZ D for pilot {pilotNumber}", Math.Round(distanceT7HWZ_D, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
            }
            //TODO check marker drop before 20:30 loc
            MarkerDrop markerDrop = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 1);
            if (markerDrop is null)
            {
                Logger.LogWarning("No marker drop 1 for pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            else
            {
                if (markerDrop.MarkerLocation.TimeStamp > new DateTime(2026, 7, 4, 18, 30, 0))
                {
                    Logger.LogWarning("Marker drop 1 is after 20:30 local time ({drop}UTC) for pilot {pilotNumber}", markerDrop.MarkerLocation.TimeStamp , track.Pilot.PilotNumber);
                }
            }
            double distanceFromMarkerDropToDeclaredGoal = CoordinateHelpers.Calculate3DDistance(markerDrop.MarkerLocation, latestDeclaration.DeclaredGoal, true);
            Logger.LogInformation("Task 5 PNo{pilotNumber} result: {result}m", track.Pilot.PilotNumber, Math.Round(distanceFromMarkerDropToDeclaredGoal, 0, MidpointRounding.AwayFromZero));
        }
    }

    internal void Task6(List<Track> tracks)
    {
        foreach (Track track in tracks)
        {
            Declaration latestDeclaration = track.GetLatestDeclaration(2);
            if (latestDeclaration is null)
            {
                Logger.LogWarning("No declaration 1 for pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                Logger.LogWarning("Failed to estimate launch time for pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO check ILP to declared goal distance <1000m
                double distanceFromLaunchToDeclaredGoal = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, latestDeclaration.DeclaredGoal);
                if (distanceFromLaunchToDeclaredGoal < 1000)
                {
                    Logger.LogWarning("Distance between launch point and declared goal <1000m ({distance}) for {pilotNumber}", Math.Round(distanceFromLaunchToDeclaredGoal, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
                }
            }
            //TODO check declared with altitude
            if (!latestDeclaration.HasPilotDelaredGoalAltitude)
            {
                Logger.LogWarning("Pilot {pilotNumber} did not declare goal altitude for declaration 2", track.Pilot.PilotNumber);
            }
            //TODO check declaration position to declared goal distance <1000m
            double distanceBetweenDeclarationPositionAndDeclaredGoal = CoordinateHelpers.Calculate2DDistanceHavercos(latestDeclaration.PositionAtDeclaration, latestDeclaration.DeclaredGoal);
            if (distanceBetweenDeclarationPositionAndDeclaredGoal < 1000)
            {
                Logger.LogWarning("Distance between declaration position and declared goal <1000m ({distance}) for {pilotNumber}", Math.Round(distanceBetweenDeclarationPositionAndDeclaredGoal, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
            }
            //TODO check declared goal Task5 distance >1000m
            Declaration t5 = track.GetLatestDeclaration(1);
            double distanceBetweenT5AndT6 = CoordinateHelpers.Calculate2DDistanceHavercos(t5.DeclaredGoal, latestDeclaration.DeclaredGoal);
            if (distanceBetweenT5AndT6 < 1000)
            {
                Logger.LogWarning("Distance between declared goal of Task 5 and Task 6 is less than 1000m ({distance}) for {pilotNumber}", Math.Round(distanceBetweenT5AndT6, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
            }
            //TODO check declared goal to HWZ distance >1000m
            double distanceT7HWZ_A = CoordinateHelpers.Calculate2DDistanceHavercos(latestDeclaration.DeclaredGoal, Task7_HWZ_A);
            if (distanceT7HWZ_A < 1000)
            {
                Logger.LogWarning("Declared goal is within 1000m ({distance}) of Task 7 HWZ A for pilot {pilotNumber}", Math.Round(distanceT7HWZ_A, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
            }
            double distanceT7HWZ_B = CoordinateHelpers.Calculate2DDistanceHavercos(latestDeclaration.DeclaredGoal, Task7_HWZ_B);
            if (distanceT7HWZ_B < 1000)
            {
                Logger.LogWarning("Declared goal is within 1000m ({distance}) of Task 7 HWZ B for pilot {pilotNumber}", Math.Round(distanceT7HWZ_B, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
            }
            double distanceT7HWZ_C = CoordinateHelpers.Calculate2DDistanceHavercos(latestDeclaration.DeclaredGoal, Task7_HWZ_C);
            if (distanceT7HWZ_C < 1000)
            {
                Logger.LogWarning("Declared goal is within 1000m ({distance}) of Task 7 HWZ C for pilot {pilotNumber}", Math.Round(distanceT7HWZ_C, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
            }
            double distanceT7HWZ_D = CoordinateHelpers.Calculate2DDistanceHavercos(latestDeclaration.DeclaredGoal, Task7_HWZ_D);
            if (distanceT7HWZ_D < 1000)
            {
                Logger.LogWarning("Declared goal is within 1000m ({distance}) of Task 7 HWZ D for pilot {pilotNumber}", Math.Round(distanceT7HWZ_D, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
            }
            //TODO check marker drop before 20:30 loc
            MarkerDrop markerDrop = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 2);
            if (markerDrop is null)
            {
                Logger.LogWarning("No marker drop 1 for pilot {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            else
            {
                if (markerDrop.MarkerLocation.TimeStamp > new DateTime(2026, 7, 4, 18, 30, 0))
                {
                    Logger.LogWarning("Marker drop 1 is after 20:30 local time ({drop}UTC) for pilot {pilotNumber}", markerDrop.MarkerLocation.TimeStamp, track.Pilot.PilotNumber);
                }
            }
            double distanceFromMarkerDropToDeclaredGoal = CoordinateHelpers.Calculate3DDistance(markerDrop.MarkerLocation, latestDeclaration.DeclaredGoal, true);
            Logger.LogInformation("Task 6 PNo{pilotNumber} result: {result}m", track.Pilot.PilotNumber, Math.Round(distanceFromMarkerDropToDeclaredGoal, 0, MidpointRounding.AwayFromZero));
        }
    }

    internal void Task7(List<Track> tracks)
    {
        HesitationWaltzTask hesitationWaltzTask = new();
        hesitationWaltzTask.SetupHWZ(7, track => [Task7_HWZ_A, Task7_HWZ_B, Task7_HWZ_C, Task7_HWZ_D], 3, DistanceCalculationType.WithSeparationAlitude,null,Competition.Validation.ValidationStrictnessType.First);
        hesitationWaltzTask.SeparationAltitude = _separationAltitude;

        foreach (Track track in tracks)
        {
            if (hesitationWaltzTask.CalculateResults(track, true, out double result))
            {
                double finalResult = Math.Max(result,50);
                Logger.LogInformation("Task 7 PNo{pilotNumber} result: {finalResult}m ({result})", track.Pilot.PilotNumber, Math.Round(finalResult, 0, MidpointRounding.AwayFromZero),result);
            }
            else
            {
                Logger.LogWarning("Failed to calculate Task 7 result for pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
        }
    }
}
