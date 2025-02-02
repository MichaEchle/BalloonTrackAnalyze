using Scoring.Coordinates;

namespace Scoring.Shapes;

public class Polygon : Shapes2D
{
    public required List<Coordinate> PolygonPoints
    {
        get; init;
    }

    private static double IsLeft(Coordinate coordinate1, Coordinate coordinate2, Coordinate coordinate3)
    {
        return (coordinate2.Longitude - coordinate1.Longitude) * (coordinate3.Latitude - coordinate1.Latitude) - (coordinate3.Longitude - coordinate1.Longitude) * (coordinate2.Latitude - coordinate1.Latitude);
    }

    private int CalculateWindingNumber(Coordinate coordinate)
    {
        int windingNumber = 0;
        for (int index = 0; index < PolygonPoints.Count; index++)
        {
            if (GetPolygonPointWrappedAround(index).Latitude <= coordinate.Latitude)
            {
                if (GetPolygonPointWrappedAround(index + 1).Latitude > coordinate.Latitude)
                {
                    if (IsLeft(GetPolygonPointWrappedAround(index), GetPolygonPointWrappedAround(index + 1), coordinate) > 0)
                    {
                        windingNumber++;
                    }
                }
            }
            else
            {
                if (GetPolygonPointWrappedAround(index + 1).Latitude <= coordinate.Latitude)
                {
                    if (IsLeft(GetPolygonPointWrappedAround(index), GetPolygonPointWrappedAround(index + 1), coordinate) < 0)
                    {
                        windingNumber--;
                    }
                }
            }
        }
        return windingNumber;
    }

    private Coordinate GetPolygonPointWrappedAround(int index)
    {
        return PolygonPoints[index % PolygonPoints.Count];
    }

    override public bool IsCoordinateInside(Coordinate coordinate)
    {
        return (CalculateWindingNumber(coordinate) != 0);
    }

    public double CalculateArea()
    {
        double area = 0.0;
        for (int index = 0; index < PolygonPoints.Count; index++)
        {
            area += (GetPolygonPointWrappedAround(index).Longitude * GetPolygonPointWrappedAround(index + 1).Latitude) - (GetPolygonPointWrappedAround(index + 1).Longitude * GetPolygonPointWrappedAround(index).Latitude);
        }
        return Math.Abs(area) / 2;
    }
}