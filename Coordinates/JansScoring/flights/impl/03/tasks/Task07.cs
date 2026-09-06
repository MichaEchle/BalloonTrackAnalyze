using Coordinates;
using JansScoring.check;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._03.tasks;

public class Task07 : TaskHWZPair
{
    
    public Task07(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 7;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }


    public override Coordinate[] Goals(int pilot)
    {
        return
        [
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 545153, 5507580, CoordinateHelpers.ConvertToMeter(1013)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 544301, 5507650, CoordinateHelpers.ConvertToMeter(1113)), 
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 544622, 5507138, CoordinateHelpers.ConvertToMeter(1191)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 543151, 5507649, CoordinateHelpers.ConvertToMeter(1011)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 542819, 5507304, CoordinateHelpers.ConvertToMeter(1003)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 543091, 5506620, CoordinateHelpers.ConvertToMeter(985))
        ];
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 09, 05, 06, 30, 00);
    }

    protected override int MarkerNumber()
    {
        return 2;
    }

    protected override int MMA()
    {
        return 50;
    }

    public override bool InOrder()
    {
        return false;
    }
}