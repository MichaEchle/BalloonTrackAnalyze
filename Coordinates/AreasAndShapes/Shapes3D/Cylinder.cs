using Coordinates;

namespace Shapes;

public class Cylinder : Shapes3D
{
    public Circle Circle
    {
        get; private set;
    }

    public double LowerBoundary
    {
        get; private set;
    }

    public double UpperBoundary
    {
        get; private set;
    }

    public Cylinder(Circle circle, double lowerBoundary, double upperBoundary)
    {
        Circle = circle;
        LowerBoundary = lowerBoundary;
        UpperBoundary = upperBoundary;
    }

    public override bool IsWithin(Coordinate coordinate, bool useGPSAltitude)
    {
        if (!Circle.IsWithin(coordinate))
            return false;
        return base.IsWithinAltitudeBoundary(coordinate, useGPSAltitude,LowerBoundary,UpperBoundary);
    }

}
