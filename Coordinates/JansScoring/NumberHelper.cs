using System;

namespace JansScoring;

public class NumberHelper
{
    public static string formatDoubleToStringAndRound(double formatted)
    {
        return Math.Round(formatted, 4).ToString().Replace(",", ".");
    }


}