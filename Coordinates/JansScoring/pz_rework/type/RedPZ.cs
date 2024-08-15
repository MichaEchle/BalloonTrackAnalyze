﻿using Coordinates;
using JansScoring.calculation;
using JansScoring.flights;
using System;

namespace JansScoring.pz_rework.type;

public class RedPZ : PZ
{
    private Coordinate centerCoordinate;
    private double height;
    private int radius;

    public RedPZ(int id, Coordinate centerCoordinate, double height, int radius) : base(id)
    {
        this.centerCoordinate = centerCoordinate;
        this.height = height;
        this.radius = radius;
    }

    public override bool IsInsidePz(Flight flight, Track track, Coordinate coordinate, out String comment)
    {
        comment = "";
        double disctanceBetweenRedPZ =
            CalculationHelper.Calculate2DDistance(coordinate, centerCoordinate, flight.getCalculationType());
        if (disctanceBetweenRedPZ <= radius &&
            (flight.useGPSAltitude() ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric) <=
            height)
        {
            return true;
        }

        return false;
    }

    public double calculatePenalty(Flight flight, Coordinate entry, Coordinate exit, out double percentage)
    {
        double distanceHorizontal = CalculationHelper.Calculate2DDistance(entry, exit, flight.getCalculationType());
        double averageHeigtDiffrence = Math.Abs((flight.useGPSAltitude()
            ? entry.AltitudeGPS + exit.AltitudeGPS
            : entry.AltitudeBarometric + exit.AltitudeBarometric) / 2);
        double horizontalPercentage = (distanceHorizontal / radius * 2) * 100;
        double verticalPercentage = 100 - ((averageHeigtDiffrence / height) * 100);
        percentage = (verticalPercentage + horizontalPercentage) / 2;
        return percentage * 500;
    }
}