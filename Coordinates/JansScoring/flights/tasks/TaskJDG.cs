using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using OfficeOpenXml.Drawing.Chart.ChartEx;
using System;

namespace JansScoring.flights.tasks;

public abstract class TaskJDG : Task
{
    protected TaskJDG(Flight flight) : base(flight)
    {
    }

    public override void Score(Track track, ref string comment, out double result)
    {
        if (Goals().Length <= 0)
        {
            result = Double.MinValue;
            comment = "No goal available to score.";
            return;
        }

        MarkerChecks.LoadMarker(track, MarkerNumber(), out MarkerDrop markerDrop, ref comment);
        if (markerDrop == null)
        {
            result = Double.MinValue;
            return;
        }
        MarkerChecks.CheckScoringPeriode(this, markerDrop, ref comment);

        Coordinate goal = Goals()[0];

        if (GoalChecks.Use3DScoring(Flight, markerDrop, ref comment))
        {
            Coordinate movedGoal = goal.Clone();
            movedGoal.AltitudeBarometric = Flight.getSeperationAltitudeMeters();
            movedGoal.AltitudeGPS = Flight.getSeperationAltitudeMeters();

            result = CoordinateHelpers.Calculate3DDistance(movedGoal, markerDrop.MarkerLocation,
                Flight.useGPSAltitude(),
                Flight.getCalculationType());

        }
        else
        {
            result = CalculationHelper.Calculate2DDistance(goal, markerDrop.MarkerLocation, Flight.getCalculationType());
        }
        GoalChecks.CorrectMMAResult(MMA(), ref result, ref comment);
    }

    protected abstract int MarkerNumber();
    protected abstract int MMA();

}