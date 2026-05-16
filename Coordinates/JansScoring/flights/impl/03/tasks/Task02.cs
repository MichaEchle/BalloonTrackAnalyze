using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._03.tasks;

public class Task02 : TaskJDG
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

    public override Coordinate[] Goals(int pilot)
    {
        return
        [
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622949, 227746,
                CoordinateHelpers.ConvertToMeter(1702))
        ];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 05, 16, 06, 00, 00);
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