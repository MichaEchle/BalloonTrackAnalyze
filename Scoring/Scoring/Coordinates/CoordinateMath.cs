using Scoring.Competitions;
using static System.Math;

namespace Scoring.Coordinates;

public static class CoordinateMath
{
    /// <summary>
    /// Calculate the 2D distance [m] between the two coordinates using havercos formula
    /// </summary>
    /// <param name="coordinate1">the first coordinate</param>
    /// <param name="coordinate2">the second coordinate</param>
    /// <returns>the distance in meters</returns>
    public static double Calculate2DDistance(Coordinate coordinate1, Coordinate coordinate2)
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
        double distance2D = 2.0 * Competition.Instance.Ellipsoid.EquatorialRadius * Asin(Sqrt(a));
        return distance2D;
    }


    /// <summary>
    /// Calculate the 3D distance [m] between the two coordinates using havercos for 2D distance and Euclid for 3D distance
    /// </summary>
    /// <param name="coordinate1">the first coordinate</param>
    /// <param name="coordinate2">the second coordinate</param>
    /// <returns>the 3D distance in meters</returns>
    public static double Calculate3DDistance(Coordinate coordinate1, Coordinate coordinate2)
    {
        ArgumentNullException.ThrowIfNull(coordinate1);

        ArgumentNullException.ThrowIfNull(coordinate2);

        double distance2D = Calculate2DDistance(coordinate1, coordinate2);
        double deltaAltitude = coordinate1.Altitude - coordinate2.Altitude;

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
    public static double Calculate3DDistanceBetweenPoints(List<Coordinate> coordinates)
    {
        double result = 0.0;
        for (int index = 0; index < coordinates.Count - 1; index++)
        {
            result += Calculate3DDistance(coordinates[index], coordinates[index + 1]);
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

        if (coordinate.Altitude > separationAltitude)
        {
            Coordinate tempCoordinate = new()
            {
                Latitude = targetCoordinate.Latitude,
                Longitude = targetCoordinate.Longitude,
                Altitude = separationAltitude,
                TimeStamp = targetCoordinate.TimeStamp
            };
            return Calculate3DDistance(tempCoordinate, coordinate);
        }
        else
        {
            return Calculate2DDistance(targetCoordinate, coordinate);
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
        double a = Calculate2DDistance(coordinateB, coordinateC);
        double b = Calculate2DDistance(coordinateA, coordinateC);
        double c = Calculate2DDistance(coordinateA, coordinateB);

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
        double a = Calculate2DDistance(coordinateB, coordinateC);
        double b = Calculate2DDistance(coordinateA, coordinateC);
        double c = Calculate2DDistance(coordinateA, coordinateB);

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
        {
            throw new ArgumentException("Cannot be NaN or Infinity", nameof(distanceInMeters));
        }

        if (double.IsNaN(bearingInDecimalDegree) || double.IsInfinity(bearingInDecimalDegree))
        {
            throw new ArgumentException("Cannot be NaN or Infinity", nameof(bearingInDecimalDegree));
        }

        double angularDistance = Abs(distanceInMeters) / Competition.Instance.Ellipsoid.EquatorialRadius;
        double lat1 = coordinate1.Latitude * PI / 180.0;
        double long1 = coordinate1.Longitude * PI / 180.0;
        double bearing = (bearingInDecimalDegree % 360.0) * PI / 180.0;

        double latitude = Asin(Sin(lat1) * Cos(angularDistance) + Cos(lat1) * Sin(angularDistance) * Cos(bearing));
        double longitude = long1 + Atan2(Sin(bearing) * Sin(angularDistance) * Cos(lat1), Cos(angularDistance) - Sin(lat1) * Sin(latitude));

        latitude *= 180.0 / PI;
        longitude *= 180.0 / PI;
        Coordinate coordinate = new()
        {
            Latitude = latitude,
            Longitude = longitude,
            Altitude = coordinate1.Altitude,
            TimeStamp = DateTime.UtcNow
        };

        return coordinate;
    }
}
