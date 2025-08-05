using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using System;

namespace JansScoring.flights.impl._02.tasks;

public class Task07 : TaskFON
{
    public Task07(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 07;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);
        if (declaration == null)
        {
            return true;
        }

        DeclarationChecks.CheckIfDeclarationPointWasBeforeEastGridline(Flight, declaration, 513000, ref comment);
        DeclarationChecks.CheckIfDeclarationGoalWasAboveHeight(Flight, declaration,
            CoordinateHelpers.ConvertToMeter(1499), ref comment);
        DeclarationChecks.CheckDistanceFromDeclarationPointToDelcaredGoal(Flight, declaration, 2000, ref comment);
        return false;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 15, 06, 15, 00);
    }

    protected override int DeclarationNumber()
    {
        return 2;
    }

    protected override int MarkerNumber()
    {
        return 4;
    }
}