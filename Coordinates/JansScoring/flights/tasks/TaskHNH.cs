using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.tasks;

public abstract class TaskHNH : Task
{
    public TaskHNH(Flight flight) : base(flight)
    {
    }


    protected abstract int MarkerNumber();
    protected abstract int MMA();

    public override void Score(Track track, ref string comment, out double result)
    {
        MarkerChecks.LoadMarker(track, MarkerNumber(), out MarkerDrop markerDrop, ref comment);
        if (markerDrop == null)
        {
            result = Double.MinValue;
            return;
        }

        MarkerChecks.CheckScoringPeriode(this, markerDrop, ref comment);
        if (GoalChecks.Use3DScoring(Flight, markerDrop, ref comment))
        {
            Coordinate[] goals = Goals();
            GoalChecks.MoveGoalHeightToSeperationAltitude(Flight, ref goals);
            List<double> distanceToAllGoals = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation,
                goals,
                Flight.useGPSAltitude(), Flight.getCalculationType());
            result = distanceToAllGoals.Min();
        }
        else
        {
            List<double> distanceToAllGoals = CalculationHelper.calculate2DDistanceToAllGoals(markerDrop.MarkerLocation,
                Goals(),
                Flight.getCalculationType());

            result = distanceToAllGoals.Min();
        }
        GoalChecks.CorrectMMAResult(MMA(), ref result, ref comment);
    }
}