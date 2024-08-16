using Coordinates;
using System.Collections.Generic;

namespace Shapes;

public class Polygon : Shapes2D
{
    public List<Coordinate> PolygonPoints
    {
        get; private set;
    }

    public Polygon(List<Coordinate> polygonPoints)
    {
        PolygonPoints = polygonPoints;
    }

    private double IsLeft(Coordinate coordinate1, Coordinate coordinate2, Coordinate coordinate3)
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
                        windingNumber++;
                }
            }
            else
            {
                if (GetPolygonPointWrappedAround(index + 1).Latitude <= coordinate.Latitude)
                {
                    if (IsLeft(GetPolygonPointWrappedAround(index), GetPolygonPointWrappedAround(index + 1), coordinate) < 0)
                        windingNumber--;
                }
            }
        }
        return windingNumber;
    }

    private Coordinate GetPolygonPointWrappedAround(int index)
    {
        return PolygonPoints[index % PolygonPoints.Count];
    }

    public override bool IsWithin(Coordinate coordinate)
    {
        return (CalculateWindingNumber(coordinate) != 0);
    }



}
