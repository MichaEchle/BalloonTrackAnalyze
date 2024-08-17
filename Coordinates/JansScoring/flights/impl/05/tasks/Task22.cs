using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using System;

namespace JansScoring.flights.impl._05.tasks;

public class Task22 : TaskFON
{
    public Task22(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 22;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);

        if (declaration == null)
        {
            return true;
        }

        DeclarationChecks.CheckMaxReDeclaration(track, DeclarationNumber(), 4, ref comment);
        DeclarationChecks.CheckHeightBetweenDeclarationPointAndDeclaredGoal(Flight, declaration,
            CoordinateHelpers.ConvertToMeter(1000), ref comment);
        DeclarationChecks.CheckIfDeclarationGoalWasAboveHeight(Flight, declaration,
            CoordinateHelpers.ConvertToMeter(1499), ref comment);
        DeclarationChecks.CheckDistanceFromDeclarationPointToDelcaredGoal(Flight, declaration, 3000, ref comment);
        return false;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 17, 06, 00, 00);
    }

    protected override int DeclarationNumber()
    {
        return 1;
    }

    protected override int MarkerNumber()
    {
        return 5;
    }
}