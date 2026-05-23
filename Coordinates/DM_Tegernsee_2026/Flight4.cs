using Competition;
using Competition.Tasks;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace DM_Tegernsee_2026;

internal class Flight4
{
    private readonly ILogger<Flight4> Logger = LogConnector.LoggerFactory.CreateLogger<Flight4>();
    internal readonly Flight _flight = Flight.GetInstance();
    internal readonly double _separationAltitude = CoordinateHelpers.ConvertToMeter(3500);
    internal Coordinate Task13_FIN = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 704269, 5296594, 770);
    internal Coordinate Task14_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 702063, 5296326, 762);
    internal Coordinate Task14_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 702454, 5295028, 770);


    internal void ScoreFlight()
    {
        Logger.LogInformation("Scoring flight 4");
        _flight.FlightNumber = 4;
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\flight_04\tracks\scoring", true, null, defaultGoalAltitude: _separationAltitude))
            Console.WriteLine("Failed to parse track files");
        _flight.MapPilotNamesToTracks(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\software\DM_Tegernsee2026_Pilots.csv");
        List<Track> tracks = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber)];

        Task15(tracks);
    }

    private void Task13(List<Track> tracks)
    {
        foreach (Track track in tracks)
        {
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                Logger.LogWarning("Failed to estimate launch point for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO check ILP to goal <1000
                double distanceILPToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, Task13_FIN);
                if (distanceILPToGoal < 1000)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has ILP to goal <1000m: {distance}m", track.Pilot.PilotNumber, distanceILPToGoal);
                }
                //TODO check launch before 17 or after 18 UTC
                if (launchPoint.TimeStamp < new DateTime(2026, 5, 22, 17, 00, 0))
                {
                    Logger.LogWarning("Pilot {pilotNumber} has launch before 17:00 UTC: {launchTime}", track.Pilot.PilotNumber, launchPoint.TimeStamp);
                }

                if (launchPoint.TimeStamp > new DateTime(2026, 5, 22, 18, 00, 0))
                {
                    Logger.LogWarning("Pilot {pilotNumber} has launch after 18:00 UTC: {launchTime}", track.Pilot.PilotNumber, launchPoint.TimeStamp);
                }
            }
            MarkerDrop marker1 = track.MarkerDrops.FirstOrDefault(md => md.MarkerNumber == 1);
            if (marker1 is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no marker drop 1", track.Pilot.PilotNumber);
                continue;
            }
            //TODO check marker 1 drop after 18:30
            if (marker1.MarkerLocation.TimeStamp > new DateTime(2026, 5, 22, 18, 30, 0))
            {
                Logger.LogWarning("Pilot {pilotNumber} has marker drop 1 after 18:30 UTC: {markerDropTime}", track.Pilot.PilotNumber, marker1.MarkerLocation.TimeStamp);
            }
            //TODO calculate distance with separation altitude between goal and marker drop 1, best result can be 50m
            double distanceMarker1ToGoal = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(Task13_FIN, marker1.MarkerLocation, _separationAltitude, true);

            double result = Math.Max(50, distanceMarker1ToGoal);
            Logger.LogInformation("Task 13 result for Pilot {pilotNumber}: {result}m ({distanceMarker1ToGoal}m)", track.Pilot.PilotNumber, Math.Round(result, 0, MidpointRounding.AwayFromZero), distanceMarker1ToGoal);
        }
    }

    private void Task14(List<Track> tracks)
    {
        HesitationWaltzTask task14 = new();
        task14.SetupHWZ(14, [Task14_HWZ_A, Task14_HWZ_B], 2, DistanceCalculationType.WithSeparationAlitude, null, Competition.Validation.ValidationStrictnessType.First);
        task14.SeparationAltitude = _separationAltitude;
        foreach (Track track in tracks)
        {
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                Logger.LogWarning("Failed to estimate launch point for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO check ILP to goal <1000
                double distanceILPToGoalA = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, Task14_HWZ_B);
                if (distanceILPToGoalA < 1000)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has ILP to goal A <1000m: {distance}m", track.Pilot.PilotNumber, distanceILPToGoalA);
                }
                double distanceILPToGoalB = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, Task14_HWZ_B);
                if (distanceILPToGoalB < 1000)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has ILP to goal B <1000m: {distance}m", track.Pilot.PilotNumber, distanceILPToGoalB);
                }
            }
            MarkerDrop markerDrop1 = track.MarkerDrops.FirstOrDefault(md => md.MarkerNumber == 1);
            MarkerDrop markerDrop2 = track.MarkerDrops.FirstOrDefault(md => md.MarkerNumber == 2);
            if (markerDrop2 is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no marker drop 2", track.Pilot.PilotNumber);
                continue;
            }
            if (markerDrop1 != null && markerDrop2 != null)
            {
                //TODO check marker drop 1 after marker drop 2
                if (markerDrop2.MarkerLocation.TimeStamp < markerDrop1.MarkerLocation.TimeStamp)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has marker drop 1 after marker drop 2: {markerDrop1Time} vs {markerDrop2Time}", track.Pilot.PilotNumber, markerDrop1.MarkerLocation.TimeStamp, markerDrop2.MarkerLocation.TimeStamp);
                }
            }
            //TODO check marker drop 2  after 18:30
            if (markerDrop2.MarkerLocation.TimeStamp > new DateTime(2026, 5, 22, 18, 30, 0))
            {
                Logger.LogWarning("Pilot {pilotNumber} has marker drop 2 after 18:30 UTC: {markerDropTime}", track.Pilot.PilotNumber, markerDrop2.MarkerLocation.TimeStamp);
            }
            //TODO calculate distance with separation altitude between goals and marker drop 2,take smallest result, best result can be 50m
            if (task14.CalculateResults(track, true, out double result))
            {
                double finalResult = Math.Max(result, 50);
                Logger.LogInformation("Task 14 PNo{pilotNumber} result: {finalResult}m ({result})", track.Pilot.PilotNumber, Math.Round(finalResult, 0, MidpointRounding.AwayFromZero), result);
            }
            else
            {
                Logger.LogWarning("Failed to calculate Task 14 result for pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
        }
    }

    private void Task15(List<Track> tracks)
    {
        foreach (var track in tracks)
        {
            MarkerDrop marker2 = track.MarkerDrops.FirstOrDefault(md => md.MarkerNumber == 2);
            if (marker2 is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no marker drop 2", track.Pilot.PilotNumber);
                continue;
            }
            MarkerDrop marker3 = track.MarkerDrops.FirstOrDefault(md => md.MarkerNumber == 3);
            if (marker3 is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no marker drop 3", track.Pilot.PilotNumber);
                continue;
            }
            //TODO check marker drop 4 exists -> idiot
            MarkerDrop marker4 = track.MarkerDrops.FirstOrDefault(md => md.MarkerNumber == 4);
            if (marker4 is not null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has marker drop 4 (IDIOT.....)", track.Pilot.PilotNumber);
            }
            //TODO check marker drop 2 after marker drop 3
            if (marker3.MarkerLocation.TimeStamp < marker2.MarkerLocation.TimeStamp)
            {
                Logger.LogWarning("Pilot {pilotNumber} has marker drop 2 after marker drop 3: {markerDrop2Time} vs {markerDrop3Time}", track.Pilot.PilotNumber, marker2.MarkerLocation.TimeStamp, marker3.MarkerLocation.TimeStamp);
            }
            //TODO check marker drop 3 after 18:30
            if (marker3.MarkerLocation.TimeStamp > new DateTime(2026, 5, 22, 18, 30, 0))
            {
                Logger.LogWarning("Pilot {pilotNumber} has marker drop 3 after 18:30 UTC: {markerDropTime}", track.Pilot.PilotNumber, marker3.MarkerLocation.TimeStamp);
            }
            //TODO calculate area using marker 2, marker 3 and first track point 15 min after marker drop 2 (use calculate area, not landrun task)
            Coordinate trackPoints = track.TrackPoints.FirstOrDefault(x => x.TimeStamp >= marker2.MarkerLocation.TimeStamp.AddMinutes(15));
            if (trackPoints is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no track point 15min after marker drop 2", track.Pilot.PilotNumber);
            }
            else
            {
                double result = CoordinateHelpers.CalculateArea(marker2.MarkerLocation, marker3.MarkerLocation, marker4.MarkerLocation);
                Logger.LogInformation("Task 15 Pilot {pilotNumber}: {result}km²", track.Pilot.PilotNumber, Math.Round(result / 1.0e6, 3, MidpointRounding.AwayFromZero));
            }

        }
    }
}
