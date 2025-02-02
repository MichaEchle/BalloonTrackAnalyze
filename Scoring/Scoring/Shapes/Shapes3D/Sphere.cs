using Scoring.Coordinates;

namespace Scoring.Shapes;

public class Sphere : Shapes3D
{
    public required Coordinate CenterPoint
    {
        get; init;
    }

    public required double Radius
    {
        get; init;
    }

    public override bool IsCoordinateInside(Coordinate coordinate)
    {
        return CoordinateMath.Calculate3DDistance(CenterPoint, coordinate) <= Radius;
    }
}
