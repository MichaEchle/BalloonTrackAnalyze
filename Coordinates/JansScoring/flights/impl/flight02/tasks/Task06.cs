using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl.flight02.tasks;

public class Task06 : TaskHWZ
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
        return false;
    }

    public override Coordinate[] Goals()
    {
        return new[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 515357, 5355169,
                CoordinateHelpers.ConvertToMeter(2597)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 514489, 5354031, 776),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 515851, 5353410, 757)
        };
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2025, 04, 05, 07, 00, 00);
    }

    protected override int MarkerNumber()
    {
        return 3;
    }

    protected override int MMA()
    {
        return 50;
    }
}