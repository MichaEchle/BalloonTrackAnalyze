using Coordinates;
using JansScoring.flights.tasks;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._01.tasks;

public class Task01 : TaskJDG
{
    public Task01(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 1;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {

        TrackHelpers.EstimateLaunchAndLandingTime(track, Flight.UseGPSAltitude(), out Coordinate launchCoordinate,
            out Coordinate landingCoordinate);

        List<(String, Coordinate)> goals = new ();
        Coordinate[] goalList = Goals(track.Pilot.PilotNumber);
        for (int index = 1; index <= goalList.Length; index++)
        {
            Coordinate goal = goalList[index - 1];
            goals.Add((index.ToString(), goal));
        }


        List<(string identifier, double distance)> distanceBetweenLaunchPointAndGoals = TrackHelpers.Calculate2DDistanceBetweenLaunchPointAndJudeDeclaredGoals(launchCoordinate, goals);

        foreach (var distanceBetweenLaunchPointAndGoal in distanceBetweenLaunchPointAndGoals)
        {
            if (distanceBetweenLaunchPointAndGoal.distance < 1000)
            {
                string penalty = DistanceViolationPenalties.CalculateAndFormatPenalty(distanceBetweenLaunchPointAndGoal.distance, 1000, "TP");
                comment += $"Distance between launch point and goal {distanceBetweenLaunchPointAndGoal.identifier} is to short {distanceBetweenLaunchPointAndGoal.distance}m [{penalty}] | ";
            }
        }

        return false;
    }

    public override Coordinate[] Goals(int pilot)
    {
        return new[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 512993, 5355294, CoordinateHelpers.ConvertToMeter(2599))
        };
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 04, 10, 18, 00, 00);
    }


    protected override int MarkerNumber()
    {
        return 1;
    }

    protected override int MMA()
    {
        return 50;
    }


}