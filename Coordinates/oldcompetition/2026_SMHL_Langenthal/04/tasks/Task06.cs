using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._04.tasks;

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

    public override Coordinate[] Goals(int pilot)
    {
        return
        [
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626202, 228911, CoordinateHelpers.ConvertToMeter(1466)),
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626202, 228911, CoordinateHelpers.ConvertToMeter(1466)),
        ];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 05, 16, 18, 59, 00);

    }

    protected override int MarkerNumber()
    {
        return 1;
    }

    protected override int MMA()
    {
        return 30;
    }
}