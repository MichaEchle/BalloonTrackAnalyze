using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._02.tasks;
using System;

namespace JansScoring.flights.impl._02;

public class Flight02 : Flight
{
    public override int FlightNumber()
    {
        return 2;
    }

    public override DateTime StartOfLaunchPeriode()
    {
        return new DateTime(2026, 09, 05, 04, 40, 00);
    }

    public override int LaunchPeriode()
    {
        return 65;
    }

    public override bool UseGPSAltitude()
    {
        return true;
    }

    public override double DistanceToAllGoals()
    {
        return 500;
    }

    public override string TracksPath()
    {
        return "/home/codingphoenix/Documents/balloon/competitions/2026 Höhefeld/tracks/flight_02/input/";
    }

    public override Task[] Tasks()
    {
        return
        [
            new Task01(this),
            new Task02(this),
            new Task03(this),
            new Task04(this),
            new Task05(this)
        ];
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
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 544145, 5506565);
    }

    public override int QNH()
    {
        return 1021;
    }
}