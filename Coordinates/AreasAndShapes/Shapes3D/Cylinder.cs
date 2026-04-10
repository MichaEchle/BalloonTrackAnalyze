using Coordinates;
using Shapes.Shapes2D;

namespace Shapes.Shapes3D;

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
        return !Circle.IsWithin(coordinate) ? false : base.IsWithinAltitudeBoundary(coordinate, useGPSAltitude, LowerBoundary, UpperBoundary);
    }

}
