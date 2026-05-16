using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._04.tasks;

public class Task07 : TaskHNH
{
    public Task07(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 7;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override Coordinate[] Goals(int pilot)
    {
        return
        [
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 629011, 225651, CoordinateHelpers.ConvertToMeter(2066))
        ];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 05, 16, 18, 59, 00);
    }

    protected override int MarkerNumber()
    {
        return 2;
    }

    protected override int MMA()
    {
        return 15;
    }
}