using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._02.tasks;

public class Task06 : TaskHWZ
{
    public Task06(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 6;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override Coordinate[] Goals()
    {
        return new Coordinate[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0512643,5328526, CoordinateHelpers.ConvertToMeter(1044)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0512590,5327734, 377),
        };
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024,08,15,05,30,00);
    }

    protected override int MarkerNumber()
    {
        return 3;
    }

    protected override int MMA()
    {
        return 50;
    }
}