using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.tasks;

public abstract class TaskHWZ : Task
{
    protected TaskHWZ(Flight flight) : base(flight)
    {
    }

    public override void Score(Track track, ref string comment, out double result)
    {
        if (Goals(track.Pilot.PilotNumber).Length <= 0)
        {
            result = Double.MinValue;
            comment = "No goal available to score.";
            return;
        }
        MarkerChecks.LoadMarker(track, MarkerNumber(), out MarkerDrop markerDrop, ref comment );
        if (markerDrop == null)
        {
            result = Double.MinValue;
            return;
        }
        MarkerChecks.CheckScoringPeriode(this, markerDrop, ref comment);

        if (GoalChecks.Use3DScoring(Flight, markerDrop, ref comment))
        {
            List<Coordinate> goals = new();
            foreach (Coordinate coordinate in Goals(track.Pilot.PilotNumber))
            {
                Coordinate goal = coordinate.Clone();
                goal.AltitudeBarometric = Flight.getSeperationAltitudeMeters();
                goal.AltitudeGPS = Flight.getSeperationAltitudeMeters();
                goals.Add(goal);
            }

            List<double> distanceToAllGoals = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation, goals.ToArray(), Flight.useGPSAltitude(),
                Flight.getCalculationType());

            result = distanceToAllGoals.Min();
        }
        else
        {
            result = CalculationHelper.calculate2DDistanceToAllGoals(markerDrop.MarkerLocation, Goals(track.Pilot.PilotNumber), Flight.getCalculationType()).Min();
        }

        GoalChecks.CorrectMMAResult(MMA(), ref result, ref comment);

    }

    protected abstract int MarkerNumber();
    protected abstract int MMA();

}