using Scoring.Coordinates;

namespace Scoring.Shapes.GridLine;

internal class GridLine
{
    public required Coordinate GridCenter
    {
        get; init;
    }

    public bool IsNorthOfGridCenter(Coordinate point)
    {
        return point.Latitude > GridCenter.Latitude;
    }

    public bool IsSouthOfGridCenter(Coordinate point)
    {
        return point.Latitude < GridCenter.Latitude;
    }

    public bool IsEastOfGridCenter(Coordinate point)
    {
        return point.Longitude > GridCenter.Longitude;
    }

    public bool IsWestOfGridCenter(Coordinate point)
    {
        return point.Longitude < GridCenter.Longitude;
    }

    public double Calculate2DDistanceToNorthSouthGridLine(Coordinate point)
    {
        Coordinate tempGridCenter = new()
        {
            Altitude = point.Altitude,
            Latitude = GridCenter.Latitude,
            Longitude = point.Longitude
        };

        return CoordinateMath.Calculate2DDistance(point, tempGridCenter);
    }

    public double Calculate2DDistanceToEastWestGridLine(Coordinate point)
    {
        Coordinate tempGridCenter = new()
        {
            Altitude = point.Altitude,
            Latitude = point.Latitude,
            Longitude = GridCenter.Longitude
        };

        return CoordinateMath.Calculate2DDistance(point, tempGridCenter);
    }
}
