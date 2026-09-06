using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._04.tasks;
using System;

namespace JansScoring.flights.impl._04;

public class Flight04 : Flight
{
    public override int FlightNumber()
    {
        return 4;
    }

    public override DateTime StartOfLaunchPeriode()
    {
        return new DateTime(2026, 05, 16, 16, 30, 00);
    }

    public override int LaunchPeriode()
    {
        return 60;
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
        return
            @"/home/codingphoenix/Documents/balloon/competitions/2026 SMHL Langenthal/scoring/flights/flight_04/tracks/";
    }

    public override Task[] Tasks()
    {
        return
        [
            new Task06(this),
            new Task07(this),
            new Task08(this),
            new Task09(this)
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
        return CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623180, 228470);
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