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
        return new DateTime(2026, 09, 06, 04, 40, 00);
    }

    public override int LaunchPeriode()
    {
        return 50;
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
        return "/home/codingphoenix/Documents/balloon/competitions/2026 Höhefeld/tracks/flight_03/input/";
    }

    public override Task[] Tasks()
    {
        Task07 task07 = new(this);
        Task08 task08 = new(this);
        task07.AddPair(task08);
        task08.AddPair(task07);
        return
        [
            new Task06(this),
            task07,
            task08,
            new Task09(this),
            new Task10(this)
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
        return 1028;
    }
}