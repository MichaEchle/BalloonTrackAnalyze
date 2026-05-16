using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._03.tasks;
using System;

namespace JansScoring.flights.impl._03;

public class Flight03 : Flight
{
    public override int FlightNumber()
    {
        return 3;
    }

    public override DateTime StartOfLaunchPeriode()
    {
        return new DateTime(2026, 05, 16, 03,51, 00);
    }

    public override int LaunchPeriode()
    {
        return 55;
    }

    public override bool UseGPSAltitude()
    {
        return false;
    }

    public override double DistanceToAllGoals()
    {
        return 500;
    }

    public override string TracksPath()
    {
        return @"/home/codingphoenix/Documents/balloon/competitions/2026 SMHL Langenthal/scoring/flights/flight_03/tracks/";
    }

    public override Task[] Tasks()
    {
        return
        [
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
        return 3000;
    }

    public override Coordinate BackupCoordinates()
    {
        return CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03,623180, 228470);
    }

    public override CoordinateSystem DeclarationCoordinateSystem()
    {
        return CoordinateSystem.SwissGrid_LV03;
    }

    public override int QNH()
    {
        return 1008;
    }
}