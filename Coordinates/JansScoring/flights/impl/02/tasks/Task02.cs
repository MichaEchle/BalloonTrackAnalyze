using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._02.tasks;

public class Task02 : TaskJDG
{
    public Task02(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 02;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override Coordinate[] Goals(int pilot)
    {
        return
        [
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 539812, 5504347, CoordinateHelpers.ConvertToMeter(924))
        ];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 09, 05, 06, 00, 00);
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