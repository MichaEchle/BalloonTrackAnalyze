using Competition;
using Competition.Tasks;
using Competition.Validation;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace DM_Tegernsee_2026;

internal class Flight1
{
    private readonly ILogger<Flight1> Logger = LogConnector.LoggerFactory.CreateLogger<Flight1>();
    internal readonly Flight _flight = Flight.GetInstance();
    internal readonly double _separationAltitude = CoordinateHelpers.ConvertToMeter(3500);
    internal Coordinate Task2_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 702062, 5296323, 773);
    internal Coordinate Task2_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 701299, 5295943, 770);
    internal Coordinate Task2_HWZ_C = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 702711, 5300580, 720);

    internal Coordinate ReferenceCoordinate = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 700000, 5300000, 750);
    internal Coordinate ReferenceCoordinate1 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 700000, 5200000, 750);
    internal Coordinate ReferenceCoordinate2 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 600000, 5300000, 750);
    internal Coordinate ReferenceCoordinate3 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 600000, 5200000, 750);
    internal void ScoreFlight1()
    {
        Logger.LogInformation("Scoring flight 1");
        _flight.FlightNumber = 1;
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\flight_01\tracks\scoring", true, ReferenceCoordinate1, defaultGoalAltitude: _separationAltitude))
            Console.WriteLine("Failed to parse track files");
        _flight.MapPilotNamesToTracks(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\software\DM_Tegernsee2026_Pilots.csv");
        List<Track> tracks = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber)];
        Task4_FON(tracks);
    }

    internal void Task4_3D_CheckDeclaredInDonut(List<Track> tracks)
    {
        foreach (Track track in tracks)
        {
            List<Declaration> declarations = [.. track.Declarations.Where(x => x.GoalNumber == 2)];
            if (declarations.Count > 1)
            {
                for (int i = 0; i < declarations.Count - 1; i++)
                {
                    double distanceDeclarationToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(declarations[i].PositionAtDeclaration, declarations[i].DeclaredGoal);
                    if (distanceDeclarationToGoal > 20000)
                    {
                        Logger.LogInformation("Pilot {pilotNumber} wrong reference coordinate.", track.Pilot.PilotNumber);
                        continue;
                    }
                    Coordinate? firstPointInDonut = track.TrackPoints.FirstOrDefault(x => CoordinateHelpers.Calculate2DDistanceHavercos(x, declarations[i].DeclaredGoal) < 2000);
                    if (firstPointInDonut is not null)
                    {
                        if (firstPointInDonut.TimeStamp < declarations[i + 1].PositionAtDeclaration.TimeStamp)
                        {
                            Logger.LogInformation("Pilot {pilotNumber} has a track point within 2km of declared goal before the next declaration. This declaration will be used for scoring.", track.Pilot.PilotNumber);
                        }
                        else
                        {
                            Logger.LogInformation("Pilot {pilotNumber} has declared again before entering the previously declared donut",
                                track.Pilot.PilotNumber);
                        }
                    }
                    else
                    {
                        Logger.LogInformation("No point in donut found for pilot {pilotNumber}", track.Pilot.PilotNumber);
                    }


                }
            }
        }
    }

    internal void Task1_PDG(List<Track> tracks)
    {
        foreach (Track track in tracks)
        {
            Declaration declaration1 = track.GetLatestDeclaration(1);
            double distanceDeclaration1ToGoal1 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.PositionAtDeclaration, declaration1.DeclaredGoal);
            Logger.LogInformation("Distance from declaration 1 to goal 1 for pilot {track.Pilot.PilotNumber}: {distanceDeclartionToGoal}\t->\t{declarationValid}", track.Pilot.PilotNumber, distanceDeclaration1ToGoal1, distanceDeclaration1ToGoal1 >= 1000 ? "OK" : "INVALID");
            double distanceHWZ_A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.DeclaredGoal, Task2_HWZ_A);
            double distanceHWZ_B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.DeclaredGoal, Task2_HWZ_B);
            double distanceHWZ_C = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.DeclaredGoal, Task2_HWZ_C);
            Logger.LogInformation("Distance from goal 1 to HWZ A for pilot {track.Pilot.PilotNumber}: {distanceHWZ_A}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_A, distanceHWZ_A >= 1000 ? "OK" : "INVALID");
            Logger.LogInformation("Distance from goal 1 to HWZ B for pilot {track.Pilot.PilotNumber}: {distanceHWZ_B}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_B, distanceHWZ_B >= 1000 ? "OK" : "INVALID");
            Logger.LogInformation("Distance from goal 1 to HWZ C for pilot {track.Pilot.PilotNumber}: {distanceHWZ_C}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_C, distanceHWZ_C >= 1000 ? "OK" : "INVALID");

            MarkerDrop? markerDrop1 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 1);
            if (markerDrop1 is null)
            {
                Logger.LogInformation("Pilot {track.Pilot.PilotNumber} did not drop marker 1", track.Pilot.PilotNumber);
                continue;
            }
            double distanceMarkerDrop1ToGoal1 = CoordinateHelpers.Calculate3DDistance(markerDrop1.MarkerLocation, declaration1.DeclaredGoal, true);

            Logger.LogInformation("Task 1 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(distanceMarkerDrop1ToGoal1, 0, MidpointRounding.AwayFromZero));
        }
    }

    internal void Task2_HWZ(List<Track> tracks)
    {
        HesitationWaltzTask task2_HWZ = new();
        task2_HWZ.SeparationAltitude = _separationAltitude;
        List<Coordinate> goals = [];
        goals.Add(Task2_HWZ_A);
        goals.Add(Task2_HWZ_B);
        goals.Add(Task2_HWZ_C);
        task2_HWZ.SetupHWZ(2, goals, 2, DistanceCalculationType.WithSeparationAlitude, null, ValidationStrictnessType.FirstValid);

        foreach (var track in tracks)
        {
            if (!task2_HWZ.CalculateResults(track, true, out double result))
            {
                Logger.LogError("Failed to calculate task 2 result for {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            else
            {
                double finalResult = Math.Max(result, 50);
                Logger.LogInformation("Task 2 {PilotNumber}: {finalResult}[m] ({result})", track.Pilot.PilotNumber, Math.Round(finalResult, 0, MidpointRounding.AwayFromZero), result);
            }
        }
    }

    internal void Task3_3D(List<Track> tracks)
    {
        DeclarationToGoalDistanceRule declarationToGoalDistanceRule = new();
        declarationToGoalDistanceRule.SetupRule(2500, double.NaN);
        GoalToOtherGoalsDistanceRule goalToOtherGoalsDistanceRule = new();
        goalToOtherGoalsDistanceRule.SetupRule(2500, double.NaN, [1, 3]);
        DeclarationAndRule declarationAndRule = new()
        {
            ValidationRules = [declarationToGoalDistanceRule, goalToOtherGoalsDistanceRule]
        };
        DonutTask task3_Donut = new DonutTask();
        task3_Donut.SetupDonut(3, 2, 10, 1000, 2000, double.NaN, double.NaN, true, null, ValidationStrictnessType.First);

        foreach (var track in tracks)
        {
            //Declaration declaration2 = track.GetLatestDeclaration(2);
            Declaration declaration2 = track.Declarations.FirstOrDefault(x => x.GoalNumber == 2);

            double distanceDeclaration2ToGoal2 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.PositionAtDeclaration, declaration2.DeclaredGoal);
            Logger.LogInformation("Distance from declaration 2 to goal 2 for pilot {track.Pilot.PilotNumber}: {distanceDeclartionToGoal}\t->\t{declarationValid}", track.Pilot.PilotNumber, distanceDeclaration2ToGoal2, distanceDeclaration2ToGoal2 >= 2500 ? "OK" : "INVALID");
            double distanceHWZ_A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.DeclaredGoal, Task2_HWZ_A);
            double distanceHWZ_B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.DeclaredGoal, Task2_HWZ_B);
            double distanceHWZ_C = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.DeclaredGoal, Task2_HWZ_C);
            Logger.LogInformation("Distance from goal 2 to HWZ A for pilot {track.Pilot.PilotNumber}: {distanceHWZ_A}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_A, distanceHWZ_A >= 2500 ? "OK" : "INVALID");
            Logger.LogInformation("Distance from goal 2 to HWZ B for pilot {track.Pilot.PilotNumber}: {distanceHWZ_B}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_B, distanceHWZ_B >= 2500 ? "OK" : "INVALID");
            Logger.LogInformation("Distance from goal 2 to HWZ C for pilot {track.Pilot.PilotNumber}: {distanceHWZ_C}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_C, distanceHWZ_C >= 2500 ? "OK" : "INVALID");

            if (!task3_Donut.CalculateResults(track, true, out double result))
            {
                Logger.LogError("Failed to calculate task 3 result for {pilotNumber}", track.Pilot.PilotNumber);
                continue;
            }
            Logger.LogInformation("Task 3 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(result, 0, MidpointRounding.AwayFromZero));
        }
    }

    internal void Task4_FON(List<Track> tracks)
    {
        foreach (Track track in tracks)
        {
            Declaration declaration3 = track.GetLatestDeclaration(3);
            if (declaration3 is null)
            {
                Logger.LogInformation("No goal3 declared from pilot {pilotnumber}", track.Pilot.PilotNumber);
                continue;
            }
            double distanceDeclaration1ToGoal1 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration3.PositionAtDeclaration, declaration3.DeclaredGoal);
            Logger.LogInformation("Declared easting for goal 3 or pilot {pilotNumber}: {easting} \t->\t{eastingValid}", track.Pilot.PilotNumber, declaration3.OrignalEastingDeclarationUTM, declaration3.OrignalEastingDeclarationUTM == 800 ? "OK" : "INVALID");
            Logger.LogInformation("Declared northing for goal 3 or pilot {pilotNumber}: {northing} \t->\t{northingValid}", track.Pilot.PilotNumber, declaration3.OrignalNorhtingDeclarationUTM, declaration3.OrignalNorhtingDeclarationUTM > 9300 ? "OK" : "INVALID");
            Logger.LogInformation("Distance from declaration 3 to goal 3 for pilot {track.Pilot.PilotNumber}: {distanceDeclartionToGoal}\t->\t{declarationValid}", track.Pilot.PilotNumber, distanceDeclaration1ToGoal1, distanceDeclaration1ToGoal1 >= 1000 ? "OK" : "INVALID");
            double distanceHWZ_A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration3.DeclaredGoal, Task2_HWZ_A);
            double distanceHWZ_B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration3.DeclaredGoal, Task2_HWZ_B);
            double distanceHWZ_C = CoordinateHelpers.Calculate2DDistanceHavercos(declaration3.DeclaredGoal, Task2_HWZ_C);
            Logger.LogInformation("Distance from goal 3 to HWZ A for pilot {track.Pilot.PilotNumber}: {distanceHWZ_A}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_A, distanceHWZ_A >= 1000 ? "OK" : "INVALID");
            Logger.LogInformation("Distance from goal 3 to HWZ B for pilot {track.Pilot.PilotNumber}: {distanceHWZ_B}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_B, distanceHWZ_B >= 1000 ? "OK" : "INVALID");
            Logger.LogInformation("Distance from goal 3 to HWZ C for pilot {track.Pilot.PilotNumber}: {distanceHWZ_C}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_C, distanceHWZ_C >= 1000 ? "OK" : "INVALID");
            Declaration declaration2 = track.GetLatestDeclaration(2);
            if (declaration2 is not null)
            {
                double distanceGoal2ToGoal3 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.DeclaredGoal, declaration3.DeclaredGoal);
                Logger.LogInformation("Distance from goal 2 to goal 3 for pilot {track.Pilot.PilotNumber}: {distanceGoal2ToGoal3}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceGoal2ToGoal3, distanceGoal2ToGoal3 >= 2500 ? "OK" : "INVALID");
            }
            else
            {
                Logger.LogInformation("No goal2 declared from pilot {pilotnumber}", track.Pilot.PilotNumber);
            }
            MarkerDrop? markerDrop3 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 3);
            if (markerDrop3 is null)
            {
                Logger.LogInformation("Pilot {track.Pilot.PilotNumber} did not drop marker 3", track.Pilot.PilotNumber);
                continue;
            }
            double distanceMarkerDrop3ToGoal3 = CoordinateHelpers.Calculate3DDistance(markerDrop3.MarkerLocation, declaration3.DeclaredGoal, true);

            Logger.LogInformation("Task 4 PNo{PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(distanceMarkerDrop3ToGoal3, 0, MidpointRounding.AwayFromZero));
        }
    }

}

