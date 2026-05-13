using Scoring.Coordinates;

namespace Scoring.Shapes;

/// <summary>
/// Represents a uniform prism: a polygon in 2D extruded vertically with lower and upper altitude boundaries.
/// </summary>
/// <remarks>
/// A coordinate is inside if it lies within the polygonal base and between altitude bounds.
/// Altitude bounds use <see cref="double.NaN"/> to represent "no limit".
/// </remarks>
public class UniformPrism : Shapes3D
{
    /// <summary>
    /// Gets the polygonal base of this prism.
    /// </summary>
    public required Polygon Polygon
    {
        get; init;
    }

    /// <summary>
    /// Gets the lower altitude boundary in meters, or <see cref="double.NaN"/> for no limit.
    /// </summary>
    public required double LowerBoundary
    {
        get; init;
    }

    /// <summary>
    /// Gets the upper altitude boundary in meters, or <see cref="double.NaN"/> for no limit.
    /// </summary>
    public required double UpperBoundary
    {
        get; init;
    }

    /// <inheritdoc/>
    public override bool IsCoordinateInside(Coordinate coordinate)
    {
        if (!Polygon.IsCoordinateInside(coordinate))
        {
            return false;
        }

        return base.IsWithinAltitudeBoundary(coordinate, LowerBoundary, UpperBoundary);
    }
}

