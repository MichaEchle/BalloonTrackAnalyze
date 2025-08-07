using Coordinates;

namespace Shapes.Shapes3D;

public class Block : Shapes3D
{
    public Coordinate FirstCorner
    {
        get;
        private set;
    }

    public Coordinate SecondCorner
    {
        get;
        private set;
    }

    public Block(Coordinate firstCorner, Coordinate secondCorner)
    {
        FirstCorner = firstCorner;
        SecondCorner = secondCorner;
    }

    public override bool IsWithin(Coordinate point, bool useGPSAltitude)
    {
        double minLat = Math.Min(FirstCorner.Latitude, SecondCorner.Latitude);
        double maxLat = Math.Max(FirstCorner.Latitude, SecondCorner.Latitude);

        double minLong = Math.Min(FirstCorner.Longitude, SecondCorner.Longitude);
        double maxLong = Math.Max(FirstCorner.Longitude, SecondCorner.Longitude);

        double minAltitude = Math.Min((useGPSAltitude ? FirstCorner.AltitudeGPS : FirstCorner.AltitudeBarometric),
            (useGPSAltitude ? SecondCorner.AltitudeGPS : SecondCorner.AltitudeBarometric));
        double maxAltitude = Math.Max((useGPSAltitude ? FirstCorner.AltitudeGPS : FirstCorner.AltitudeBarometric),
            (useGPSAltitude ? SecondCorner.AltitudeGPS : SecondCorner.AltitudeBarometric));

        bool isLatWithinRange = point.Latitude >= minLat && point.Latitude <= maxLat;

        bool isLongWithinRange = point.Longitude >= minLong && point.Longitude <= maxLong;
        
        bool isAltitudeWithinRange = point.AltitudeGPS >= minAltitude && point.AltitudeGPS <= maxAltitude;

        return isLatWithinRange && isLongWithinRange && isAltitudeWithinRange;
    }
}