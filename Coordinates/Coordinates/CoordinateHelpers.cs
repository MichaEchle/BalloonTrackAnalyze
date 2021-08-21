using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Coordinates
{
    /// <summary>
    /// Formulas has been taken from:
    /// <para>https://www.movable-type.co.uk/scripts/latlong.html</para>
    /// </summary>
    public static class CoordinateHelpers
    {
        /// <summary>
        /// the ratio of feet to meter conversion
        /// </summary>
        public const double FEET_TO_METER_RATIO = 0.3048;

        /// <summary>
        /// the earths radius in meter
        /// </summary>
        public const double EARTH_RADIUS_METER = 6371.0e3;

        /// <summary>
        /// Convert a part of coordinate from degree minute notation to decimal degree
        /// </summary>
        /// <param name="degrees">the integer degrees</param>
        /// <param name="degreeMinutes">the integer degree minutes</param>
        /// <param name="degreeSeconds">the integer degree seconds</param>
        /// <param name="isNorthingOrEasting">true: is northing or easting; false: is southing or westing </param>
        /// <returns>the part of a coordinate in decimal degree</returns>
        public static double ConvertToDecimalDegree(int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthseconds, bool isNorthingOrEasting)
        {
            return degrees + (degreeMinutes / 60.0) + (degreeSeconds / 3600.0) + (degreeTenthseconds / 36000.0) * (isNorthingOrEasting ? 1.0 : -1.0);
        }

        public static (int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds) ConvertToDegreeMinutes(double decimalDegrees)
        {
            while (decimalDegrees < -180.0)
                decimalDegrees += 360.0;
            while (decimalDegrees > 180.0)
                decimalDegrees -= 360.0;
            int degrees = (int)Math.Floor(decimalDegrees);
            double minutes = (decimalDegrees - degrees) * 60.0;
            double seconds = (minutes - Math.Floor(minutes)) * 60.0;
            double tenths = (seconds - Math.Floor(seconds)) * 10.0;

            int degreeMinutes = (int)Math.Floor(minutes);
            int degreeSeconds = (int)Math.Floor(seconds);
            int degreeTenthSeconds = (int)Math.Round(tenths, 0, MidpointRounding.AwayFromZero);

            return (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds);
        }


        public static string BeautifyDegreeMinutes(int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds)
        {
            return $"{degrees}° {degreeMinutes}ʹ {degreeSeconds}ʺ {degreeTenthSeconds}ʺʹ";
        }
        /// <summary>
        /// Convert feet into meters
        /// </summary>
        /// <param name="feets">the feet to be converted</param>
        /// <returns>the amount of feet in meters</returns>
        public static double ConvertToMeter(double feets)
        {
            return feets * FEET_TO_METER_RATIO;
        }

        /// <summary>
        /// Converts meters into feet
        /// </summary>
        /// <param name="meters">the meters to be converted</param>
        /// <returns>the amount of meter in feet</returns>
        public static double ConvertToFeet(double meters)
        {
            return meters / FEET_TO_METER_RATIO;
        }

        /// <summary>
        /// Calculate the 2D distance [m] between the two coordinates using havercos formula
        /// </summary>
        /// <param name="coordinate1">the first coordinate</param>
        /// <param name="coordinate2">the second coordinate</param>
        /// <returns>the distance in meters</returns>
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


            double phi1 = coordinate1.Latitude * Math.PI / 180.0;
            double phi2 = coordinate2.Latitude * Math.PI / 180.0;
            double lambda1 = coordinate1.Longitude * Math.PI / 180.0;
            double lambda2 = coordinate2.Longitude * Math.PI / 180.0;
            double deltaPhi = phi2 - phi1;
            double deltaLambda = lambda2 - lambda1;

            double a = Math.Pow(Math.Sin(deltaPhi / 2.0), 2) + Math.Cos(phi1) * Math.Cos(phi2) * Math.Pow(Math.Sin(deltaLambda / 2.0), 2);
            double distance2D = 2.0 * EARTH_RADIUS_METER * Math.Asin(Math.Sqrt(a));
            return distance2D;
        }

        /// <summary>
        /// Calculate the 3D distance [m] between the two coordinates using havercos for 2D distance and Euclid for 3D distance
        /// </summary>
        /// <param name="coordinate1">the first coordinate</param>
        /// <param name="coordinate2">the second coordinate</param>
        /// <param name="useGPSAltitude">true: use GPS altitude; false: use barometric altitude</param>
        /// <returns>the 3D distance in meters</returns>
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

        /// <summary>
        /// Accumulates the 2D distance [m] between consecutive coordinates
        /// <para>ensure the coordinates are sorted accordingly</para>
        /// </summary>
        /// <param name="coordinates">the list of coordinates</param>
        /// <returns>the accumulated 2D distance in meters</returns>
        public static double Calculate2DDistanceBetweenPoints(List<Coordinate> coordinates)
        {
            double result = 0.0;
            for (int index = 0; index < coordinates.Count - 1; index++)
            {
                result += Calculate2DDistance(coordinates[index], coordinates[index + 1]);
            }
            return result;
        }

        /// <summary>
        /// Accumulates the 3D distance [m] between consecutive coordinates
        /// <para>ensure the coordinates are sorted accordingly</para>
        /// </summary>
        /// <param name="coordinates">the list of coordinates</param>
        /// <param name="useGPSAltitude">true: use GPS altitude; false: use barometric altitude</param>
        /// <returns>the accumulated 3D distance in meters</returns>
        public static double Calculate3DDistanceBetweenPoints(List<Coordinate> coordinates, bool useGPSAltitude)
        {
            double result = 0.0;
            for (int index = 0; index < coordinates.Count - 1; index++)
            {
                result += Calculate3DDistance(coordinates[index], coordinates[index + 1], useGPSAltitude);
            }
            return result;
        }

        /// <summary>
        /// Calculate the interior angle at <paramref name="coordinateB"/> where the route is defined from <paramref name="coordinateA"/> to <paramref name="coordinateB"/> and <paramref name="coordinateB"/> to <paramref name="coordinateC"/>  
        /// </summary>
        /// <param name="coordinateA">first coordinate</param>
        /// <param name="coordinateB">second coordinate</param>
        /// <param name="coordinateC">third coordinate</param>
        /// <returns>the interior angle in degrees</returns>
        public static double CalculateInteriorAngle(Coordinate coordinateA, Coordinate coordinateB, Coordinate coordinateC)
        {
            if (coordinateA is null)
            {
                throw new ArgumentNullException(nameof(coordinateA));
            }

            if (coordinateB is null)
            {
                throw new ArgumentNullException(nameof(coordinateB));
            }

            if (coordinateC is null)
            {
                throw new ArgumentNullException(nameof(coordinateC));
            }

            double result = 0.0;
            double a = Calculate2DDistance(coordinateB, coordinateC);
            double b = Calculate2DDistance(coordinateA, coordinateC);
            double c = Calculate2DDistance(coordinateA, coordinateB);

            double beta = Math.Acos((Math.Pow(a, 2) + Math.Pow(c, 2) - Math.Pow(b, 2)) / (2 * a * c));
            result = beta / Math.PI * 180.0;
            return result;
        }

        /// <summary>
        /// Calculate the area of the triangle defined by <paramref name="coordinateA"/>, <paramref name="coordinateB"/> and <paramref name="coordinateC"/>
        /// </summary>
        /// <param name="coordinateA">first coordinate</param>
        /// <param name="coordinateB">second coordinate</param>
        /// <param name="coordinateC">third coordinate</param>
        /// <returns>the area in square meters</returns>
        public static double CalculateArea(Coordinate coordinateA, Coordinate coordinateB, Coordinate coordinateC)
        {
            if (coordinateA is null)
            {
                throw new ArgumentNullException(nameof(coordinateA));
            }

            if (coordinateB is null)
            {
                throw new ArgumentNullException(nameof(coordinateB));
            }

            if (coordinateC is null)
            {
                throw new ArgumentNullException(nameof(coordinateC));
            }

            double result = 0.0;
            double a = Calculate2DDistance(coordinateB, coordinateC);
            double b = Calculate2DDistance(coordinateA, coordinateC);
            double c = Calculate2DDistance(coordinateA, coordinateB);

            double halfOfCircumference = (a + b + c) / 2.0;

            double area = Math.Sqrt(halfOfCircumference * (halfOfCircumference - a) * (halfOfCircumference - b) * (halfOfCircumference - c));
            result = area;

            return result;
        }

        /// <summary>
        /// Calculate a coordinate with given start point (<paramref name="coordinate1"/>), distance and bearing
        /// <para>altitude will be copied for <paramref name="coordinate1"/> and time stamp set to current UTC time at time of calculation</para>
        /// </summary>
        /// <param name="coordinate1">a coordinate as start point</param>
        /// <param name="distanceInMeters">the distance in meters</param>
        /// <param name="bearingInDecimalDegree">the bearing in decimal degree</param>
        /// <returns>a target coordinate</returns>
        public static Coordinate CalculatePointWithDistanceAndBearing(Coordinate coordinate1, double distanceInMeters, double bearingInDecimalDegree)
        {
            if (coordinate1 is null)
            {
                throw new ArgumentNullException(nameof(coordinate1));
            }
            if (double.IsNaN(distanceInMeters) || double.IsInfinity(distanceInMeters))
            {
                throw new ArgumentException(nameof(distanceInMeters));
            }

            if (double.IsNaN(bearingInDecimalDegree) || double.IsInfinity(bearingInDecimalDegree))
            {
                throw new ArgumentException(nameof(bearingInDecimalDegree));
            }

            double angularDistance = Math.Abs(distanceInMeters) / EARTH_RADIUS_METER;
            double lat1 = coordinate1.Latitude * Math.PI / 180.0;
            double long1 = coordinate1.Longitude * Math.PI / 180.0;
            double bearing = (bearingInDecimalDegree % 360.0) * Math.PI / 180.0;

            double latitude = Math.Asin(Math.Sin(lat1) * Math.Cos(angularDistance) + Math.Cos(lat1) * Math.Sin(angularDistance) * Math.Cos(bearing));
            double longitude = long1 + Math.Atan2(Math.Sin(bearing) * Math.Sin(angularDistance) * Math.Cos(lat1), Math.Cos(angularDistance) - Math.Sin(lat1) * Math.Sin(latitude));

            latitude *= 180.0 / Math.PI;
            longitude *= 180.0 / Math.PI;
            Coordinate coordinate = new Coordinate(latitude, longitude, coordinate1.AltitudeGPS, coordinate1.AltitudeBarometric, DateTime.UtcNow);

            return coordinate;
        }

        /// <summary>
        /// Calculate the initial bearing (forward azimuth) between the two coordinates 
        /// </summary>
        /// <param name="coordinate1">first coordinate</param>
        /// <param name="coordinate2">second coordinate</param>
        /// <returns>the initial bearing in degrees</returns>
        public static double CalculateInitalBearing(Coordinate coordinate1, Coordinate coordinate2)
        {
            if (coordinate1 is null)
            {
                throw new ArgumentNullException(nameof(coordinate1));
            }

            if (coordinate2 is null)
            {
                throw new ArgumentNullException(nameof(coordinate2));
            }

            double phi1 = coordinate1.Latitude * Math.PI / 180.0;
            double phi2 = coordinate2.Latitude * Math.PI / 180.0;

            double lambda1 = coordinate1.Longitude * Math.PI / 180.0;
            double lambda2 = coordinate2.Longitude * Math.PI / 180.0;

            double deltaLambda = lambda2 - lambda1;
            double bearing = Math.Atan2(Math.Sin(deltaLambda) * Math.Cos(phi2), Math.Cos(phi1) * Math.Sin(phi2) - Math.Sin(phi1) * Math.Cos(phi2) * Math.Cos(deltaLambda));

            bearing = ((bearing * 180.0 / Math.PI) + 360) % 360;

            return bearing;

        }

        /// <summary>
        /// Convert a position from UTM format to lat/long format
        /// <para>Please note that altitude is set to double.NaN and timestamp to DateTime.MinValue</para>
        /// </summary>
        /// <param name="utmZone">the UTM zone e.g. "32U"</param>
        /// <param name="easting">the easting portion e.g. 630084</param>
        /// <param name="northing">the northing portion e.g. 4833438</param>
        /// <returns>a Coordinate object</returns>
        public static Coordinate ConvertUTMToLatitudeLongitudeCoordinate(string utmZone, int easting, int northing)
        {
            (double latitude, double longitude) latitudeLongitude = ConvertUTMToLatitudeLongitude(utmZone, easting, northing);
           return new Coordinate(latitudeLongitude.latitude, latitudeLongitude.longitude, double.NaN, double.NaN, DateTime.MinValue);
        }

        /// <summary>
        /// Convert a position from UTM format to lat/long format
        /// </summary>
        /// <param name="utmZone">the UTM zone e.g. "32U"</param>
        /// <param name="easting">the easting portion e.g. 630084</param>
        /// <param name="northing">the northing portion e.g. 4833438</param>
        /// <returns>the latitude and longitude pair</returns>
        public static (double latitude, double longitude) ConvertUTMToLatitudeLongitude(string utmZone, int easting, int northing)
        {
            CoordinateSharp.UniversalTransverseMercator utmCoordindate = new CoordinateSharp.UniversalTransverseMercator(utmZone, easting, northing);
            CoordinateSharp.Coordinate coordinateSharp = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utmCoordindate);
            return (coordinateSharp.Latitude.DecimalDegree, coordinateSharp.Longitude.DecimalDegree);
        }

        /// <summary>
        /// Convert a Coordinate objects latitude and longitude to UTM format
        /// </summary>
        /// <param name="coordinate">the coordinate</param>
        /// <returns>UTM zone, easting and northing rounded to the next integer</returns>
        public static (string utmZone, int easting, int northing) ConvertLatitudeLongitudeCoordinateToUTM(Coordinate coordinate)
        {
            return ConvertLatitudeLongitudeToUTM(coordinate.Latitude, coordinate.Longitude);
        }

        /// <summary>
        /// Converts latitude and longitude to UTM format
        /// </summary>
        /// <param name="latitude">the latitude in decimal degrees</param>
        /// <param name="longitude">the longitude in decimal degrees</param>
        /// <returns>UTM zone,easting and northing rounded to the next integer</returns>
        public static (string utmZone, int easting, int northing) ConvertLatitudeLongitudeToUTM(double latitude, double longitude)
        {
            CoordinateSharp.Coordinate coordinateSharp = new CoordinateSharp.Coordinate(latitude, longitude);
            return ($"{coordinateSharp.UTM.LongZone}{coordinateSharp.UTM.LatZone}", (int)(Math.Round(coordinateSharp.UTM.Easting,0,MidpointRounding.AwayFromZero)), (int)(Math.Round(coordinateSharp.UTM.Northing,0,MidpointRounding.AwayFromZero)));
        }
    }
}
