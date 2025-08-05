using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl.flight01.tasks;

public class Task02 : TaskHWZ
{
    public Task02(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 2;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override Coordinate[] Goals()
    {
        return new[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 513901, 5357501,
                CoordinateHelpers.ConvertToMeter(2556)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 513571, 5357273,
                CoordinateHelpers.ConvertToMeter(2556)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 513917, 5356812, 777)
        };
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2025, 04, 04, 18, 00, 00);
    }

    protected override int MarkerNumber()
    {
        return 2;
    }

    protected override int MMA()
    {
        return 50;
    }
}