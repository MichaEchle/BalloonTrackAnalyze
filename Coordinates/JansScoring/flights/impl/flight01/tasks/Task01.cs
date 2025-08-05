using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl.flight01.tasks;

public class Task01 : TaskJDG
{
    public Task01(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 1;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override Coordinate[] Goals()
    {
        return new Coordinate[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",515049,5357106,731)
        };
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2025, 04, 04, 18, 00, 00);
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