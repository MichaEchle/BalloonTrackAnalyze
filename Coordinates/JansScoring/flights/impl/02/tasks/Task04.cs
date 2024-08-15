using Coordinates;
using JansScoring.check;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._02.tasks;

public class Task04 : TaskJDG
{
    public Task04(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 4;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override Coordinate[] Goals()
    {
        return new[] { CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0504670, 5328745, CoordinateHelpers.ConvertToMeter(907)) };
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 15, 05, 30, 00);
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