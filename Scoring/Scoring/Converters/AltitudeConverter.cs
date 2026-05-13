namespace Scoring.Converters;

/// <summary>
/// Provides altitude conversion utilities between feet and meters, and QNH (barometric pressure) adjustments.
/// </summary>
/// <remarks>
/// Conversion factor: 1 meter = 3.28084 feet (or 1 foot = 0.3048 meters).
/// QNH adjustment uses standard barometric formula; valid range is 800-1200 hectopascals (hPa).
/// </remarks>
public static class AltitudeConverter
{
    private const double FEET_TO_METER_RATIO = 0.3048;

    /// <summary>
    /// Converts altitude in feet to meters.
    /// </summary>
    /// <param name="feet">The altitude in feet.</param>
    /// <returns>The equivalent altitude in meters.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="feet"/> is infinity or NaN.</exception>
    public static double ConvertToMeter(double feet)
    {
        if (double.IsInfinity(feet) || double.IsNaN(feet))
        {
            throw new ArgumentOutOfRangeException(nameof(feet), "Cannot be infinity or NaN");
        }
        return feet * FEET_TO_METER_RATIO;
    }

    /// <summary>
    /// Converts altitude in meters to feet.
    /// </summary>
    /// <param name="meters">The altitude in meters.</param>
    /// <returns>The equivalent altitude in feet.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="meters"/> is infinity or NaN.</exception>
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
    /// <remarks>
    /// The correction is based on the difference between the specified QNH and the standard pressure
    /// of 1013.25 hPa. A lower QNH results in a higher corrected altitude, while a higher QNH results in a lower
    /// corrected altitude. Uses standard barometric correction coefficients.
    /// </remarks>
    /// <param name="altitude">The altitude to be corrected, in meters.</param>
    /// <param name="qnh">The QNH (barometric pressure) value, in hectopascals (hPa). Must be between 800 and 1200 hPa.</param>
    /// <returns>The corrected altitude, in meters.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="qnh"/> is less than 800 hPa or greater than 1200 hPa.</exception>
    public static double CorrectAltitudeWithQNH(double altitude, double qnh)
    {
        if (qnh is < 800 or > 1200)
        {
            throw new ArgumentOutOfRangeException(nameof(qnh), "QNH must be between 800 and 1200 hPa");
        }
        const double standardPressure = 1013.25;
        // Barometric formula coefficient for pressure above standard (standard atmosphere model)
        const double correctAbove = 0.121;
        // Barometric formula coefficient for pressure below standard (standard atmosphere model)
        const double correctBelow = 0.119;
        double newAltitude = altitude;
        if (qnh < standardPressure)
        {
            newAltitude = altitude + (qnh - standardPressure) / correctBelow;
        }
        else if (qnh > standardPressure)
        {
            newAltitude = altitude + (qnh - standardPressure) / correctAbove;
        }
        return newAltitude;
    }

    /// <summary>
    /// Adjusts the altitude based on the provided QNH (barometric pressure) value using the Balloon Live formula.
    /// </summary>
    /// <remarks>
    /// This method uses the barometric formula to adjust the altitude based on the difference
    /// between the provided QNH value and the standard atmospheric pressure of 1013.25 hPa.
    /// Use this method when getting altitude from Balloon Live (App or Sensor).
    /// </remarks>
    /// <param name="altitude">The initial altitude in meters to be corrected.</param>
    /// <param name="qnh">The QNH (barometric pressure) in hectopascals (hPa). Must be between 800 and 1200 hPa.</param>
    /// <returns>The corrected altitude in meters, adjusted for the specified QNH value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="qnh"/> is less than 800 or greater than 1200 hPa.</exception>
    public static double CorrectAltitudeWithQNHBalloonLive(double altitude, double qnh)
    {
        if (qnh is < 800 or > 1200)
        {
            throw new ArgumentOutOfRangeException(nameof(qnh), "QNH must be between 800 and 1200 hPa");
        }
        const double standardPressure = 1013.25;
        // Barometric formula constant: approximate altitude offset for pressure-altitude conversion (meters)
        const double t0lambda = 44330.7692;
        // Barometric formula exponent: reciprocal of temperature lapse rate coefficient in standard atmosphere model
        const double alpha = 0.190295;

        return (1.0 - Math.Pow(Math.Pow(1.0 - altitude / t0lambda, 1.0 / alpha) + 1.0 - qnh / standardPressure, alpha)) * t0lambda;
    }
}
