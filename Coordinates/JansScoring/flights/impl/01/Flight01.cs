using Coordinates;
using JansScoring.calculation;
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
        return new DateTime(2025, 08, 06, 03, 54, 00);
    }

    public override int LaunchPeriode()
    {
        return 66;
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
        return new Task[] { };
    }

    public override CalculationType CalculationType()
    {
        return calculation.CalculationType.Haversin;
    }

    public override double SeperationAltitudeFeet()
    {
        return 1800;
    }

    public override Coordinate BackupCoordinates()
    {
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 625598, 5521249);
    }

    public override int QNH()
    {
        return 1025;
    }
}