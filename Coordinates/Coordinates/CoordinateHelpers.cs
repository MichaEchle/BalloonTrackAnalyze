using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinates
{
    public static class CoordinateHelpers
    {
        /// <summary>
        /// the ratio of feet in meter
        /// </summary>
        public const double FEET_TO_METER_RATIO = 0.3048;

        /// <summary>
        /// Convert a part of coordinate from degree minute notation to decimal degree
        /// </summary>
        /// <param name="degrees">the integer degrees</param>
        /// <param name="degreeMinutes">the integer degree minutes</param>
        /// <param name="degreeSeconds">the integer degree seconds</param>
        /// <param name="isNorthingOrEasting">true: is norting or easting; false: is southing or westing </param>
        /// <returns>the part of a coordinate in decimal degree</returns>
        public static double ConvertToDecimalDegree(int degrees, int degreeMinutes, int degreeSeconds, bool isNorthingOrEasting)
        {
            return degrees + (degreeMinutes / 60.0) + (degreeSeconds / 3600.0) * (isNorthingOrEasting ? 1.0 : -1.0);
        }

        /// <summary>
        /// Convert feets into meters
        /// </summary>
        /// <param name="feets">the feets to be converted</param>
        /// <returns>the amount of feet in meters</returns>
        public static double ConvertToMeter(double feets)
        {
            return feets * FEET_TO_METER_RATIO;
        }

        /// <summary>
        /// Converts meters into feets
        /// </summary>
        /// <param name="meters">the meters to be converted</param>
        /// <returns>the amount of meter in feet</returns>
        public static double ConvertToFeet(double meters)
        {
            return meters / FEET_TO_METER_RATIO;
        }

        public static double Calculate2DDistance(Coordinate coordinate1, Coordinate coordinate2)
        {
            if (coordinate1 is null)
            {
                throw new ArgumentNullException(nameof(coordinate1));
            }

            if (coordinate2 is null)
            {
                throw new ArgumentNullException(nameof(coordinate2));
            }

            double earthRadius = 6371.0e3;
            double phi1 = coordinate1.Latitude * Math.PI / 180.0;
            double phi2 = coordinate2.Latitude * Math.PI / 180.0;
            double lambda1 = coordinate1.Longitude * Math.PI / 180.0;
            double lambda2 = coordinate2.Longitude * Math.PI / 180.0;
            double deltaPhi = phi2 - phi1;
            double deltaLambda = lambda2 - lambda1;

            double a = Math.Pow(Math.Sin(deltaPhi / 2.0), 2) + Math.Cos(phi1) * Math.Cos(phi2) * Math.Pow(Math.Sin(deltaLambda / 2.0), 2);
            double distance2D = 2.0 * earthRadius * Math.Asin(Math.Sqrt(a));
            return distance2D;
        }

        public static double Calculate3DDistance(Coordinate coordinate1, Coordinate coordinate2, bool useGPSAltitude)
        {
            if (coordinate1 is null)
            {
                throw new ArgumentNullException(nameof(coordinate1));
            }

            if (coordinate2 is null)
            {
                throw new ArgumentNullException(nameof(coordinate2));
            }
            double distance2D = Calculate2DDistance(coordinate1, coordinate2);
            double deltaAltitude;
            if (useGPSAltitude)
            {
                deltaAltitude = coordinate1.AltitudeGPS - coordinate2.AltitudeGPS;
            }
            else
            {
                deltaAltitude = coordinate1.AltitudeBarometric - coordinate2.AltitudeBarometric;
            }

            double distance3D = Math.Sqrt(Math.Pow(distance2D, 2) + Math.Pow(deltaAltitude, 2));
            return distance3D;
        }

        public static double Calculate2DDistanceBetweenPoints(List<Coordinate> coordinates)
        {
            double result = 0.0;
            for (int index = 0; index < coordinates.Count - 1; index++)
            {
                result += Calculate2DDistance(coordinates[index], coordinates[index + 1]);
            }
            return result;
        }

        public static double Calculate3DDistanceBetweenPoints(List<Coordinate> coordinates, bool useGPSAltitude)
        {
            double result = 0.0;
            for (int index = 0; index < coordinates.Count - 1; index++)
            {
                result += Calculate3DDistance(coordinates[index], coordinates[index + 1], useGPSAltitude);
            }
            return result;
        }

        public static double CalculateInteriorAngle(Coordinate coordinateA, Coordinate coordinateB, Coordinate coordinateC)
        {
            double result = 0.0;
            double a = Calculate2DDistance(coordinateB, coordinateC);
            double b = Calculate2DDistance(coordinateA, coordinateC);
            double c = Calculate2DDistance(coordinateA, coordinateB);

            double beta = Math.Acos((Math.Pow(a, 2) + Math.Pow(c, 2) - Math.Pow(b, 2)) / (2 * a * c));
            result = beta / Math.PI * 180.0;
            return result;
        }

        public static double CalculateArea(Coordinate coordinateA, Coordinate coordinateB, Coordinate coordinateC)
        {
            double result = 0.0;
            double a = Calculate2DDistance(coordinateB, coordinateC);
            double b = Calculate2DDistance(coordinateA, coordinateC);
            double c = Calculate2DDistance(coordinateA, coordinateB);

            double halfOfCircumference = (a + b + c) / 2.0;

            double area = Math.Sqrt(halfOfCircumference * (halfOfCircumference - a) * (halfOfCircumference - b) * (halfOfCircumference - c));
            result = area;

            return result;
        }
    }
}
