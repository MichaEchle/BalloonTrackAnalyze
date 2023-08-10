using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights;

public class FlightTestOne : Flight
{
    public override int getFlightNumber()
    {
        return 0;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2022, 09, 06, 04, 00, 00);
    }

    public override int launchPeriode()
    {
        return 120;
    }


    public override bool useGPSAltitude()
    {
        return true;
    }

    public override int distanceToAllGoals()
    {
        return 1000;
    }

    public override string getTracksPath()
    {
        return @"C:\balloon\comp\2022_09_Bad_Waltersdorf\flights\training flight 2\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task01(this) };
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.Haversin;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 2000;
    }

    public override Coordinate getBackupCoordinates()
    {
        throw new NotImplementedException();
    }


    private class Task01 : Task
    {
        public override int number()
        {
            return 1;
        }

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 06, 06, 00, 00);
        }

        public override string[] score(Track track)
        {
            return new[] { track.TrackPoints.Count.ToString() };
        }

        public override Coordinate[] goals()
        {
            return new Coordinate[] { };
        }

        public Task01(Flight flight) : base(flight)
        {
        }
    }
}