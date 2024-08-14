using Coordinates;
using JansScoring.check;
using System;

namespace JansScoring.flights;

public abstract class TaskFON : Task
{
    protected TaskFON(Flight flight) : base(flight)
    {
    }

    public override void Score(Track track, ref string comment, out double result)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationGoal(), out Declaration declaration, ref comment);

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

        DeclarationChecks.CheckIfDeclarationWasBeforeMarkerDrop(declaration, markerDrop, ref comment);

        result = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, markerDrop.MarkerLocation, Flight.useGPSAltitude(), Flight.getCalculationType());
    }

    protected abstract int DeclarationGoal();
    protected abstract int MarkerNumber();

    public override Coordinate[] Goals()
    {
        return Array.Empty<Coordinate>();
    }
}