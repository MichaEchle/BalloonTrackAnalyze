using Coordinates;

namespace Shapes.Shapes2D;

public class Rectangle : Shapes2D
{
    public Coordinate FirstBoundary
    {
        get; private set;
    }
    
    public Coordinate SecondBoundary
    {
        get; private set;
    }

    public Rectangle(Coordinate firstBoundary, Coordinate secondBoundary)
    {
        FirstBoundary = firstBoundary;
        SecondBoundary = secondBoundary;
    }

    public override bool IsWithin(Coordinate point)
    {
        double minLat = Math.Min(FirstBoundary.Latitude, SecondBoundary.Latitude);
        double maxLat = Math.Max(FirstBoundary.Latitude, SecondBoundary.Latitude);

        double minLong = Math.Min(FirstBoundary.Longitude, SecondBoundary.Longitude);
        double maxLong = Math.Max(FirstBoundary.Longitude, SecondBoundary.Longitude);

        bool isLatWithinRange = point.Latitude >= minLat && point.Latitude <= maxLat;

        bool isLongWithinRange = point.Longitude >= minLong && point.Longitude <= maxLong;

        return isLatWithinRange && isLongWithinRange;
    }
}