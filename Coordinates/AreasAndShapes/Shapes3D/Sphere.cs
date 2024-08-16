using Coordinates;

namespace Shapes;

public class Sphere : Shapes3D
{
    public Coordinate CenterPoint
    {
        get; private set;
    }

    public double Radius
    {
        get; private set;
    }

    public Sphere(Coordinate centerPoint, double radius)
    {
        CenterPoint = centerPoint;
        Radius = radius;
    }

    public override bool IsWithin(Coordinate coordinate, bool useGPSAltitude)
    {
        return CoordinateHelpers.Calculate3DDistance(CenterPoint, coordinate, useGPSAltitude) <= Radius;
    }
}
