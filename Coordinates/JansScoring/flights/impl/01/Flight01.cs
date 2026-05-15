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
        return new DateTime(2026, 05, 15, 03,50, 00);
    }

    public override int LaunchPeriode()
    {
        return 30;
    }

    public override bool UseGPSAltitude()
    {
        return false;
    }

    public override double DistanceToAllGoals()
    {
        return 0;
    }

    public override string TracksPath()
    {
        return @"/home/codingphoenix/Documents/balloon/competitions/2026 SMHL Langenthal/scoring/flights/flight_01/tracks/";
    }

    public override Task[] Tasks()
    {
        return new Task[]
        {
            new Task01(this),
            new Task02(this),
            new Task03(this)
        };
    }

    public override CalculationType CalculationType()
    {
        return calculation.CalculationType.Haversin;
    }

    public override double SeperationAltitudeFeet()
    {
        return 3000;
    }

    public override Coordinate BackupCoordinates()
    {
        return CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03,623180, 228470);
    }

    public override int QNH()
    {
        return 1001;
    }
}