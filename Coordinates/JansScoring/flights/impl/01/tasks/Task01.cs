using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._01.tasks;

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
        throw new NotImplementedException();
    }

    public override Coordinate[] Goals(int pilot)
    {
        return [CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 1, 1, 1)];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 09, 04, 05, 30, 00);
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