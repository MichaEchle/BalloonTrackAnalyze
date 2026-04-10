using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._01.tasks;
using System;

namespace JansScoring.flights.impl._01;

public class Flight01 : Flight
{
    public override int FlightNumber()
    {
        return 1;
    }

    public override DateTime StartOfLaunchPeriode()
    {
        return new DateTime(2026, 04, 10, 14,00, 00);
    }

    public override int LaunchPeriode()
    {
        return 180;
    }

    public override bool UseGPSAltitude()
    {
        return true;
    }

    public override int DistanceToAllGoals()
    {
        return 1000;
    }

    public override string TracksPath()
    {
        return @"/home/codingphoenix/Documents/balloon/competitions/2026 BWLV Sonnenbühl/scoring/flights/flight_01/tracks/";;
    }

    public override Task[] Tasks()
    {
        return new Task[]
        {
            //new Task01(this),
            //new Task02(this),
            new Task03(this)
        };
    }

    public override CalculationType CalculationType()
    {
        return calculation.CalculationType.Haversin;
    }

    public override double SeperationAltitudeFeet()
    {
        return 3300;
    }

    public override Coordinate BackupCoordinates()
    {
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 516350, 5357050);
    }

    public override int QNH()
    {
        return 1013;
    }
}