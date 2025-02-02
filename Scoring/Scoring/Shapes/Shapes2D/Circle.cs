using Scoring.Coordinates;

namespace Scoring.Shapes;

public class Circle : Shapes2D
{
    public required Coordinate CenterPoint
    {
        get; init;
    }

    public required double Radius
    {
        get; init;
    }


    override public bool IsCoordinateInside(Coordinate coordinate)
    {
        return CoordinateMath.Calculate2DDistance(CenterPoint, coordinate) <= Radius;
    }
}
