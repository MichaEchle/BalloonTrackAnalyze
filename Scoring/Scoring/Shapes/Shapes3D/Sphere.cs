using Scoring.Coordinates;

namespace Scoring.Shapes;

/// <summary>
/// Represents a spherical volume with a center coordinate and radius.
/// </summary>
/// <remarks>
/// Containment is tested using 3D Euclidean distance from the center.
/// </remarks>
public class Sphere : Shapes3D
{
    /// <summary>
    /// Gets the center coordinate of this sphere.
    /// </summary>
    public required Coordinate CenterPoint
    {
        get; init;
    }

    /// <summary>
    /// Gets the radius of this sphere in meters.
    /// </summary>
    public required double Radius
    {
        get; init;
    }

    /// <inheritdoc/>
    public override bool IsCoordinateInside(Coordinate coordinate)
    {
        return CoordinateMath.Calculate3DDistance(CenterPoint, coordinate) <= Radius;
    }
}
