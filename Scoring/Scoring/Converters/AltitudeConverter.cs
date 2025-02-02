namespace Scoring.Converters;
public static class AltitudeConverter
{
    private const double FEET_TO_METER_RATIO = 0.3048;

    /// <summary>
    /// Convert feet into meters
    /// </summary>
    /// <param name="feets">the feet to be converted</param>
    /// <returns>the amount of feet in meters</returns>
    public static double ConvertToMeter(double feets)
    {
        if (double.IsInfinity(feets) || double.IsNaN(feets))
        {
            throw new ArgumentOutOfRangeException(nameof(feets), "Cannot be infinity or NaN");
        }
        return feets * FEET_TO_METER_RATIO;
    }

    /// <summary>
    /// Converts meters into feet
    /// </summary>
    /// <param name="meters">the meters to be converted</param>
    /// <returns>the amount of meter in feet</returns>
    public static double ConvertToFeet(double meters)
    {
        if (double.IsInfinity(meters) || double.IsNaN(meters))
        {
            throw new ArgumentOutOfRangeException(nameof(meters), "Cannot be infinity or NaN");
        }

        return meters / FEET_TO_METER_RATIO;
    }
}
