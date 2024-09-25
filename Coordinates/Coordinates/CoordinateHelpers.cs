using LoggingConnector;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Xml.Xsl;
using static System.Math;

namespace Coordinates;

/// <summary>
/// Formulas has been taken from:
/// <para>https://www.movable-type.co.uk/scripts/latlong.html</para>
/// </summary>
public static class CoordinateHelpers
{

    private static readonly ILogger Logger = LogConnector.LoggerFactory.CreateLogger(nameof(CoordinateHelpers));

    /// <summary>
    /// the ratio of feet to meter conversion
    /// </summary>
    public const double FEET_TO_METER_RATIO = 0.304_8;

    /// <summary>
    /// the earths radius in meter
    /// </summary>
    public const double EARTH_RADIUS_METER = 6_371.0e3;



    //internal static class WSG84Parameters
    //{

    //    internal static double SEMI_MAJOR_AXIS = 6_378_137.0;
    //    internal static double EARTH_FLATTENING = 1.0 / 298.257_223_563;
    //    internal static double SEMI_MINOR_AXIS = 6_356_752.314_245;
    //}

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

    /// <summary>
    /// Converts decimal degrees to full degree with minutes, seconds and tenths of seconds
    /// </summary>
    /// <param name="decimalDegrees">the decimal degree value to be converted</param>
    /// <returns>degrees, minutes, seconds and tenth of seconds</returns>
    public static (int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds) ConvertToDegreeMinutes(double decimalDegrees)
    {
        if (double.IsInfinity(decimalDegrees) || double.IsNaN(decimalDegrees))
            throw new ArgumentOutOfRangeException(nameof(decimalDegrees), "Cannot be infinity or NaN");

        while (decimalDegrees < -180.0)
            decimalDegrees += 360.0;
        while (decimalDegrees > 180.0)
            decimalDegrees -= 360.0;
        int degrees = (int)Floor(decimalDegrees);
        double minutes = (decimalDegrees - degrees) * 60.0;
        double seconds = (minutes - Floor(minutes)) * 60.0;
        double tenths = (seconds - Floor(seconds)) * 10.0;

        int degreeMinutes = (int)Floor(minutes);
        int degreeSeconds = (int)Floor(seconds);
        int degreeTenthSeconds = (int)Round(tenths, 0, MidpointRounding.AwayFromZero);

        return (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds);
    }

    /// <summary>
    /// Creates a beautify string for output and logging purposes
    /// </summary>
    /// <param name="degrees">the full degrees</param>
    /// <param name="degreeMinutes">the degree minutes</param>
    /// <param name="degreeSeconds">the degree seconds</param>
    /// <param name="degreeTenthSeconds">the tenth of degree seconds</param>
    /// <returns>beautified string using the input values</returns>
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
        if (double.IsInfinity(feets) || double.IsNaN(feets))
            throw new ArgumentOutOfRangeException(nameof(feets), "Cannot be infinity or NaN");
        return feets * FEET_TO_METER_RATIO;
    }

    /// <summary>
    /// Converts meters into feet
    /// </summary>
    /// <param name="meters">the meters to be converted</param>
    /// <returns>the amount of meter in feet</returns>
    public static double ConvertToFeet(double meters)
    {
        if (double.IsInfinity(meters) || double.IsNaN(meters))
            throw new ArgumentOutOfRangeException(nameof(meters), "Cannot be infinity or NaN");
        return meters / FEET_TO_METER_RATIO;
    }

    /// <summary>
    /// Calculate the 2D distance [m] between the two coordinates using havercos formula
    /// </summary>
    /// <param name="coordinate1">the first coordinate</param>
    /// <param name="coordinate2">the second coordinate</param>
    /// <returns>the distance in meters</returns>
    public static double Calculate2DDistanceHavercos(Coordinate coordinate1, Coordinate coordinate2)
    {
        ArgumentNullException.ThrowIfNull(coordinate1);

        ArgumentNullException.ThrowIfNull(coordinate2);


        double phi1 = coordinate1.Latitude * PI / 180.0;
        double phi2 = coordinate2.Latitude * PI / 180.0;
        double lambda1 = coordinate1.Longitude * PI / 180.0;
        double lambda2 = coordinate2.Longitude * PI / 180.0;
        double deltaPhi = phi2 - phi1;
        double deltaLambda = lambda2 - lambda1;

        double a = Pow(Sin(deltaPhi / 2.0), 2) + Cos(phi1) * Cos(phi2) * Pow(Sin(deltaLambda / 2.0), 2);
        double distance2D = 2.0 * EARTH_RADIUS_METER * Asin(Sqrt(a));
        return distance2D;
    }

    /// <summary>
    /// Calculate the 2D distance [m] between the two coordinates using haversin formula
    /// </summary>
    /// <param name="coordinate1">the first coordinate</param>
    /// <param name="coordinate2">the second coordinate</param>
    /// <returns>the distance in meters</returns>
    public static double Calculate2DDistanceHaversin(Coordinate coordinate1, Coordinate coordinate2)
    {
        ArgumentNullException.ThrowIfNull(coordinate1);

        ArgumentNullException.ThrowIfNull(coordinate2);

        double phi1 = coordinate1.Latitude * PI / 180.0;
        double phi2 = coordinate2.Latitude * PI / 180.0;
        double lambda1 = coordinate1.Longitude * PI / 180.0;
        double lambda2 = coordinate2.Longitude * PI / 180.0;
        double deltaPhi = Abs(phi2 - phi1);
        double deltaLambda = Abs(lambda2 - lambda1);
        double deltaSigma = 2.0 * Asin(Sqrt(Pow(Sin(deltaPhi / 2.0), 2) + ((1 - Pow(Sin(deltaPhi / 2.0), 2) - Pow(Sin((phi1 + phi2) / 2.0), 2)) * Pow(Sin(deltaLambda / 2.0), 2))));
        double distance2D = EARTH_RADIUS_METER * deltaSigma;
        return distance2D;
    }

    /// <summary>
    /// Calculates the 2D distance between two coordinates using the more precise Vincenty algorithm on the WSG84 Ellipsoid
    ///<para>ported implementation from http://www.movable-type.co.uk/scripts/latlong-vincenty.html</para>
    /// </summary>
    /// <param name="coordinate1">the first coordinate</param>
    /// <param name="coordinate2">the second coordinate</param>
    /// <returns>the 2D distance between the two coordinates in meters</returns>
    /// <exception cref="ArgumentNullException">one of the coordinates is null</exception>
    /// <exception cref="Exception">throws when lambda get greater than PI</exception>
    public static double Calculate2DDistanceVincentyWSG84(Coordinate coordinate1, Coordinate coordinate2)
    {
        ArgumentNullException.ThrowIfNull(coordinate1);

        ArgumentNullException.ThrowIfNull(coordinate2);

        double phi1 = coordinate1.Latitude * PI / 180.0;
        double lambda1 = coordinate1.Longitude * PI / 180.0;
        double phi2 = coordinate2.Latitude * PI / 180;
        double lambda2 = coordinate2.Longitude * PI / 180.0;

        double a = 6378137.0;
        double b = 6356752.314245;
        double f = 1.0 / 298.257223563;

        double l = lambda2 - lambda1;


        double tanU1 = (1.0 - f) * Tan(phi1);
        double cosU1 = 1.0 / Sqrt((1 + tanU1 * tanU1));
        double sinU1 = tanU1 * cosU1;
        double tanU2 = (1.0 - f) * Tan(phi2);
        double cosU2 = 1.0 / Sqrt((1 + tanU2 * tanU2));
        double sinU2 = tanU2 * cosU2;

        bool antipodal = (Abs(l) > (PI / 2.0)) || (Abs(phi2 - phi1) > (PI / 2.0));

        double lambda = l;
        double sinLambda;
        double cosLambda;
        double sigma = antipodal ? PI : 0;
        double sinSigma = 0;
        double cosSigma = antipodal ? -1.0 : 1.0;
        double sinSquareSigma;

        double cos2Sigma_m = 1.0;                      // sigmaₘ = angular distance on the sphere from the equator to the midpoint of the line
        double cosSqureAlpha = 1.0;                      // α = azimuth of the geodesic at the equator
        double lambda_temp;
        int iterations = 0;

        do
        {
            sinLambda = Sin(lambda);
            cosLambda = Cos(lambda);
            sinSquareSigma = Pow(cosU2 * sinLambda, 2) + Pow(cosU1 * sinU2 - sinU1 * cosU2 * cosLambda, 2);


            if (Abs(sinSquareSigma) < 1e-24)
                break;  // co-incident/antipodal points (sigma < ≈0.006mm)
            sinSigma = Sqrt(sinSquareSigma);
            cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
            sigma = Atan2(sinSigma, cosSigma);
            double sinAlpha = cosU1 * cosU2 * sinLambda / sinSigma;
            cosSqureAlpha = 1.0 - sinAlpha * sinAlpha;
            cos2Sigma_m = (Abs(cosSqureAlpha) > double.Epsilon) ? (cosSigma - 2 * sinU1 * sinU2 / cosSqureAlpha) : 0; // on equatorial line cos²α = 0 (§6)
            double c = f / 16.0 * cosSqureAlpha * (4.0 + f * (4.0 - 3.0 * cosSqureAlpha));
            lambda_temp = lambda;
            lambda = l + (1 - c) * f * sinAlpha * (sigma + c * sinSigma * (cos2Sigma_m + c * cosSigma * (-1 + 2 * cos2Sigma_m * cos2Sigma_m)));
            double iterationCheck = antipodal ? Abs(lambda) - PI : Abs(lambda);
            if (iterationCheck > PI)
                throw new Exception("lambda > PI");
        } while (Abs(lambda - lambda_temp) > 1e-12 && ++iterations < 1000); // TV: 'iterate until negligible change in lambda' (≈0.006mm)

        if (iterations >= 1000)
        {
            Logger?.LogWarning("Vincenty did not converge in 1000 iterations, the calculated distance may not be accurate");
        }

        double uSqaure = cosSqureAlpha * (a * a - b * b) / (b * b);
        double upperA = 1.0 + uSqaure / 16384.0 * (4096.0 + uSqaure * (-768.0 + uSqaure * (320.0 - 175.0 * uSqaure)));
        double upperB = uSqaure / 1024.0 * (256.0 + uSqaure * (-128.0 + uSqaure * (74.0 - 47.0 * uSqaure)));
        double deltaSigma = upperB * sinSigma * (cos2Sigma_m + upperB / 4 * (cosSigma * (-1.0 + 2.0 * cos2Sigma_m * cos2Sigma_m) - upperB / 6.0 * cos2Sigma_m * (-3.0 + 4.0 * sinSigma * sinSigma) * (-3.0 + 4.0 * cos2Sigma_m * cos2Sigma_m)));

        double distance2D = b * upperA * (sigma - deltaSigma); // s = length of the geodesic

        // note special handling of exactly antipodal points where sin²sigma = 0 (due to discontinuity
        // atan2(0, 0) = 0 but atan2(ε, 0) = PI/2 / 90°) - in which case bearing is always meridional,
        // due north (or due south!)
        // α = azimuths of the geodesic; α2 the direction P₁ P₂ produced
        //double alpha1 = Abs(sinSquareSigma) < double.Epsilon ? 0 : Atan2(cosU2 * sinLambda, cosU1 * sinU2 - sinU1 * cosU2 * cosLambda);
        //double alpha2= Abs(sinSquareSigma) < double.Epsilon ? PI : Atan2(cosU1 * sinLambda, -sinU1 * cosU2 + cosU1 * sinU2 * cosLambda);

        return distance2D;


    }

    /// <summary>
    /// Calculates the 2D distance between two coordinates using UTM coordinates
    /// <para>the method takes care of the UTM conversation</para>
    /// </summary>
    /// <param name="coordinate1">the first coordinate</param>
    /// <param name="coordinate2">the second coordinate</param>
    /// <returns>the 2D Distance between the two coordinates in meters</returns>
    public static double Calculate2DDistanceUTM(Coordinate coordinate1, Coordinate coordinate2)
    {
        ArgumentNullException.ThrowIfNull(coordinate1);

        ArgumentNullException.ThrowIfNull(coordinate2);


        (_, int easting, int northing) = ConvertLatitudeLongitudeCoordinateToUTM(coordinate1);
        (string zone, int easting, int northing) coordinate2UTM = ConvertLatitudeLongitudeCoordinateToUTM(coordinate2);

        return Sqrt(Pow(easting - coordinate2UTM.easting, 2) + Pow(northing - coordinate2UTM.northing, 2));
    }

    /// <summary>
    /// Calculates the 2D Distance between two Coordinates using precise UTM Coordinates
    /// <para>the method takes care of the UTM conversion</para>
    /// <para>the UTM coordinates are not limited to integers but allow for decimal places as well</para>
    /// </summary>
    /// <param name="coordinate1">the first coordinate</param>
    /// <param name="coordinate2">the second coordinates</param>
    /// <returns>the 2D distance between the two coordiantes in meters</returns>
    public static double Calculate2DDistanceUTM_Precise(Coordinate coordinate1, Coordinate coordinate2)
    {
        ArgumentNullException.ThrowIfNull(coordinate1);

        ArgumentNullException.ThrowIfNull(coordinate2);


        (_, double easting, double northing) = ConvertLatitudeLongitudeCoordinateToUTM_Precise(coordinate1);
        (string zone, double easting, double northing) coordinate2UTM = ConvertLatitudeLongitudeCoordinateToUTM_Precise(coordinate2);

        return Sqrt(Pow(easting - coordinate2UTM.easting, 2) + Pow(northing - coordinate2UTM.northing, 2));
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
        ArgumentNullException.ThrowIfNull(coordinate1);

        ArgumentNullException.ThrowIfNull(coordinate2);

        double distance2D = Calculate2DDistanceHavercos(coordinate1, coordinate2);
        double deltaAltitude;
        if (useGPSAltitude)
        {
            deltaAltitude = coordinate1.AltitudeGPS - coordinate2.AltitudeGPS;
        }
        else
        {
            deltaAltitude = coordinate1.AltitudeBarometric - coordinate2.AltitudeBarometric;
        }

        double distance3D = Sqrt(Pow(distance2D, 2) + Pow(deltaAltitude, 2));
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
        ArgumentNullException.ThrowIfNull(coordinates);

        double result = 0.0;
        for (int index = 0; index < coordinates.Count - 1; index++)
        {
            result += Calculate2DDistanceHavercos(coordinates[index], coordinates[index + 1]);
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
    /// Calculates the distance [m] between two coordinates using a separation altitude to switch between 2D and 3D distance calculation
    /// <para>if the target coordinate is below the separation altitude, the 2D distance will be calculated</para>
    /// <para>if the target coordinate is above the separation altitude, the 3D distance between the target coordinate at separation altitude and the coordinate will be calculated</para>
    /// </summary>
    /// <param name="targetCoordinate">the target coordinate, will be lifted to separation altitude if coordinate is above the separation altitude</param>
    /// <param name="targetCoordinate">the coordinate for which to calculate the distance with respect to the separation altitude</param>
    /// <param name="separationAltitude">the separation altitude in [m]</param>
    /// <param name="useGPSAltitude">true: use GPS altitude; false: use barometric altitude</param>
    /// <returns>the distance in [m]</returns>
    public static double CalculateDistanceWithSeparationAltitude(Coordinate targetCoordinate, Coordinate coordinate, double separationAltitude, bool useGPSAltitude)
    {
        ArgumentNullException.ThrowIfNull(targetCoordinate);

        ArgumentNullException.ThrowIfNull(coordinate);

        if (useGPSAltitude)
        {
            if (coordinate.AltitudeGPS > separationAltitude)
            {
                Coordinate tempCoordinate = new(targetCoordinate.Latitude, targetCoordinate.Longitude, separationAltitude, separationAltitude, targetCoordinate.TimeStamp);
                return Calculate3DDistance(tempCoordinate, coordinate, true);
            }
            else
            {
                return Calculate2DDistanceHavercos(targetCoordinate, coordinate);
            }
        }
        else
        {
            if (coordinate.AltitudeBarometric > separationAltitude)
            {
                Coordinate tempCoordinate = new(targetCoordinate.Latitude, targetCoordinate.Longitude, separationAltitude, separationAltitude, targetCoordinate.TimeStamp);
                return Calculate3DDistance(tempCoordinate, coordinate, false);
            }
            else
            {
                return Calculate2DDistanceHavercos(targetCoordinate, coordinate);
            }
        }
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
        ArgumentNullException.ThrowIfNull(coordinateA);

        ArgumentNullException.ThrowIfNull(coordinateB);

        ArgumentNullException.ThrowIfNull(coordinateC);
        double a = Calculate2DDistanceHavercos(coordinateB, coordinateC);
        double b = Calculate2DDistanceHavercos(coordinateA, coordinateC);
        double c = Calculate2DDistanceHavercos(coordinateA, coordinateB);

        double beta = Acos((Pow(a, 2) + Pow(c, 2) - Pow(b, 2)) / (2 * a * c));
        double result = beta / PI * 180.0;
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
        ArgumentNullException.ThrowIfNull(coordinateA);

        ArgumentNullException.ThrowIfNull(coordinateB);

        ArgumentNullException.ThrowIfNull(coordinateC);
        double a = Calculate2DDistanceHavercos(coordinateB, coordinateC);
        double b = Calculate2DDistanceHavercos(coordinateA, coordinateC);
        double c = Calculate2DDistanceHavercos(coordinateA, coordinateB);

        double halfOfCircumference = (a + b + c) / 2.0;

        double area = Sqrt(halfOfCircumference * (halfOfCircumference - a) * (halfOfCircumference - b) * (halfOfCircumference - c));
        double result = area;

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
        ArgumentNullException.ThrowIfNull(coordinate1);

        if (double.IsNaN(distanceInMeters) || double.IsInfinity(distanceInMeters))
            throw new ArgumentException("Cannot be NaN or Infinity", nameof(distanceInMeters));

        if (double.IsNaN(bearingInDecimalDegree) || double.IsInfinity(bearingInDecimalDegree))
            throw new ArgumentException("Cannot be NaN or Infinity", nameof(bearingInDecimalDegree));

        double angularDistance = Abs(distanceInMeters) / EARTH_RADIUS_METER;
        double lat1 = coordinate1.Latitude * PI / 180.0;
        double long1 = coordinate1.Longitude * PI / 180.0;
        double bearing = (bearingInDecimalDegree % 360.0) * PI / 180.0;

        double latitude = Asin(Sin(lat1) * Cos(angularDistance) + Cos(lat1) * Sin(angularDistance) * Cos(bearing));
        double longitude = long1 + Atan2(Sin(bearing) * Sin(angularDistance) * Cos(lat1), Cos(angularDistance) - Sin(lat1) * Sin(latitude));

        latitude *= 180.0 / PI;
        longitude *= 180.0 / PI;
        Coordinate coordinate = new(latitude, longitude, coordinate1.AltitudeGPS, coordinate1.AltitudeBarometric, DateTime.UtcNow);

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
        ArgumentNullException.ThrowIfNull(coordinate1);

        ArgumentNullException.ThrowIfNull(coordinate2);

        double phi1 = coordinate1.Latitude * PI / 180.0;
        double phi2 = coordinate2.Latitude * PI / 180.0;

        double lambda1 = coordinate1.Longitude * PI / 180.0;
        double lambda2 = coordinate2.Longitude * PI / 180.0;

        double deltaLambda = lambda2 - lambda1;
        double bearing = Atan2(Sin(deltaLambda) * Cos(phi2), Cos(phi1) * Sin(phi2) - Sin(phi1) * Cos(phi2) * Cos(deltaLambda));

        bearing = ((bearing * 180.0 / PI) + 360) % 360;

        return bearing;

    }

    /// <summary>
    /// Convert a position from UTM format to lat/long format
    /// <para>Please note that altitude is set to double.NaN and time stamp to DateTime.MinValue</para>
    /// </summary>
    /// <param name="utmZone">the UTM zone e.g. "32U"</param>
    /// <param name="easting">the easting portion e.g. 630084</param>
    /// <param name="northing">the northing portion e.g. 4833438</param>
    /// <returns>a Coordinate object</returns>
    public static Coordinate ConvertUTMToLatitudeLongitudeCoordinate(string utmZone, int easting, int northing)
    {
        (double latitude, double longitude) = ConvertUTMToLatitudeLongitude(utmZone, easting, northing);
        return new Coordinate(latitude, longitude, double.NaN, double.NaN, DateTime.MinValue);
    }

    /// <summary>
    /// Convert a position from UTM format to lat/long format
    /// <para>Please the time stamp to DateTime.MinValue</para>
    /// </summary>
    /// <param name="utmZone">the UTM zone e.g. "32U"</param>
    /// <param name="easting">the easting portion e.g. 630084</param>
    /// <param name="northing">the northing portion e.g. 4833438</param>
    /// <param name="altitude">the altitude in meters</param>
    /// <returns>a Coordinate object</returns>
    public static Coordinate ConvertUTMToLatitudeLongitudeCoordinate(string utmZone, int easting, int northing, double altitude)
    {
        (double latitude, double longitude) = ConvertUTMToLatitudeLongitude(utmZone, easting, northing);
        return new Coordinate(latitude, longitude, altitude, altitude, DateTime.MinValue);
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
        CoordinateSharp.UniversalTransverseMercator utmCoordindate = new(utmZone, easting, northing);
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
    /// Convert a Coordinate objects latitude and longitude to UTM format
    /// </summary>
    /// <param name="coordinate">the coordinate</param>
    /// <returns>UTM zone and easting / northing using double</returns>
    public static (string utmZone, double easting, double northing) ConvertLatitudeLongitudeCoordinateToUTM_Precise(Coordinate coordinate)
    {
        ArgumentNullException.ThrowIfNull(coordinate);


        return ConvertLatitudeLongitudeToUTM_Presice(coordinate.Latitude, coordinate.Longitude);
    }

    /// <summary>
    /// Converts latitude and longitude to UTM format
    /// <para>easting and northing are not rounded to integer but are floating point numbers instead</para>
    /// </summary>
    /// <param name="latitude">the latitude in decimal degrees</param>
    /// <param name="longitude">the longitude in decimal degrees</param>
    /// <returns>UTM zone,easting and northing rounded to the next integer</returns>
    public static (string utmZone, int easting, int northing) ConvertLatitudeLongitudeToUTM(double latitude, double longitude)
    {
        CoordinateSharp.Coordinate coordinateSharp = new(latitude, longitude);
        return ($"{coordinateSharp.UTM.LongZone}{coordinateSharp.UTM.LatZone}", (int)(Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero)), (int)(Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero)));
    }

    /// <summary>
    /// Converts latitude and longitude to UTM format
    /// <para>easting and northing are not rounded to integer but are floating point numbers instead</para>
    /// </summary>
    /// <param name="latitude">the latitude in decimal degrees</param>
    /// <param name="longitude">the longitude in decimal degrees</param>
    /// <returns>UTM zone and easting / northing using double</returns>
    public static (string utmZone, double easting, double northing) ConvertLatitudeLongitudeToUTM_Presice(double latitude, double longitude)
    {
        CoordinateSharp.Coordinate coordinateSharp = new(latitude, longitude);
        return ($"{coordinateSharp.UTM.LongZone}{coordinateSharp.UTM.LatZone}", coordinateSharp.UTM.Easting, coordinateSharp.UTM.Northing);
    }

    public static (double x, double y, double z) ConvertToCartesian(Coordinate coordinate, bool useGPSAltitude)
    {
        ArgumentNullException.ThrowIfNull(coordinate);

        CoordinateSharp.Coordinate tempCoordiante = new(coordinate.Latitude, coordinate.Longitude);
        CoordinateSharp.ECEF ecefCoordinate = new(tempCoordiante, new CoordinateSharp.Distance(useGPSAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric, CoordinateSharp.DistanceType.Meters));


        return (ecefCoordinate.X, ecefCoordinate.Y, ecefCoordinate.Z);
    }

    public static (double xTransform, double yTransform, double zTransform) HelmertTransformation((double x, double y, double z) input, (double tx, double ty, double tz) translation, (double sx, double sy, double sz) scaling, (double rx, double ry, double rz) rotation)
    {

        //output= input + translation + scaling * input + rotation * input
        //where output, input, translation and scaling are vectors
        //and rotation is a 3x3 matrix with the following values
        //| 0 , -rz, ry|
        //| rz,  0, -rx|
        //|-ry, rx,  0 |

        double xTransform = input.x + translation.tx + scaling.sx * input.x - rotation.rz * input.x + rotation.ry * input.x;
        double yTransform = input.y + translation.ty + scaling.sy * input.y + rotation.rz * input.y - rotation.rx * input.y;
        double zTransform = input.z + translation.tz + scaling.sz * input.z - rotation.ry * input.z + rotation.rx * input.z;

        return (xTransform, yTransform, zTransform);
    }

    public static Coordinate ConvertToLongitudeLatitude(double x, double y, double z)
    {
        CoordinateSharp.ECEF ecefCoordinate = new(x, y, z);
        CoordinateSharp.Coordinate coordinateSharp = CoordinateSharp.ECEF.ECEFToLatLong(ecefCoordinate);
        return new Coordinate(coordinateSharp.Latitude.DecimalDegree, coordinateSharp.Longitude.DecimalDegree, ecefCoordinate.GeoDetic_Height.Meters, ecefCoordinate.GeoDetic_Height.Meters, DateTime.Now);
    }
}
