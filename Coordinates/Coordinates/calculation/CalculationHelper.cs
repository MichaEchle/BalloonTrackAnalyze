using Coordinates;
using System;
using System.Collections.Generic;

namespace JansScoring.calculation;

public class CalculationHelper
{
    public static double Calculate2DDistance(Coordinate coordinate1, Coordinate coordinate2,
        CalculationType calculationType)
    {
        if (coordinate1 == null || coordinate2 == null)
        {
            Console.WriteLine("One Variable was null at calculate the distance");
            return -1;
        }

        switch (calculationType)
        {
            case CalculationType.Havercos:
                return CoordinateHelpers.Calculate2DDistanceHavercos(coordinate1, coordinate2);
            case CalculationType.Haversin:
                return CoordinateHelpers.Calculate2DDistanceHaversin(coordinate1, coordinate2);
            case CalculationType.VincentyWSG84:
                return CoordinateHelpers.Calculate2DDistanceVincentyWSG84(coordinate1, coordinate2);
            case CalculationType.UTM:
                return CoordinateHelpers.Calculate2DDistanceUTM(coordinate1, coordinate2);
            case CalculationType.UTMPrecise:
                return CoordinateHelpers.Calculate2DDistanceUTM_Precise(coordinate1, coordinate2);
            default:
                return -1;
        }
    }

    public static List<double> calculate3DDistanceToAllGoals(Coordinate mark, Coordinate[] goals, bool useGPSAltitude,
        CalculationType calculationType)
    {
        List<double> distances = new List<double>();
        foreach (Coordinate goal in goals)
        {
            distances.Add(CoordinateHelpers.Calculate3DDistance(mark, goal, useGPSAltitude, calculationType));
        }

        return distances;
    }

    public static List<double> calculate2DDistanceToAllGoals(Coordinate mark, Coordinate[] goals,
        CalculationType calculationType)
    {
        List<double> distances = new List<double>();
        foreach (Coordinate goal in goals)
        {
            distances.Add(Calculate2DDistance(mark, goal, calculationType));
        }

        return distances;
    }

    public static double calculateDistancePanelties(double needed, double had)
    {
        double neededMore = needed - had;
        double percent = (neededMore / needed) * 100;
        Console.WriteLine();
        return (percent > 25 ? -1 : Math.Abs(percent * 20));
    }
}