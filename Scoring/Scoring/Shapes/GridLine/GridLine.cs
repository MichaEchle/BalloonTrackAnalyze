using Scoring.Coordinates;

namespace Scoring.Shapes.GridLine;

/// <summary>
/// Provides utilities for testing proximity to cardinal grid lines (north-south and east-west).
/// </summary>
/// <remarks>
/// Grid lines follow lines of constant latitude (parallels) and longitude (meridians).
/// Distance is measured in meters via Haversine formula.
/// </remarks>
internal class GridLine
{
    /// <summary>
    /// Gets the center point that defines the grid lines.
    /// </summary>
    /// <remarks>
    /// The north-south grid line passes through this center at its longitude.
    /// The east-west grid line passes through this center at its latitude.
    /// </remarks>
    public required Coordinate GridCenter
    {
        get; init;
    }

    /// <summary>
    /// Determines whether the specified point is north of the grid center.
    /// </summary>
    /// <param name="point">The coordinate to test.</param>
    /// <returns>
    /// <see langword="true"/> if the point's latitude is greater than the grid center's latitude;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsNorthOfGridCenter(Coordinate point)
    {
        return point.Latitude > GridCenter.Latitude;
    }

    /// <summary>
    /// Determines whether the specified point is south of the grid center.
    /// </summary>
    /// <param name="point">The coordinate to test.</param>
    /// <returns>
    /// <see langword="true"/> if the point's latitude is less than the grid center's latitude;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsSouthOfGridCenter(Coordinate point)
    {
        return point.Latitude < GridCenter.Latitude;
    }

    /// <summary>
    /// Determines whether the specified point is east of the grid center.
    /// </summary>
    /// <param name="point">The coordinate to test.</param>
    /// <returns>
    /// <see langword="true"/> if the point's longitude is greater than the grid center's longitude;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsEastOfGridCenter(Coordinate point)
    {
        return point.Longitude > GridCenter.Longitude;
    }

    /// <summary>
    /// Determines whether the specified point is west of the grid center.
    /// </summary>
    /// <param name="point">The coordinate to test.</param>
    /// <returns>
    /// <see langword="true"/> if the point's longitude is less than the grid center's longitude;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsWestOfGridCenter(Coordinate point)
    {
        return point.Longitude < GridCenter.Longitude;
    }

    /// <summary>
    /// Computes the distance in meters from the specified point to the north-south grid line.
    /// </summary>
    /// <remarks>
    /// The north-south grid line is a meridian (line of constant longitude) passing through the grid center.
    /// Distance is calculated using the 2D Haversine formula and represents the ground distance in meters.
    /// </remarks>
    /// <param name="point">The coordinate to measure from.</param>
    /// <returns>Distance in meters to the north-south grid line.</returns>
    public double Calculate2DDistanceToNorthSouthGridLine(Coordinate point)
    {
        Coordinate tempGridCenter = new()
        {
            Altitude = point.Altitude,
            Latitude = GridCenter.Latitude,
            Longitude = point.Longitude
        };

        return CoordinateMath.Calculate2DDistance(point, tempGridCenter);
    }

    /// <summary>
    /// Computes the distance in meters from the specified point to the east-west grid line.
    /// </summary>
    /// <remarks>
    /// The east-west grid line is a parallel (line of constant latitude) passing through the grid center.
    /// Distance is calculated using the 2D Haversine formula and represents the ground distance in meters.
    /// </remarks>
    /// <param name="point">The coordinate to measure from.</param>
    /// <returns>Distance in meters to the east-west grid line.</returns>
    public double Calculate2DDistanceToEastWestGridLine(Coordinate point)
    {
        Coordinate tempGridCenter = new()
        {
            Altitude = point.Altitude,
            Latitude = point.Latitude,
            Longitude = GridCenter.Longitude
        };

        return CoordinateMath.Calculate2DDistance(point, tempGridCenter);
    }
}
