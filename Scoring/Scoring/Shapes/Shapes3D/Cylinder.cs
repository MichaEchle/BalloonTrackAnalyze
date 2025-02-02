using Scoring.Coordinates;

namespace Scoring.Shapes;

public class Cylinder : Shapes3D
{
    public required Circle Circle
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
        if (!Circle.IsCoordinateInside(coordinate))
        {
            return false;
        }

        return IsWithinAltitudeBoundary(coordinate,  LowerBoundary, UpperBoundary);
    }

}
