using Competition;
using Competition.Penalty;
using Coordinates;
using CoordinateSharp.Formatters;
using System.Drawing.Printing;

namespace TestProgramm;

public class EuropeansWieselburg2025
{
    private readonly bool useGPSAltitude = false;
    private readonly string root_path =
        @"C:\Users\Jan\Nextcloud2\shared\Ballonveranstaltungen\2025 Wieselburg\scoring\flights";

    public void Score(bool practiseFlight, int flightNo)
    {
        Flight flight = Flight.GetInstance();
        flight.FlightNumber = flightNo;

        var flightPath = Path.Combine(root_path,
            "flight_" + (practiseFlight ? "practise_" : "") + flightNo.ToString("D2"));

        if (!flight.ParseTrackFiles(Path.Combine(root_path, Path.Combine(flightPath, @"tracks\scoring")), true))
        {
            Console.WriteLine("Failed to parse track files for " + (practiseFlight ? "trainings " : "") + "flight " +
                              flightNo);
        }

        if (!flight.MapPilotNamesToTracks(@".\PilotsMapping.csv"))
        {
            Console.WriteLine("Failed to map pilot names to tracks");
        }

        //_ = flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(1500));

        if (practiseFlight)
        {
            switch (flightNo)
            {
                case 2:
                    CheckPractiseFlight2(flight);
                    CheckPractise2FlightTask7(flight);
                    break;
            }

            return;
        }

        switch (flightNo)
        {
            case 1:

                break;
        }
    }


    #region PractiseFlight2

    private readonly int practiseFlight2Task7GoalNumber = 1;
    private void CheckPractiseFlight2(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (!TrackHelpers.CheckLaunchConstraints(track, useGPSAltitude, new DateTime(2025, 08, 16, 04, 02, 00),
                    new DateTime(2025, 08, 16, 04, 30, 00),
                    PractiseFlight2Targets().Values.ToList(), 2000, double.NaN, out Coordinate launchPoint,
                    out bool launchedInStartPeriod, out List<double> distanceToGoals,
                    out List<bool> distancesToGoalsOk))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Failed to check launch constraints");
            }
            else
            {
                if (!launchedInStartPeriod)
                {
                    Console.WriteLine($"{track.Pilot.PilotNumber}: not launched in start period. Launch point: "+ launchPoint.TimeStamp.ToString("HH:mm:ss"));
                }

                if (distancesToGoalsOk.Any())
                {
                    for (int i = 0; i < distancesToGoalsOk.Count; i++)
                    {
                        if (!distancesToGoalsOk[i])
                        {
                            Console.WriteLine(
                                $"{track.Pilot.PilotNumber}: distance violation. distance between launch and goal {i + 1} is {distanceToGoals[i].Round(2)}m");
                        }
                    }
                }
            }
        }
    }

    private void CheckPractise2FlightTask7(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            Declaration? latestDeclaration = track.GetLatestDeclaration(practiseFlight2Task7GoalNumber);

            if (latestDeclaration is null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: no declaration in #{practiseFlight2Task7GoalNumber}");
                continue;
            }

            bool successfullyChecked =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    latestDeclaration.PositionAtDeclaration, latestDeclaration.DeclaredGoal, 4500, Double.NaN,
                    out bool hasInfringement,
                    out double distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                    out double penaltyAtPositionAtDeclarationAndDeclaredGoal,
                    out double distanceBetweenPositionAtDeclarationAndDeclaredGoal);
            
            if (successfullyChecked && hasInfringement)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}:declaration has infringement. has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal.Round(2)}m between declaration point and declared goal. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
            }
        }
    }

    private Dictionary<string, Coordinate> PractiseFlight2Targets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["T4"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 504551, 5328694,
                    CoordinateHelpers.ConvertToMeter(945)),
            ["T5"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 508931, 5328679,
                    CoordinateHelpers.ConvertToMeter(873)),
            ["T6"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 517811, 5327020,
                CoordinateHelpers.ConvertToMeter(889)),
        };
    }

    #endregion
}