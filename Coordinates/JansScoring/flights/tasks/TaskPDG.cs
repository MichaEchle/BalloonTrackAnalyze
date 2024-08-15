using Coordinates;
using JansScoring.check;
using System;

namespace JansScoring.flights.tasks;

public abstract class TaskPDG : Task
{
    public TaskPDG(Flight flight) : base(flight)
    {
    }

    public abstract int MarkerNumber();
    public abstract int DeclarationNumber();


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

        result = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, markerDrop.MarkerLocation, Flight.useGPSAltitude(), Flight.getCalculationType());
    }

    public override Coordinate[] Goals()
    {
        return Array.Empty<Coordinate>();
    }
}