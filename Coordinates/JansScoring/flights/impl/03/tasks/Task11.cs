using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._03.tasks;

public class Task11 : TaskJDG
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
        return new[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0511650, 5328320,
                CoordinateHelpers.ConvertToMeter(1030))
        };
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 15, 18, 00, 00);
    }

    protected override int MarkerNumber()
    {
        return 1;
    }

    protected override int MMA()
    {
        return 50;
    }
}