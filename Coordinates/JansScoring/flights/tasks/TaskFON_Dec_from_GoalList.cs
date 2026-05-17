using Coordinates;
using JansScoring.check;
using System;

namespace JansScoring.flights;

public abstract class TaskFON_Dec_from_GoalList : Task
{
    protected TaskFON_Dec_from_GoalList(Flight flight) : base(flight)
    {
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        Declaration declaration = track.Declarations.FindLast(drop => drop.GoalNumber == DeclarationNumber());

        if (declaration == null)
        {
            comment += $"Pilot has no declaration in goal number {DeclarationNumber()} | ";
            return true;
        }

        Coordinate coordinate = Goal(track, declaration);

        if (coordinate == null)
        {
            comment += "No goal found by input.";
            return true;
        }
        return false;
    }

    public override void Score(Track track, ref string comment, out double result)
    {
        Declaration declaration = track.Declarations.FindLast(drop => drop.GoalNumber == DeclarationNumber());

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


        Coordinate coordinate = Goal(track, declaration);

        if (coordinate == null)
        {
            comment += "No goal found by input.";
            result = Double.MinValue;
            return;
        }

        result = CoordinateHelpers.Calculate3DDistance(coordinate, markerDrop.MarkerLocation, Flight.UseGPSAltitude(), Flight.CalculationType());
    }

    protected abstract int DeclarationNumber();
    protected abstract int MarkerNumber();

    public abstract Coordinate Goal(Track track, Declaration declaration);

    public override Coordinate[] Goals(int pilot)
    {
        return Array.Empty<Coordinate>();
    }
}