using Coordinates;
using JansScoring.check;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._02.tasks;

public class Task03 : TaskPDG
{
    public Task03(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 3;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);

        if (declaration == null)
        {
            return true;
        }

        TrackHelpers.EstimateLaunchAndLandingTime(track, Flight.useGPSAltitude(), out Coordinate launchPoint, out _);

        Declaration startPointAsDeclaration = declaration.Clone();
        startPointAsDeclaration.PositionAtDeclaration = launchPoint;
        DeclarationChecks.CheckDistanceFromDeclarationPointToDelcaredGoal(Flight, startPointAsDeclaration, 3000,
            ref comment);
        DeclarationChecks.CheckDistanceFromDelcaredGoalToAllGoals(Flight, startPointAsDeclaration, 3000, ref comment);
        DeclarationChecks.CheckDistanceFromDelcaredGoalToOtherDeclarations(Flight, track, declaration, 3000,
            ref comment);
        DeclarationChecks.CheckIfDeclarationWasBeforeTakeOff(declaration, launchPoint.TimeStamp, ref comment);
        return false;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 15, 05, 30, 00);
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