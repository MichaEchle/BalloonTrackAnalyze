using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.flight_5;

public class FlightFive : Flight
{
    public override int getFlightNumber()
    {
        return 5;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2022, 09, 10, 15, 00, 00);
    }

    public override int launchPeriode()
    {
        return 80;
    }

    public override bool useGPSAltitude()
    {
        return false;
    }

    public override int distanceToAllGoals()
    {
        return 1000;
    }

    public override string getTracksPath()
    {
        return @"C:\balloon\comp\2022_09_Bad_Waltersdorf\flights\flight 5\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task23(this) };
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.UTMPrecise;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 2000;
    }


    public class Task23 : Task
    {
        public Task23(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 23;
        }

        public override string[] score(Track track)
        {
            double distance = double.NaN;
            string reasonForNoResult = "";
            MarkerDrop marker1 = track.MarkerDrops.FindLast(x => x.MarkerNumber == 1);
            MarkerDrop marker2 = track.MarkerDrops.FindLast(x => x.MarkerNumber == 2);
            int marker1SliceNumber = -1;
            int marker2SliceNumber = -1;
            Coordinate centerPoint = goals()[0];
            Coordinate referencePoint = CoordinateHelpers.CalculatePointWithDistanceAndBearing(centerPoint, 50, 0);
            if ((marker1 is null) || (marker2 is null))
            {
                return new[] { "No Result", "Marker 2 or Marker 3 not dropped" };
            }

            double distanceMarker2 =
                CalculationHelper.Calculate2DDistance(marker1.MarkerLocation, centerPoint, flight.getCalculationType());
            double distanceMarker3 =
                CalculationHelper.Calculate2DDistance(marker2.MarkerLocation, centerPoint, flight.getCalculationType());

            if (distanceMarker2 < 0 || distanceMarker3 < 0)
                reasonForNoResult = "Marker 2 or Marker 3 to close to center point";
            else
            {
                double angleMarker2 = CoordinateHelpers.CalculateInteriorAngle(marker1.MarkerLocation, centerPoint,
                    referencePoint, flight.getCalculationType());
                if (marker1.MarkerLocation.Longitude < centerPoint.Longitude)
                    angleMarker2 = 360 - angleMarker2;
                double angleMarker3 = CoordinateHelpers.CalculateInteriorAngle(marker2.MarkerLocation, centerPoint,
                    referencePoint, flight.getCalculationType());
                if (marker2.MarkerLocation.Longitude < centerPoint.Longitude)
                    angleMarker3 = 360 - angleMarker3;

                for (int index = 0; index < 8; index++)
                {
                    if (angleMarker2 >= (360 / 8 * index) && angleMarker2 < (360 / 8 * (index + 1)))
                        marker1SliceNumber = index + 1;
                    if (angleMarker3 >= (360 / 8 * index) && angleMarker3 < (360 / 8 * (index + 1)))
                        marker2SliceNumber = index + 1;
                }

                double angle = CoordinateHelpers.CalculateInteriorAngle(marker1.MarkerLocation, centerPoint,
                    marker2.MarkerLocation, flight.getCalculationType());
                if (angle < 45.0)
                    return new[] { "No Result", "Angle between markers below 45 Degrees" };
                else if (angle >= 45.0 && angle < 90.0)
                {
                    if (marker1SliceNumber == marker2SliceNumber)
                        return new[] { "No Result", $"Markers are in same slice ({marker1SliceNumber})" };
                    else
                    {
                        if (marker2SliceNumber == ((marker1SliceNumber + 6) % 8) + 1)
                            return new[]
                            {
                                "No Result",
                                $"Markers are in adjacent slices (1: {marker1SliceNumber} | 2: {marker2SliceNumber})"
                            };
                        else if (marker2SliceNumber == ((marker1SliceNumber) % 8) + 1)
                            return new[]
                            {
                                "No Result",
                                $"Markers are in adjacent slices (1: {marker1SliceNumber} | 2: {marker2SliceNumber})"
                            };
                        else
                            distance = CalculationHelper.Calculate2DDistance(marker1.MarkerLocation,
                                marker2.MarkerLocation, flight.getCalculationType());
                    }
                }
                else //angle >=90
                {
                    distance = CalculationHelper.Calculate2DDistance(marker1.MarkerLocation, marker2.MarkerLocation,
                        flight.getCalculationType());
                }
            }

            return new[]
            {
                NumberHelper.formatDoubleToStringAndRound(distance),
                (reasonForNoResult != "" ? $"{reasonForNoResult} | " : "") +
                $"Marker 1 is in slice {marker1SliceNumber} | Marker 2 is in slice {marker2SliceNumber}"
            };
        }

        public override Coordinate[] goals()
        {
            return new[]
            {
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 575970, 5224640,
                    CoordinateHelpers.ConvertToMeter(964))
            };
        }

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 10, 17, 20, 00);
        }
    }


    public class Task24 : Task
    {
        public Task24(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 24;
        }

        public override string[] score(Track track)
        {
            string result = "";
            string comment = "";

            MarkerDrop markerDrop = track.MarkerDrops.FindLast(x => x.MarkerNumber == 3);
            if (markerDrop == null)
                return new[] { "No Result", "No Markerdrop found" };

            double distance2d = CalculationHelper.Calculate2DDistance(markerDrop.MarkerLocation, goals()[0],
                flight.getCalculationType());
            double distance = Double.NaN;
            if ((flight.useGPSAltitude()
                    ? markerDrop.MarkerLocation.AltitudeGPS
                    : markerDrop.MarkerLocation.AltitudeBarometric) > flight.getSeperationAltitudeMeters())
            {
                distance = CoordinateHelpers.Calculate3DDistance(markerDrop.MarkerLocation, goals()[0],
                    flight.useGPSAltitude(),
                    flight.getCalculationType());
            }
            else
            {
                distance = distance2d;
            }

            if (distance2d < 30)
                return new[] { "No Result", "Marker inside MMA" };

            result = NumberHelper.formatDoubleToStringAndRound(distance);

                return new[] { result, comment };
        }

        public override Coordinate[] goals()
        {
            return new[]
            {
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 577640, 5223630,
                    CoordinateHelpers.ConvertToMeter(963))
            };
        }

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 10, 17, 20, 00);
        }
    }
}