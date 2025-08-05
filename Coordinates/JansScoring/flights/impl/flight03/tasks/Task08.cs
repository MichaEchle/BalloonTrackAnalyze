using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl.flight03.tasks;

public class Task08 : TaskHWZ
{
    public Task08(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 8;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return NORMAL_CALCULATION;
    }

    public override Coordinate[] Goals()
    {
        return new Coordinate[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 515357, 5355169,
                CoordinateHelpers.ConvertToMeter(2590)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 516601, 5354831, 763),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 518168, 5354638, 716)
        };
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2025, 04, 05, 18, 00, 00);
    }

    protected override int MarkerNumber()
    {
        return 1;
    }

    protected override int MMA()
    {
        return 50;
    }
}