using Coordinates;

namespace Shapes;
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
        if (!Polygon.IsWithin(coordinate))
            return false;
        return base.IsWithinAltitudeBoundary(coordinate, useGPSAltitude, LowerBoundary, UpperBoundary);
    }
}
