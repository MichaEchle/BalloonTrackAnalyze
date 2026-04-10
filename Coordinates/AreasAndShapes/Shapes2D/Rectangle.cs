using Coordinates;

namespace Shapes.Shapes2D;

public class Rectangle : Shapes2D
{
    public Coordinate FirstCorner
    {
        get; private set;
    }
    
    public Coordinate SecondCorner
    {
        get; private set;
    }

    public Rectangle(Coordinate firstCorner, Coordinate secondCorner)
    {
        FirstCorner = firstCorner;
        SecondCorner = secondCorner;
    }

    public override bool IsWithin(Coordinate point)
    {
        double minLat = Math.Min(FirstCorner.Latitude, SecondCorner.Latitude);
        double maxLat = Math.Max(FirstCorner.Latitude, SecondCorner.Latitude);

        double minLong = Math.Min(FirstCorner.Longitude, SecondCorner.Longitude);
        double maxLong = Math.Max(FirstCorner.Longitude, SecondCorner.Longitude);

        bool isLatWithinRange = point.Latitude >= minLat && point.Latitude <= maxLat;

        bool isLongWithinRange = point.Longitude >= minLong && point.Longitude <= maxLong;

        return isLatWithinRange && isLongWithinRange;
    }
}