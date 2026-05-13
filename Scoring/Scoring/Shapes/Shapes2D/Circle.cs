using Scoring.Coordinates;

namespace Scoring.Shapes;

/// <summary>
/// Represents a circle in 2D space with a center coordinate and radius.
/// </summary>
/// <remarks>
/// The circle is defined by a <see cref="CenterPoint"/> coordinate and <see cref="Radius"/> in meters.
/// Containment is tested using Haversine distance.
/// </remarks>
public class Circle : Shapes2D
{
    /// <summary>
    /// Gets the center coordinate of this circle.
    /// </summary>
    public required Coordinate CenterPoint
    {
        get; init;
    }

    /// <summary>
    /// Gets the radius of this circle in meters.
    /// </summary>
    /// <value>Radius in meters; must be non-negative.</value>
    public required double Radius
    {
        get; init;
    }


    /// <inheritdoc/>
    override public bool IsCoordinateInside(Coordinate coordinate)
    {
        return CoordinateMath.Calculate2DDistance(CenterPoint, coordinate) <= Radius;
    }
}
