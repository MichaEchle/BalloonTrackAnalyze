using Coordinates;
using JansScoring.check;
using System;

namespace JansScoring.flights.impl._02.tasks;

public class Task09 : TaskFON
{
    public Task09(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 9;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);
        if (declaration == null)
        {
            return true;
        }
        DeclarationChecks.CheckDistanceFromDeclarationPointToDelcaredGoal(Flight, declaration, 2000, ref comment);
        DeclarationChecks.CheckMaxReDeclaration(track, DeclarationNumber(), 3, ref comment);
        DeclarationChecks.CheckIfDeclarationGoalWasAboveHeight(Flight, declaration,
            CoordinateHelpers.ConvertToMeter(1499), ref comment);
        DeclarationChecks.CheckHeightBetweenDeclarationPointAndDeclaredGoal(Flight, declaration,
            CoordinateHelpers.ConvertToMeter(1000), ref comment);
        return false;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 15, 06, 15, 00);
    }

    protected override int DeclarationNumber()
    {
        return 3;
    }

    protected override int MarkerNumber()
    {
        return 8;
    }
}