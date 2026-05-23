using Competition;
using Competition.Tasks;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace DM_Tegernsee_2026;

internal class Flight3
{
    private readonly ILogger<Flight3> Logger = LogConnector.LoggerFactory.CreateLogger<Flight3>();
    internal readonly Flight _flight = Flight.GetInstance();
    internal readonly double _separationAltitude = CoordinateHelpers.ConvertToMeter(3500);

    //internal (string, int, int) northWest= CoordinateHelpers.ConvertLatitudeLongitudeToUTM(47.888780, 11.514940);
    //internal (string, int, int) northEast = CoordinateHelpers.ConvertLatitudeLongitudeToUTM(47.877230, 11.995900);
    //internal (string, int, int) southWest = CoordinateHelpers.ConvertLatitudeLongitudeToUTM(47.655060, 11.503690);
    //internal (string, int, int) southEast = CoordinateHelpers.ConvertLatitudeLongitudeToUTM(47.64361, 11.982500);

    internal Coordinate Task8_FIN = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 703921, 5295035, 762);
    internal Coordinate Task9_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 702064, 5296322, 773);
    internal Coordinate Task9_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 701297, 5295944, 769);

    internal Coordinate ReferenceCoordinate = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 700000, 5300000, 750);
    internal Coordinate ReferenceCoordinate1 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 700000, 5200000, 750);
    internal Coordinate ReferenceCoordinate2 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 600000, 5300000, 750);
    internal Coordinate ReferenceCoordinate3 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 600000, 5200000, 750);


    internal Dictionary<int, Coordinate> T10_ReferenceCoordinates = [];
    internal Dictionary<int, Declaration> T11_Declaration = [];
    internal void ScoreFlight()
    {
        Logger.LogInformation("Scoring flight 3");
        _flight.FlightNumber = 3;
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\flight_03\tracks\scoring", true, ReferenceCoordinate, defaultGoalAltitude: _separationAltitude))
            Console.WriteLine("Failed to parse track files");
        _flight.MapPilotNamesToTracks(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\software\DM_Tegernsee2026_Pilots.csv");
        List<Track> tracks1 = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber).Where(x=>x.Pilot.PilotNumber==19)];
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\flight_03\tracks\scoring", true, ReferenceCoordinate1, defaultGoalAltitude: _separationAltitude))
            Console.WriteLine("Failed to parse track files");
        _flight.MapPilotNamesToTracks(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\software\DM_Tegernsee2026_Pilots.csv");
        List<Track> tracks2 = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber).Where(x=>x.Pilot.PilotNumber==19)];
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\flight_03\tracks\scoring", true, ReferenceCoordinate2, defaultGoalAltitude: _separationAltitude))
            Console.WriteLine("Failed to parse track files");
        _flight.MapPilotNamesToTracks(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\software\DM_Tegernsee2026_Pilots.csv");
        List<Track> tracks3 = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber).Where(x=>x.Pilot.PilotNumber==19)];
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\flight_03\tracks\scoring", true, ReferenceCoordinate3, defaultGoalAltitude: _separationAltitude))
            Console.WriteLine("Failed to parse track files");
        _flight.MapPilotNamesToTracks(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\software\DM_Tegernsee2026_Pilots.csv");
        List<Track> tracks4 = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber).Where(x=>x.Pilot.PilotNumber==19)];
        Task10(tracks1);
        Task10(tracks2);
        Task10(tracks3);
        Task10(tracks4);

        Task11(tracks1);
        Task11(tracks2);
        Task11(tracks3);
        Task11(tracks4);

        Task12(tracks1);
        Task12(tracks2);
        Task12(tracks3);
        Task12(tracks4);

    }

    internal void Task8(List<Track> tracks)
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
                double distanceILPToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, Task8_FIN);
                if (distanceILPToGoal < 1000)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has ILP to goal <1000m: {distance}m", track.Pilot.PilotNumber, distanceILPToGoal);
                }
                //TODO launched after 05:15
                if (launchPoint.TimeStamp > new DateTime(2026, 6, 22, 5, 15, 0))
                {
                    Logger.LogWarning("Pilot {pilotNumber} has launch after 05:15 UTC: {launchTime}", track.Pilot.PilotNumber, launchPoint.TimeStamp);
                }
            }
            MarkerDrop marker1 = track.MarkerDrops.FirstOrDefault(md => md.MarkerNumber == 1);
            if (marker1 is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no marker drop 1", track.Pilot.PilotNumber);
                continue;
            }
            //TODO check marker drop1 after 06:15
            if (marker1.MarkerLocation.TimeStamp > new DateTime(2026, 6, 22, 6, 15, 0))
            {
                Logger.LogWarning("Pilot {pilotNumber} has marker drop 1 after 06:15 UTC: {markerDropTime}", track.Pilot.PilotNumber, marker1.MarkerLocation.TimeStamp);
            }
            //TODO calculate marker 1 to goal with separation alt, best result is 50m
            double distanceMarker1ToGoal = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(Task8_FIN, marker1.MarkerLocation, _separationAltitude, true);

            double result = Math.Max(50, distanceMarker1ToGoal);
            Logger.LogInformation("Task 8 result for Pilot {pilotNumber}: {result}m ({distanceMarker1ToGoal}m)", track.Pilot.PilotNumber, Math.Round(result, 0, MidpointRounding.AwayFromZero), distanceMarker1ToGoal);
        }

    }
    internal void Task9(List<Track> tracks)
    {
        HesitationWaltzTask task9 = new();
        task9.SetupHWZ(9, [Task9_HWZ_A, Task9_HWZ_B], 2, DistanceCalculationType.WithSeparationAlitude, null, Competition.Validation.ValidationStrictnessType.First);
        task9.SeparationAltitude = _separationAltitude;
        foreach (Track track in tracks)
        {
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                Logger.LogWarning("Failed to estimate launch point for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO check ILP to goal <1000
                double distanceILPToGoalA = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, Task9_HWZ_A);
                if (distanceILPToGoalA < 1000)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has ILP to goal A <1000m: {distance}m", track.Pilot.PilotNumber, distanceILPToGoalA);
                }

                double distanceILPToGoalB = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, Task9_HWZ_B);
                if (distanceILPToGoalB < 1000)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has ILP to goal B <1000m: {distance}m", track.Pilot.PilotNumber, distanceILPToGoalB);
                }
            }
            MarkerDrop marker2 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 2);
            if (marker2 is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no marker drop 2", track.Pilot.PilotNumber);
                continue;
            }
            //TODO check marker drop2 before 06:30
            if (marker2.MarkerLocation.TimeStamp > new DateTime(2026, 6, 22, 6, 30, 0))
            {
                Logger.LogWarning("Pilot {pilotNumber} has marker drop 2 after 06:30 UTC: {markerDropTime}", track.Pilot.PilotNumber, marker2.MarkerLocation.TimeStamp);
            }
            //TODO calculate marker 2 to goals with separation alt, take better result, but best result is 50m
            if (!task9.CalculateResults(track, true, out double result))
            {
                Logger.LogWarning("Failed to calculate Task 9 result for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                double finalResult = Math.Max(50, result);
                Logger.LogInformation("Task 9 result for Pilot {pilotNumber}: {finalResult}m ({result)}", track.Pilot.PilotNumber, Math.Round(finalResult, 0, MidpointRounding.AwayFromZero), result);
            }
        }
    }
    internal void Task10(List<Track> tracks)
    {
        foreach (var track in tracks)
        {
            //TODO take first declaration
            Declaration declaration = track.Declarations.OrderBy(x => x.PositionAtDeclaration.TimeStamp).FirstOrDefault(x => x.GoalNumber == 1);
            if (declaration is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no declared goal 1", track.Pilot.PilotNumber);
                continue;
            }
            MarkerDrop marker2 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 2);
            if (marker2 is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no marker drop 2", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO check declaration before marker drop 2
                if (declaration.PositionAtDeclaration.TimeStamp < marker2.MarkerLocation.TimeStamp)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has declaration of goal 2 before marker drop 2: {declarationTime} < {markerDropTime}", track.Pilot.PilotNumber, declaration.PositionAtDeclaration.TimeStamp, marker2.MarkerLocation.TimeStamp);
                }
            }
            //TODO calculate 2D distance between first track point 15min after timestamp of position at declaration and declared goal
            Coordinate coordiante = track.TrackPoints.FirstOrDefault(x => x.TimeStamp >= declaration.PositionAtDeclaration.TimeStamp.AddMinutes(15));
            if (coordiante is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no track point 15min after declaration of goal 2", track.Pilot.PilotNumber);
                continue;
            }
            if (!T10_ReferenceCoordinates.ContainsKey(track.Pilot.PilotNumber))
            {
                T10_ReferenceCoordinates.Add(track.Pilot.PilotNumber, coordiante);
            }
            else
            {
                double distanceReferenceToPreviousReference = CoordinateHelpers.Calculate2DDistanceHavercos(coordiante, T10_ReferenceCoordinates[track.Pilot.PilotNumber]);
                double distanceReferenceToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(coordiante, declaration.DeclaredGoal);
                if (distanceReferenceToGoal < distanceReferenceToPreviousReference)
                {
                    T10_ReferenceCoordinates[track.Pilot.PilotNumber] = coordiante;
                }
                Logger.LogInformation("Task 10 Pilot {pilotNumber}: {distance}m", track.Pilot.PilotNumber, Math.Round(distanceReferenceToGoal, 0, MidpointRounding.AwayFromZero));
            }
        }
    }

    internal void Task11(List<Track> tracks)
    {
        PieTask pieTask = new();
        PieTask.PieTier tier0 = new();
        tier0.SetupPieTier(2, 2000, true, 1.0, 0, CoordinateHelpers.ConvertToMeter(3000), null, Competition.Validation.ValidationStrictnessType.First);
        PieTask.PieTier tier1 = new();
        tier1.SetupPieTier(2, 1500, true, 2.0, CoordinateHelpers.ConvertToMeter(3000), CoordinateHelpers.ConvertToMeter(3500), null, Competition.Validation.ValidationStrictnessType.First);
        PieTask.PieTier tier2 = new();
        tier2.SetupPieTier(2, 500, true, 3.0, CoordinateHelpers.ConvertToMeter(3500), CoordinateHelpers.ConvertToMeter(4000), null, Competition.Validation.ValidationStrictnessType.First);
        pieTask.SetupPie(11, [tier0, tier1, tier2]);
        foreach (var track in tracks)
        {
            List<Declaration> declarations = [.. track.Declarations.Where(x => x.GoalNumber == 2).OrderBy(x => x.PositionAtDeclaration.TimeStamp)];
            Declaration declaration = null;
            if (declarations.Count == 0)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no declared goal 2", track.Pilot.PilotNumber);
                continue;
            }
            if (declarations.Count == 1)
            {
                declaration = declarations[0];
            }
            else
            {
                for (int index = 0; index < declarations.Count - 1; index++)
                {
                    List<Coordinate> tier0Coordinates = [.. track.TrackPoints.Where(x => x.AltitudeGPS < CoordinateHelpers.ConvertToMeter(3000))];
                    List<Coordinate> tier1Coordinates = [.. track.TrackPoints.Where(x => x.AltitudeGPS >= CoordinateHelpers.ConvertToMeter(3000) && x.AltitudeGPS < CoordinateHelpers.ConvertToMeter(3500))];
                    List<Coordinate> tier2Coordinates = [.. track.TrackPoints.Where(x => x.AltitudeGPS >= CoordinateHelpers.ConvertToMeter(3500) && x.AltitudeGPS < CoordinateHelpers.ConvertToMeter(4000))];

                    List<DateTime> firstPoints = [];
                    Coordinate firstInTier0 = tier0Coordinates.FirstOrDefault(x => CoordinateHelpers.Calculate2DDistanceHavercos(x, declarations[index].DeclaredGoal) < 2000);
                    firstPoints.Add(firstInTier0?.TimeStamp ?? DateTime.MaxValue);
                    Coordinate firstInTier1 = tier1Coordinates.FirstOrDefault(x => CoordinateHelpers.Calculate2DDistanceHavercos(x, declarations[index].DeclaredGoal) < 1500);
                    firstPoints.Add(firstInTier1?.TimeStamp ?? DateTime.MaxValue);
                    Coordinate firstInTier2 = tier2Coordinates.FirstOrDefault(x => CoordinateHelpers.Calculate2DDistanceHavercos(x, declarations[index].DeclaredGoal) < 500);
                    firstPoints.Add(firstInTier2?.TimeStamp ?? DateTime.MaxValue);

                    DateTime firstPointInCake = firstPoints.Min();

                    //TODO take declaration with first trackpoint within cake, compare timing of first trackpoint in cake with time of next declaration
                    if (firstPointInCake < declarations[index + 1].PositionAtDeclaration.TimeStamp)
                    {
                        declaration = declarations[index];
                        break;
                    }
                }
                if (declaration is null)
                {
                    declaration = declarations.Last();
                }
            }
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                Logger.LogWarning("Failed to estimate launch point for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO check ILP to goal <1000
                double distanceILPToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, declaration.DeclaredGoal);
                if (distanceILPToGoal < 1000)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has ILP to goal <1000m: {distance}m", track.Pilot.PilotNumber, distanceILPToGoal);
                }
            }
            //TODO check distance between declaration and declared goal <2500
            double distanceDeclarationToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.PositionAtDeclaration, declaration.DeclaredGoal);
            if (distanceDeclarationToGoal < 2500)
            {
                Logger.LogWarning("Pilot {pilotNumber} has distance between declaration and declared goal <2500m: {distance}m", track.Pilot.PilotNumber, distanceDeclarationToGoal);
            }
            Track trackToBeUsed = new();
            trackToBeUsed.Declarations.Add(declaration);
            trackToBeUsed.Pilot = track.Pilot;
            //TODO only consider trackpoints after declared goal 2
            List<Coordinate> trackPoints = [.. track.TrackPoints.Where(x => x.TimeStamp >= declaration.PositionAtDeclaration.TimeStamp 
            //TODO only consider trackpoints till 07:30
            && x.TimeStamp <= new DateTime(2026, 05, 22, 07, 30, 00))];
            if (T10_ReferenceCoordinates.TryGetValue(track.Pilot.PilotNumber, out Coordinate referenceCoordiante))
            {
                //TODO only consider trackpoints after reference trackpoint from T10
                trackPoints = [.. trackPoints.Where(x => x.TimeStamp >= referenceCoordiante.TimeStamp)];
            }
            trackToBeUsed.TrackPoints.AddRange(trackPoints);
            //TODO calculate weighted 2D distance in cake
            if (!pieTask.CalculateResults(trackToBeUsed, true, out double result))
            {
                Logger.LogWarning("Failed to calculate Task 11 result for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {

                if (!T11_Declaration.ContainsKey(track.Pilot.PilotNumber))
                {
                    if (result > 0)
                    {
                        T11_Declaration.Add(track.Pilot.PilotNumber, declaration);
                    }
                }

                Logger.LogInformation("Task 11 result for Pilot {pilotNumber}: {result}m", track.Pilot.PilotNumber, Math.Round(result, 0, MidpointRounding.AwayFromZero));
            }
        }
    }

    internal void Task12(List<Track> tracks)
    {
        foreach (var track in tracks)
        {
            //            Declaration declaration = track.GetLatestDeclaration(3);
            Declaration declaration = track.Declarations.OrderBy(x => x.PositionAtDeclaration.TimeStamp).Where(x => x.GoalNumber == 3).Skip(2).FirstOrDefault();
            if (declaration is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no declaration for goal 3", track.Pilot.PilotNumber);
                continue;
            }
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                Logger.LogWarning("Failed to estimate launch point for Pilot {pilotNumber}", track.Pilot.PilotNumber);
            }
            else
            {
                //TODO check ILP to goal <1000
                double distanceILPToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, declaration.DeclaredGoal);
                if (distanceILPToGoal < 1000)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has ILP to goal <1000m: {distance}m", track.Pilot.PilotNumber, distanceILPToGoal);
                }
            }
            //TODO check distance between declaration and declared goal <1000
            double distanceDeclarationToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.PositionAtDeclaration, declaration.DeclaredGoal);
            if (distanceDeclarationToGoal < 1000)
            {
                Logger.LogWarning("Pilot {pilotNumber} has distance between declaration and declared goal <1000m: {distance}m", track.Pilot.PilotNumber, distanceDeclarationToGoal);
            }

            //TODO check distance between declared goal and goal T8 <1000
            double distanceGoalToTask8 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, Task8_FIN);
            if (distanceGoalToTask8 < 1000)
            {
                Logger.LogWarning("Pilot {pilotNumber} has distance between declared goal and Task 8 goal <1000m: {distance}m", track.Pilot.PilotNumber, distanceGoalToTask8);
            }
            //TODO check distance between declared goal and goal T9 <1000
            double distanceGoalToTask9A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, Task9_HWZ_A);
            if (distanceGoalToTask9A < 1000)
            {
                Logger.LogWarning("Pilot {pilotNumber} has distance between declared goal and Task 9 goal A <1000m: {distance}m", track.Pilot.PilotNumber, distanceGoalToTask9A);
            }
            double distanceGoalToTask9B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, Task9_HWZ_B);
            if (distanceGoalToTask9B < 1000)
            {
                Logger.LogWarning("Pilot {pilotNumber} has distance between declared goal and Task 9 goal B <1000m: {distance}m", track.Pilot.PilotNumber, distanceGoalToTask9B);
            }

            //check if delcared goal in contest area
            (string zone, int easting, int northing) = CoordinateHelpers.ConvertLatitudeLongitudeToUTM(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude);
            if (easting < 688000)
            {
                Logger.LogWarning("Pilot {pilotNumber} has declared goal with easting <688000: {easting}", track.Pilot.PilotNumber, easting);
            }
            if(easting>724000)
            {
                Logger.LogWarning("Pilot {pilotNumber} has declared goal with easting >724000: {easting}", track.Pilot.PilotNumber, easting);
            }
            if (northing < 5281000)
            {
                Logger.LogWarning("Pilot {pilotNumber} has declared goal with northing <5281000: {northing}", track.Pilot.PilotNumber, northing);
            }
            if (northing > 5307000)
            {
                Logger.LogWarning("Pilot {pilotNumber} has declared goal with northing >5307000: {northing}", track.Pilot.PilotNumber, northing);
            }
            //TODO check distance between declared goal and goal T10 <1000
            Declaration declarationT10 = track.Declarations.OrderBy(x => x.PositionAtDeclaration.TimeStamp).FirstOrDefault(x => x.GoalNumber == 2);
            if (declaration is not null)
            {
                double distanceGoalToTask10 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, declarationT10.DeclaredGoal);
                if (distanceGoalToTask10 < 1000)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has distance between declared goal and Task 10 declared goal <1000m: {distance}m", track.Pilot.PilotNumber, distanceGoalToTask10);
                }
            }
            //TODO check distance between declared goal and goal T11 <1000
            if (T11_Declaration.TryGetValue(track.Pilot.PilotNumber, out Declaration declarationT11))
            {
                double distanceGoalToTask11 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, declarationT11.DeclaredGoal);
                if (distanceGoalToTask11 < 1000)
                {
                    Logger.LogWarning("Pilot {pilotNumber} has distance between declared goal and Task 11 declared goal <1000m: {distance}m", track.Pilot.PilotNumber, distanceGoalToTask11);
                }
                //TODO check declared goal 3 not in cake
                if (declaration.DeclaredGoal.AltitudeGPS < CoordinateHelpers.ConvertToMeter(3000))
                {
                    if (distanceGoalToTask11 < 2000)
                    {
                        Logger.LogWarning("Pilot {pilotNumber} has distance between declared goal and Task 11 declared goal <2000m in tier 0: {distance}m", track.Pilot.PilotNumber, distanceGoalToTask11);
                    }
                }
                else if (declaration.DeclaredGoal.AltitudeGPS >= CoordinateHelpers.ConvertToMeter(3000) && declaration.DeclaredGoal.AltitudeGPS < CoordinateHelpers.ConvertToMeter(3500))
                {
                    if (distanceGoalToTask11 < 1500)
                    {
                        Logger.LogWarning("Pilot {pilotNumber} has distance between declared goal and Task 11 declared goal <1500m in tier 1: {distance}m", track.Pilot.PilotNumber, distanceGoalToTask11);
                    }
                }
                else if (declaration.DeclaredGoal.AltitudeGPS >= CoordinateHelpers.ConvertToMeter(3500) && declaration.DeclaredGoal.AltitudeGPS < CoordinateHelpers.ConvertToMeter(4000))
                {
                    if (distanceGoalToTask11 < 500)
                    {
                        Logger.LogWarning("Pilot {pilotNumber} has distance between declared goal and Task 11 declared goal <500m in tier 2: {distance}m", track.Pilot.PilotNumber, distanceGoalToTask11);
                    }
                }
            }
            MarkerDrop marker3 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 3);
            if (marker3 is null)
            {
                Logger.LogWarning("Pilot {pilotNumber} has no marker drop 3", track.Pilot.PilotNumber);
                continue;
            }
            //TODO check marker drop 3 after 07:45
            if (marker3.MarkerLocation.TimeStamp > new DateTime(2026, 05, 22, 07, 45, 00))
            {
                Logger.LogWarning("Pilot {pilotNumber} has marker drop 3 after 07:45 UTC: {markerDropTime}", track.Pilot.PilotNumber, marker3.MarkerLocation.TimeStamp);
            }
            //TODO calculate 3D distance between declared goal 3 and marker 3
            double distanceDeclarationToMarker = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, marker3.MarkerLocation, true);

            Logger.LogInformation("Task 12 result for Pilot {pilotNumber}: {distance}m", track.Pilot.PilotNumber, Math.Round(distanceDeclarationToMarker, 0, MidpointRounding.AwayFromZero));
        }
    }

}
