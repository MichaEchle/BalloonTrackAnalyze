using Coordinates;

namespace Shapes.Shapes3D;

public class Block : Shapes3D
{
    public Coordinate FirstBoundary
    {
        get;
        private set;
    }

    public Coordinate SecondBoundary
    {
        get;
        private set;
    }

    public Block(Coordinate firstBoundary, Coordinate secondBoundary)
    {
        FirstBoundary = firstBoundary;
        SecondBoundary = secondBoundary;
    }

    public override bool IsWithin(Coordinate point, bool useGPSAltitude)
    {
        double minLat = Math.Min(FirstBoundary.Latitude, SecondBoundary.Latitude);
        double maxLat = Math.Max(FirstBoundary.Latitude, SecondBoundary.Latitude);

        double minLong = Math.Min(FirstBoundary.Longitude, SecondBoundary.Longitude);
        double maxLong = Math.Max(FirstBoundary.Longitude, SecondBoundary.Longitude);

        double minAltitude = Math.Min((useGPSAltitude ? FirstBoundary.AltitudeGPS : FirstBoundary.AltitudeBarometric),
            (useGPSAltitude ? SecondBoundary.AltitudeGPS : SecondBoundary.AltitudeBarometric));
        double maxAltitude = Math.Max((useGPSAltitude ? FirstBoundary.AltitudeGPS : FirstBoundary.AltitudeBarometric),
            (useGPSAltitude ? SecondBoundary.AltitudeGPS : SecondBoundary.AltitudeBarometric));

        bool isLatWithinRange = point.Latitude >= minLat && point.Latitude <= maxLat;

        bool isLongWithinRange = point.Longitude >= minLong && point.Longitude <= maxLong;
        
        bool isAltitudeWithinRange = point.AltitudeGPS >= minAltitude && point.AltitudeGPS <= maxAltitude;

        return isLatWithinRange && isLongWithinRange && isAltitudeWithinRange;
    }
}