using Competition;
using Coordinates;
using JansScoring.check;
using System;

namespace JansScoring.flights.impl._02.tasks;

public class Task03 : TaskFON
{
    public Task03(Flight flight) : base(flight)
    {

    }

    public override int TaskNumber()
    {
        return 03;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);

        if (declaration == null)
        {
            return true;
        }
        
        MarkerChecks.LoadMarker(track, MarkerNumber(), out MarkerDrop markerDrop, ref comment);
        if (markerDrop == null)
        {
            return true;
        }
        
        DeclarationChecks.CheckDistanceFromDeclarationPointToDelcaredGoal(Flight, declaration, 1000, ref comment);
        DeclarationChecks.CheckDistanceFromDelcaredGoalToAllGoals(Flight, declaration, 1000, ref comment);
        DeclarationChecks.CheckDistanceFromPreviousMarker(Flight, track, declaration, 1000, ref comment);
        
        return false;
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 09, 05, 06, 30, 00);
    }

    protected override int DeclarationNumber()
    {
        return 1;
    }

    protected override int MarkerNumber()
    {
        return 3;
    }
}