using Coordinates;
using JansScoring.check;
using System;

namespace JansScoring.flights.impl._03.tasks;

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
        if(declaration == null)
            return true;
        
        DeclarationChecks.CheckMaxReDeclaration(track, DeclarationNumber(), 3, ref comment);
        DeclarationChecks.CheckDistanceFromDeclarationPointToDelcaredGoal(Flight, declaration, 500, ref comment);
        DeclarationChecks.CheckDistanceFromDelcaredGoalToAllGoals(Flight, declaration, 500, ref comment);       
        DeclarationChecks.CheckDistanceFromPreviousMarker(Flight, track, declaration, 500, ref comment);

        return false;
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 09, 06, 07, 00, 00);
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