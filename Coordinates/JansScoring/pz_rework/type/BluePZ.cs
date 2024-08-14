using Coordinates;
using JansScoring.flights;
using JansScoring.plt;
using System;
using System.Collections.Generic;

namespace JansScoring.pz_rework.type;

public class BluePZ : PZ
{
    private List<Coordinate> polygon;

    private double minHeight, maxHeight;


    private double northernmost, easternmost, southernmost, westernmost;

    public BluePZ(int id, String pltFilePath, double minHeight, double maxHeight) : base(id)
    {
        polygon = PLTParser.parse(pltFilePath);
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

    public BluePZ(int ID, double minHeight, double maxHeight) : base(ID)
    {
        polygon = new List<Coordinate>();
        polygon.Add(new Coordinate(-90, -180, 0, 0, DateTime.MinValue));
        polygon.Add(new Coordinate(-90, 180, 0, 0, DateTime.MinValue));
        polygon.Add(new Coordinate(90, -180, 0, 0, DateTime.MinValue));
        polygon.Add(new Coordinate(90, 180, 0, 0, DateTime.MinValue));

        northernmost = 90;
        easternmost = 180;
        southernmost = -90;
        westernmost = -180;

        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
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

        if (coordinate.Longitude > easternmost || coordinate.Longitude < westernmost ||
            coordinate.Latitude > northernmost || coordinate.Latitude < southernmost)
        {
            comment = "Out of ´box";
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