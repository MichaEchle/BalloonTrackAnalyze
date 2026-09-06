using Coordinates;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._03.tasks;

public class Task05 : TaskFON
{
    public Task05(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 5;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        List<Declaration> declarations =
            track.Declarations.FindAll(declaration => declaration.GoalNumber == DeclarationNumber());

        if (declarations.Count == 0)
        {
            comment += $"Pilot has no declaration in goal number {DeclarationNumber()} | ";
            return true;
        }

        Declaration declaration = track.GetLatestDeclaration(DeclarationNumber());
        if (declaration == null)
        {
            comment += $"No declaration found in goal number {DeclarationNumber()}. | ";
            return true;
        }

        if (Math.Abs(declaration.DeclaredGoal.AltitudeBarometric - CoordinateHelpers.ConvertToMeter(2000)) > 0.1)
        {
            comment += $"Declared altitude is not 2000ft. ({declaration.DeclaredGoal.AltitudeBarometric}) | ";
            declaration.DeclaredGoal.AltitudeBarometric = CoordinateHelpers.ConvertToMeter(2000);
        }

        var distanceBetweenDecAndDecPoint =
            CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, declaration.PositionAtDeclaration);
        if (distanceBetweenDecAndDecPoint < 3000)
        {
            var penalty =
                DistanceViolationPenalties.CalculateAndFormatPenalty(distanceBetweenDecAndDecPoint, 3000, "TP");
            comment +=
                $"Declared goal is to close to declaration point. ({distanceBetweenDecAndDecPoint}m / 3000m) [{penalty}] | ";
        }

        TrackHelpers.EstimateLaunchAndLandingTime(track, Flight.UseGPSAltitude(), out Coordinate launchpoint, out _);
        int declarationsAfterEstimated = 0;
        Declaration fistDeclarationInAir = null;
        declarations.ForEach(declaration =>
        {
            if (declaration.PositionAtDeclaration.TimeStamp > launchpoint.TimeStamp)
            {
                if (fistDeclarationInAir == null)
                    fistDeclarationInAir = declaration;
                declarationsAfterEstimated++;
            }
        });
        if (declarationsAfterEstimated > 1)
        {
            comment += "Pilot has maybe more than one declaration in the air | ";
            track.Declarations.RemoveAll(declaration1 =>
                declaration1.PositionAtDeclaration.TimeStamp > launchpoint.TimeStamp &&
                declaration1 != fistDeclarationInAir);
        }

        return false;
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 05, 16, 06, 30, 00);
    }

    protected override int DeclarationNumber()
    {
        return 1;
    }

    protected override int MarkerNumber()
    {
        return 4;
    }
}