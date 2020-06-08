using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinates
{
    public static class CoordinateHelpers
    {
        /// <summary>
        /// the ratio of feet in meter
        /// </summary>
        public const double FEET_TO_METER_RATIO = 0.3048;

        /// <summary>
        /// Convert a part of coordinate from degree minute notation to decimal degree
        /// </summary>
        /// <param name="degrees">the integer degrees</param>
        /// <param name="degreeMinutes">the integer degree minutes</param>
        /// <param name="degreeSeconds">the integer degree seconds</param>
        /// <param name="isNorthingOrEasting">true: is norting or easting; false: is southing or westing </param>
        /// <returns>the part of a coordinate in decimal degree</returns>
        public static double ConvertToDecimalDegree(int degrees, int degreeMinutes, int degreeSeconds, bool isNorthingOrEasting)
        {
            return degrees + (degreeMinutes / 60.0) + (degreeSeconds / 3600.0) * (isNorthingOrEasting ? 1.0 : -1.0);
        }

        /// <summary>
        /// Convert feets into meters
        /// </summary>
        /// <param name="feets">the feets to be converted</param>
        /// <returns>the amount of feet in meters</returns>
        public static double ConvertToMeter(double feets)
        {
            return feets * FEET_TO_METER_RATIO;
        }

        /// <summary>
        /// Converts meters into feets
        /// </summary>
        /// <param name="meters">the meters to be converted</param>
        /// <returns>the amount of meter in feet</returns>
        public static double ConvertToFeet(double meters)
        {
            return meters / FEET_TO_METER_RATIO;
        }
    }
}
