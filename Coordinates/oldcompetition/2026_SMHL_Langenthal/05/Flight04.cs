using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._04.tasks;
using JansScoring.flights.impl._05.tasks;
using System;

namespace JansScoring.flights.impl._05;

public class Flight05 : Flight
{
    public override int FlightNumber()
    {
        return 5;
    }

    public override DateTime StartOfLaunchPeriode()
    {
        return new DateTime(2026, 05, 17, 03, 50, 00);
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
        return 1000;
    }

    public override string TracksPath()
    {
        return
            @"/home/codingphoenix/Documents/balloon/competitions/2026 SMHL Langenthal/scoring/flights/flight_05/tracks/";
    }

    public override Task[] Tasks()
    {
        return
        [
            new Task10(this),
            new Task11(this),
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
        return 1016;
    }
}