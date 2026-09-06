using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._05.tasks;

public class Task11 : TaskHWZ
{
    public Task11(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 11;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override Coordinate[] Goals(int pilot)
    {
        return
        [
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626190, 215195,
                CoordinateHelpers.ConvertToMeter(2266)), //801
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623569, 214611,
                CoordinateHelpers.ConvertToMeter(2306)),
        ];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 05, 17, 06, 00, 00);
    }

    protected override int MarkerNumber()
    {
        return 2;
    }

    protected override int MMA()
    {
        return 30;
    }
}