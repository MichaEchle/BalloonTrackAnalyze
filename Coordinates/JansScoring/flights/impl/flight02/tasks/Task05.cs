using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl.flight02.tasks;

public class Task05 : TaskJDG
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

    public override Coordinate[] Goals()
    {
        return new[] { CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 517159, 5354635, CoordinateHelpers.ConvertToMeter(2481)) };
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2025, 04, 05, 06, 45, 00);
    }

    protected override int MarkerNumber()
    {
        return 2;
    }

    protected override int MMA()
    {
        return 75;
    }
}