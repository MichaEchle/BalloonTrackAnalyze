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

    /// <summary>
    /// Adjusts the given altitude based on the specified QNH (barometric pressure) value.
    /// </summary>
    /// <remarks>The correction is based on the difference between the specified QNH and the standard pressure
    /// of 1013.25 hPa. A lower QNH results in a higher corrected altitude, while a higher QNH results  in a lower
    /// corrected altitude.</remarks>
    /// <param name="altitude">The altitude to be corrected, in meter.</param>
    /// <param name="qnh">The QNH (barometric pressure) value, in hPa. Must be between 800 and 1200 hPa.</param>
    /// <returns>The corrected altitude, in meter.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="qnh"/> is less than 800 hPa or greater than 1200 hPa.</exception>
    public static double CorrectAltitudeWithQNH(double altitude, double qnh)
    {
        if (qnh is < 800 or > 1200)
        {
            throw new ArgumentOutOfRangeException(nameof(qnh), "QNH must be between 800 and 1200 hPa");
        }
        const double standardPressure = 1013.25;
        const double correctAbove = 0.121;
        const double correctBellow = 0.119;
        double newAltitude = altitude;
        if (qnh < standardPressure)
        {
            newAltitude = altitude + (qnh - standardPressure) / correctBellow;
        }
        else if (qnh > standardPressure)
        {
            newAltitude = altitude + (qnh - standardPressure) / correctAbove;
        }
        return newAltitude;
    }

    /// <summary>
    /// Adjusts the altitude based on the provided QNH (barometric pressure) value using ballon live formula.
    /// <para>Use when getting altitude from Ballon Live (App or Sensor)</para>
    /// </summary>
    /// <remarks>This method uses the barometric formula to adjust the altitude based on the difference
    /// between the provided QNH value and the standard atmospheric pressure of 1013.25 hPa.</remarks>
    /// <param name="altitude">The initial altitude in meters to be corrected.</param>
    /// <param name="qnh">The QNH (barometric pressure) in hectopascals (hPa). Must be between 800 and 1200 hPa.</param>
    /// <returns>The corrected altitude in meters, adjusted for the specified QNH value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="qnh"/> is less than 800 or greater than 1200 hPa.</exception>
    public static double CorrectAltitudeWithQNHBallonLive(double altitude, double qnh)
    {
        if (qnh is < 800 or > 1200)
        {
            throw new ArgumentOutOfRangeException(nameof(qnh), "QNH must be between 800 and 1200 hPa");
        }
        const double standardPressure = 1013.25;
        const double t0lambda = 44330.7692;
        const double alpha = 0.190295;

        return (1.0 - Math.Pow(Math.Pow(1.0 - altitude / t0lambda, 1.0 / alpha) + 1.0 - qnh / standardPressure, alpha)) * t0lambda;
    }
}
