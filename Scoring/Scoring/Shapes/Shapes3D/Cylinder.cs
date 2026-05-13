using Scoring.Coordinates;

namespace Scoring.Shapes;

/// <summary>
/// Represents a cylindrical volume: a circle in 2D with lower and upper altitude boundaries.
/// </summary>
/// <remarks>
/// A coordinate is inside if it lies within the circular base and between altitude bounds.
/// Altitude bounds use <see cref="double.NaN"/> to represent "no limit".
/// </remarks>
public class Cylinder : Shapes3D
{
    /// <summary>
    /// Gets the circular base of this cylinder.
    /// </summary>
    public required Circle Circle
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
        if (!Circle.IsCoordinateInside(coordinate))
        {
            return false;
        }

        return IsWithinAltitudeBoundary(coordinate,  LowerBoundary, UpperBoundary);
    }

}
