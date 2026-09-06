using Coordinates;
using JansScoring.flights.tasks;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._02.tasks;

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
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 624420, 228427, CoordinateHelpers.ConvertToMeter(1599)),
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 625761, 227615, CoordinateHelpers.ConvertToMeter(1611)),
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626268, 229903, CoordinateHelpers.ConvertToMeter(1554)), 
            CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626788, 228294, CoordinateHelpers.ConvertToMeter(1587)) 
        ];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026,05,15,18,58,00);
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