using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._05.tasks;

public class Task10 : TaskHWZ
{
    public Task10(Flight flight) : base(flight)
    {

    }

    public override int TaskNumber()
    {
        return 10;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override Coordinate[] Goals(int pilot)
    {
        return [
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622141, 213321, CoordinateHelpers.ConvertToMeter(2636)), //809
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622244, 213340, CoordinateHelpers.ConvertToMeter(2581)), //810
        ];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 05, 17, 06, 00, 00);
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