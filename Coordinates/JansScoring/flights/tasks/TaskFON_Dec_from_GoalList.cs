using Coordinates;
using JansScoring.check;
using System;

namespace JansScoring.flights;

public abstract class TaskFON_Dec_from_GoalList : Task
{
    protected TaskFON_Dec_from_GoalList(Flight flight) : base(flight)
    {
    }

    public override void Score(Track track, ref string comment, out double result)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);

        if (declaration == null)
        {
            result = Double.MinValue;
            return;
        }

        MarkerChecks.LoadMarker(track, MarkerNumber(), out MarkerDrop markerDrop, ref comment);
        if (markerDrop == null)
        {
            result = Double.MinValue;
            return;
        }
        MarkerChecks.CheckScoringPeriode(this, markerDrop, ref comment);


        DeclarationChecks.CheckIfDeclarationWasBeforeMarkerDrop(declaration, markerDrop, ref comment);

        result = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, markerDrop.MarkerLocation, Flight.UseGPSAltitude(), Flight.CalculationType());
    }

    protected abstract int DeclarationNumber();
    protected abstract int MarkerNumber();

    public abstract Coordinate Goal(Track track, Declaration declaration, int pilot);

    public override Coordinate[] Goals(int pilot)
    {
        return Array.Empty<Coordinate>();
    }
}