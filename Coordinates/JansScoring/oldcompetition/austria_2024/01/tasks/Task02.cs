using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using System;

namespace JansScoring.flights.impl._01.tasks;

public class Task02 : TaskFON
{
    public Task02(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 02;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment );
        if (declaration == null)
        {
            return true;
        }
        MarkerChecks.LoadMarker(track, MarkerNumber(), out MarkerDrop markerDrop, ref comment );
        if (markerDrop == null)
        {
            return true;
        }
        DeclarationChecks.CheckMaxReDeclaration(track, DeclarationNumber(), 3, ref comment);
        DeclarationChecks.CheckHeightBetweenDeclarationPointAndDeclaredGoal(Flight, declaration, CoordinateHelpers.ConvertToMeter(1000), ref comment);
        DeclarationChecks.CheckIfDeclarationWasWithDelayBeforeMarkerDrop(declaration, markerDrop, 300, ref comment);
        MarkerChecks.CheckScoringPeriode(this, markerDrop, ref comment);
        return false;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024,08,14,18,00,00);
    }
    protected override int DeclarationNumber()
    {
        return 1;
    }

    protected override int MarkerNumber()
    {
        return 2;
    }
}