using Coordinates;
using JansScoring.check;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._03.tasks;

public class Task06 : TaskPDG
{
    public Task06(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 6;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);
        if(declaration == null)
            return true;
        
        DeclarationChecks.CheckDistanceFromDeclarationPointToDelcaredGoal(Flight, declaration, 500, ref comment);
        DeclarationChecks.CheckDistanceFromDelcaredGoalToAllGoals(Flight, declaration, 500, ref comment);
        return false;
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 09, 06, 06, 00, 00);
    }

    public override int MarkerNumber()
    {
        return 1;
    }

    public override int DeclarationNumber()
    {
        return 1;
    }
}