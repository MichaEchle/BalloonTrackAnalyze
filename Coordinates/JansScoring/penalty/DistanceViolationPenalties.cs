using System;
using System.Globalization;

namespace JansScoring;

public class DistanceViolationPenalties
{
    public static double CalculatePenalty(double distance, double distanceRequired, out double percentage)
    {
        percentage = (distance * 100) / distanceRequired;
        percentage = 100 - percentage;
        if (percentage > 25)
        {
            return Double.MaxValue;
        }

        return Math.Ceiling(percentage * 10) * 2;
    }

    public static String FormatePenalty(double penalty, double percentage, string unit)
    {
        if (penalty == Double.MaxValue)
            return $"Penalty NR ({NumberHelper.formatDoubleToStringAndRound(percentage) + "%"})";
        return penalty + unit + $" ({NumberHelper.formatDoubleToStringAndRound(percentage)}%)";
    }

    public static String CalculateAndFormatPenalty(double distance, double distanceRequired, string unit)
    {
        return FormatePenalty(CalculatePenalty(distance, distanceRequired, out double percentage), percentage, unit);
    }
}