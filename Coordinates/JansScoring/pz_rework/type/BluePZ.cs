using Coordinates;
using JansScoring.flights;
using JansScoring.plt;
using System;
using System.Collections.Generic;

namespace JansScoring.pz_rework.type;

public class BluePZ : PZ
{

    private List<Coordinate> polygon;

    private int minHeight, maxHeight;

    public BluePZ(int id, String pltFilePath, int minHeight, int maxHeight) : base(id)
    {
        polygon = PLTParser.parse(pltFilePath);
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        Console.WriteLine($"Loaded {polygon.Count} corners from PLT-File");
    }

    public override bool IsInsidePz(Flight flight, Track track, Coordinate coordinate, out string comment)
    {
        comment = "";
        double altitude = flight.useGPSAltitude() ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric;
        if (altitude < minHeight || altitude > maxHeight)
        {
            comment = "Out of height";
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

        return oddNodes;
    }
}