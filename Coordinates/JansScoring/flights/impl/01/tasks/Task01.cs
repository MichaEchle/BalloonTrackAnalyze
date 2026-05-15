using Coordinates;
using JansScoring.flights.tasks;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._01.tasks;

public class Task01 : TaskHWZ
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
        MarkerDrop markerDrop = track.MarkerDrops.FindLast(x => x.MarkerNumber == MarkerNumber());
        if (markerDrop == null)
        {
            comment += $"No marker drops at slot {MarkerNumber()}. | ";
            return true;
        }
        if(markerDrop.MarkerTime > ScoringPeriodUntil())
        {
            comment += $"Marker drop at slot {MarkerNumber()} is after the scoring period. | ";
            return true;
        }

        return false;
    }

    public override Coordinate[] Goals(int pilot)
    {
        return
        [
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626240, 229890, CoordinateHelpers.ConvertToMeter(1555)),
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626210, 228930, CoordinateHelpers.ConvertToMeter(1572))
        ];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026,05,15,05,30,00);
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