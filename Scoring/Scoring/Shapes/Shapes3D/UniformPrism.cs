using Scoring.Coordinates;

namespace Scoring.Shapes;

public class UniformPrism : Shapes3D
{
    public required Polygon Polygon
    {
        get; init;
    }

    public required double LowerBoundary
    {
        get; init;
    }

    public required double UpperBoundary
    {
        get; init;
    }

    public override bool IsCoordinateInside(Coordinate coordinate)
    {
        if (!Polygon.IsCoordinateInside(coordinate))
        {
            return false;
        }

        return base.IsWithinAltitudeBoundary(coordinate, LowerBoundary, UpperBoundary);
    }
}

