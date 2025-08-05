using Coordinates;
using Shapes.Shapes2D;

namespace Shapes.Shapes3D;
public class UniformPrism : Shapes3D
{
    public Polygon Polygon
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

    public UniformPrism(Polygon polygon, double lowerBoundary, double upperBoundary)
    {
        Polygon = polygon;
        LowerBoundary = lowerBoundary;
        UpperBoundary = upperBoundary;
    }

    public override bool IsWithin(Coordinate coordinate, bool useGPSAltitude)
    {
        return !Polygon.IsWithin(coordinate)
            ? false
            : base.IsWithinAltitudeBoundary(coordinate, useGPSAltitude, LowerBoundary, UpperBoundary);
    }
}
