using Coordinates;
using JansScoring.flights.tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;

namespace JansScoring.flights.impl._01.tasks;

public class Task02 : TaskHWZ
{
    public Task02(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 2;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        TrackHelpers.EstimateLaunchAndLandingTime(track, Flight.UseGPSAltitude(), out Coordinate launchCoordinate,
            out Coordinate landingCoordinate);

        List<(String, Coordinate)> goals = new();
        Coordinate[] goalList = Goals(track.Pilot.PilotNumber);
        for (int index = 1; index <= goalList.Length; index++)
        {
            Coordinate goal = goalList[index - 1];
            goals.Add((index.ToString(), goal));
        }

        List<(string identifier, double distance)> distanceBetweenLaunchPointAndGoals =
            TrackHelpers.Calculate2DDistanceBetweenLaunchPointAndJudeDeclaredGoals(launchCoordinate, goals);

        foreach (var distanceBetweenLaunchPointAndGoal in distanceBetweenLaunchPointAndGoals)
        {
            if (distanceBetweenLaunchPointAndGoal.distance < 1000)
            {
                string penalty =
                    DistanceViolationPenalties.CalculateAndFormatPenalty(distanceBetweenLaunchPointAndGoal.distance,
                        1000, "TP");
                comment +=
                    $"Distance between launch point and goal {distanceBetweenLaunchPointAndGoal.identifier} is to short {distanceBetweenLaunchPointAndGoal.distance}m [{penalty}] | ";
            }
        }

        return false;
    }

    public override Coordinate[] Goals(int pilot)
    {
        return new[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 515361, 5355172, CoordinateHelpers.ConvertToMeter(2589)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 514504, 5354049, CoordinateHelpers.ConvertToMeter(2550)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 515971, 5353901, CoordinateHelpers.ConvertToMeter(2523)),
        };
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 04, 10, 18, 00, 00);
    }


    protected override int MarkerNumber()
    {
        return 2;
    }

    protected override int MMA()
    {
        return 50;
    }
}