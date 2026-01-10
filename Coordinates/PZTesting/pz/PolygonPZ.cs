using Coordinates;
using JansScoring.plt;

namespace PZTesting;

public class PolygonPZ : PZ
{
    private List<Coordinate> polygon;

    private double minHeight, maxHeight;


    private double northernmost, easternmost, southernmost, westernmost;

    public PolygonPZ(String pltFilePath, double minHeight, double maxHeight)
    {
        polygon = PLTParser.Parse(pltFilePath);
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        Console.WriteLine($"Loaded {polygon.Count} corners from PLT-File");

        if (polygon.Count == 0)
        {
            return;
        }

        northernmost = polygon[0].Latitude;
        easternmost = polygon[0].Longitude;
        southernmost = polygon[0].Latitude;
        westernmost = polygon[0].Longitude;
        foreach (Coordinate coordinate in polygon)
        {
            if (northernmost < coordinate.Latitude)
            {
                northernmost = coordinate.Latitude;
            }

            if (easternmost < coordinate.Longitude)
            {
                easternmost = coordinate.Longitude;
            }

            if (southernmost > coordinate.Latitude)
            {
                southernmost = coordinate.Latitude;
            }

            if (westernmost > coordinate.Longitude)
            {
                westernmost = coordinate.Longitude;
            }
        }
    }


    public bool IsInsidePz(bool useGPSAltitude, Coordinate coordinate, out double infringement)
    {
        double altitude = useGPSAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric;
        if (altitude < minHeight || altitude > maxHeight)
        {
            infringement = 0;
            return false;
        }

        if (coordinate.Longitude > easternmost || coordinate.Longitude < westernmost ||
            coordinate.Latitude > northernmost || coordinate.Latitude < southernmost)
        {
            infringement = 0;
            return false;
        }


        int polygonLength = polygon.Count;
        int i, j = polygonLength - 1;
        bool oddNodes = false;

        for (i = 0; i < polygonLength; i++)
        {
            if (polygon[i].Latitude < coordinate.Latitude && polygon[j].Latitude >= coordinate.Latitude
                || polygon[j].Latitude < coordinate.Latitude && polygon[i].Latitude >= coordinate.Latitude)
            {
                if (polygon[i].Longitude + (coordinate.Latitude - polygon[i].Latitude) /
                    (polygon[j].Latitude - polygon[i].Latitude) *
                    (polygon[j].Longitude - polygon[i].Longitude) < coordinate.Longitude)
                {
                    oddNodes = !oddNodes;
                }
            }

            j = i;
        }

        infringement = 0;
        return oddNodes;
    }
}