using Scoring.Competitions;
using static System.Math;

namespace Scoring.Coordinates;

/// <summary>
/// Provides geodesic math operations for WGS84 coordinates.
/// </summary>
/// <remarks>
/// All distance calculations use the Haversine formula and treat the Earth as a sphere.
/// For higher precision over long distances, consider an ellipsoidal model.
/// All angles are measured in degrees.
/// </remarks>
public static class CoordinateMath
{
    /// <summary>
    /// Computes the great-circle distance in meters between two coordinates using the Haversine formula.
    /// </summary>
    /// <param name="coordinate1">The origin coordinate.</param>
    /// <param name="coordinate2">The destination coordinate.</param>
    /// <returns>The 2D (surface) distance in meters.</returns>
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
    /// Computes the 3D distance in meters between two coordinates, including altitude difference.
    /// </summary>
    /// <param name="coordinate1">The origin coordinate.</param>
    /// <param name="coordinate2">The destination coordinate.</param>
    /// <returns>The 3D Euclidean distance in meters.</returns>
    /// <remarks>
    /// The 2D distance is computed using the Haversine formula, and then combined with the
    /// altitude difference using the Pythagorean theorem.
    /// </remarks>
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
    /// Computes the accumulated 2D distance in meters between consecutive coordinates.
    /// </summary>
    /// <param name="coordinates">The list of coordinates. Must be in order along the desired path.</param>
    /// <returns>The accumulated 2D (surface) distance in meters.</returns>
    /// <remarks>
    /// Ensure the coordinates are sorted accordingly to reflect the actual path traveled.
    /// </remarks>
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
    /// Computes the accumulated 3D distance in meters between consecutive coordinates.
    /// </summary>
    /// <param name="coordinates">The list of coordinates. Must be in order along the desired path.</param>
    /// <returns>The accumulated 3D Euclidean distance in meters.</returns>
    /// <remarks>
    /// Ensure the coordinates are sorted accordingly to reflect the actual path traveled.
    /// </remarks>
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
    /// Computes the distance between two coordinates, using a separation altitude to switch between 2D and 3D distance calculation.
    /// </summary>
    /// <param name="targetCoordinate">The target coordinate. Will be lifted to separation altitude if above the separation altitude for 3D calculation.</param>
    /// <param name="coordinate">The coordinate for which to calculate the distance with respect to the separation altitude.</param>
    /// <param name="separationAltitude">The separation altitude in meters.</param>
    /// <returns>The distance in meters.</returns>
    /// <remarks>
    /// If the target coordinate is below the separation altitude, the 2D distance is calculated.
    /// If the target coordinate is at or above the separation altitude, the 3D distance is calculated
    /// with the target coordinate projected to the separation altitude.
    /// </remarks>
    public static double CalculateDistanceWithSeparationAltitude(Coordinate targetCoordinate, Coordinate coordinate, double separationAltitude)
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
    /// Computes the interior angle in degrees at <paramref name="coordinateB"/> where the route is defined from <paramref name="coordinateA"/> to <paramref name="coordinateB"/> to <paramref name="coordinateC"/>.
    /// </summary>
    /// <param name="coordinateA">The first coordinate.</param>
    /// <param name="coordinateB">The vertex coordinate where the angle is measured.</param>
    /// <param name="coordinateC">The third coordinate.</param>
    /// <returns>The interior angle in degrees, in the range [0, 180].</returns>
    /// <remarks>
    /// This uses the law of cosines to compute the angle between the two paths.
    /// </remarks>
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
    /// Computes the area in square meters of the triangle defined by three coordinates.
    /// </summary>
    /// <param name="coordinateA">The first coordinate of the triangle.</param>
    /// <param name="coordinateB">The second coordinate of the triangle.</param>
    /// <param name="coordinateC">The third coordinate of the triangle.</param>
    /// <returns>The area in square meters.</returns>
    /// <remarks>
    /// This uses Heron's formula applied to the 2D distances between the coordinates.
    /// </remarks>
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
    /// Computes a coordinate at a given distance and bearing from a starting point.
    /// </summary>
    /// <param name="coordinate1">The starting coordinate.</param>
    /// <param name="distanceInMeters">The distance in meters. Must not be <see cref="double.NaN"/> or <see cref="double.PositiveInfinity"/>.</param>
    /// <param name="bearingInDecimalDegree">The bearing in decimal degrees. Must not be <see cref="double.NaN"/> or <see cref="double.PositiveInfinity"/>.</param>
    /// <returns>A new coordinate at the specified distance and bearing from the starting point.</returns>
    /// <remarks>
    /// The altitude of the returned coordinate is copied from the starting coordinate.
    /// The timestamp is set to the current UTC time at the time of calculation.
    /// The bearing is normalized to the range [0, 360).
    /// </remarks>
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
