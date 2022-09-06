using Coordinates;
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


    public override DateTime getScoringPeriodeUntil()
    {
        return new DateTime(2022, 09, 06, 06, 00, 00);
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
        return @"C:\balloon\comp\2022_09_Bad_Waltersdorf\flights\flight 0\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task01(this) };
    }


    private class Task01 : Task
    {

        public override int number()
        {
            return 1;
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