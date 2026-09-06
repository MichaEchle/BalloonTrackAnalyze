using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._02.tasks;

public class Task05 : TaskHWZ
{
    public Task05(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 05;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override Coordinate[] Goals(int pilot)
    {
        return
        [
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 542820, 5507298, CoordinateHelpers.ConvertToMeter(971)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 544230, 5507399, CoordinateHelpers.ConvertToMeter(1080)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 545152, 5507579, CoordinateHelpers.ConvertToMeter(1024)),
        ];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 09, 05, 07, 00, 00);
    }

    protected override int MarkerNumber()
    {
        return 5;
    }

    protected override int MMA()
    {
        return 50;
    }
}