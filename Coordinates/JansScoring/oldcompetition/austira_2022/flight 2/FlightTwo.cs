using Coordinates;
using JansScoring.calculation;
using System;
using System.Diagnostics;

namespace JansScoring.flights.flight_2;

public class FlightTwo : Flight
{
    public override int getFlightNumber()
    {
        return 2;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2022, 09, 08, 15, 53, 00);
    }

    public override int launchPeriode()
    {
        return 30;
    }

    public override bool useGPSAltitude()
    {
        return false;
    }

    public override int distanceToAllGoals()
    {
        return -1;
    }

    public override string getTracksPath()
    {
        return @"C:\balloon\comp\2022_09_Bad_Waltersdorf\flights\flight 2\tracks";
    }

    public override Task[] getTasks()
    {
        return new[] { new Task7(this) };
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.UTMPrecise;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 2000;
    }

    public override Coordinate getBackupCoordinates()
    {
        throw new NotImplementedException();
    }


    //Fuchsjagt
    public class Task7 : Task
    {
        public Task7(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 7;
        }

        public override string[] score(Track track)
        {
            string result = "";
            string comment = "";

            MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 2);

            if (markerDrop == null)
            {
                return new[] { "No Result", "No marker found" };
            }

            if ((flight.useGPSAltitude()
                    ? markerDrop.MarkerLocation.AltitudeGPS
                    : markerDrop.MarkerLocation.AltitudeBarometric) > flight.getSeperationAltitudeMeters())
            {
                result = NumberHelper.formatDoubleToStringAndRound(CoordinateHelpers.Calculate3DDistance(
                    markerDrop.MarkerLocation, goals()[0],
                    flight.useGPSAltitude(), flight.getCalculationType()));
            }
            else
            {
                result = NumberHelper.formatDoubleToStringAndRound(
                    CalculationHelper.Calculate2DDistance(markerDrop.MarkerLocation, goals()[0],
                        flight.getCalculationType()));
            }


            return new[] { result, comment };
        }

        public override Coordinate[] goals()
        {
            return new[]
            {
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 578939, 5224073,
                    234)
            };
        }

        public override DateTime getScoringPeriodUntil()
        {
            return new DateTime(2022, 09, 08, 17, 30, 00);
        }
    }
}