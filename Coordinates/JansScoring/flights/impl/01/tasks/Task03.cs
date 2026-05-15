using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._01.tasks;

public class Task03 : TaskHWZ
{
    public Task03(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 03;
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
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 632430, 232200, CoordinateHelpers.ConvertToMeter(1598)),
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 633380, 229620, CoordinateHelpers.ConvertToMeter(1834))
        ];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026,05,15,05,30,00);
    }

    protected override int MarkerNumber()
    {
        return 3;
    }

    protected override int MMA()
    {
        return 30;
    }
}