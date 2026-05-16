using Coordinates;
using System;

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
        Declaration declaration = track.GetLatestDeclaration(DeclarationNumber());
        if (declaration == null)
        {
            comment += $"No declaration found in goal number {DeclarationNumber()}. | ";
            return true;
        }

        if (Math.Abs(declaration.DeclaredGoal.AltitudeBarometric - CoordinateHelpers.ConvertToMeter(2000)) > 0.1)
        {
            comment += $"Declared altitude is not 2000m. ({declaration.DeclaredGoal.AltitudeBarometric}) | ";
            declaration.DeclaredGoal.AltitudeBarometric = CoordinateHelpers.ConvertToMeter(2000);
        }
        
        var distanceBetweenDecAndDecPoint = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, declaration.PositionAtDeclaration);
        if (distanceBetweenDecAndDecPoint < 3000)
        {
            var penalty = DistanceViolationPenalties.CalculateAndFormatPenalty(distanceBetweenDecAndDecPoint, 3000, "TP");
            comment += $"Declared goal is to close to declaration point. ({distanceBetweenDecAndDecPoint}m / 3000m) [{penalty}] | ";
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