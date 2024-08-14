using Coordinates;
using System;

namespace JansScoring.flights.impl._01.tasks;

public class Task01 : TaskHNH
{
    public Task01(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 01;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override Coordinate[] Goals()
    {
        return new Coordinate[] { CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0506348, 5327307) };
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 14, 18, 00, 00);
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