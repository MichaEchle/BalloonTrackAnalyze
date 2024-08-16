using Coordinates;

namespace Shapes;

public class Circle : Shapes2D
{
    public Coordinate CenterPoint
    {
        get; private set;
    }

    public double Radius
    {
        get; private set;
    }

    public Circle(Coordinate centerPoint, double radius)
    {
        CenterPoint = centerPoint;
        Radius = radius;
    }

    public override bool IsWithin(Coordinate coordinate)
    {
        return CoordinateHelpers.Calculate2DDistanceHavercos(CenterPoint, coordinate) <= Radius;
    }
}
