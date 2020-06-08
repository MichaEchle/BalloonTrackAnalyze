using System;

namespace Coordinates
{
    public enum CoordinateTypes
    {
        Unknown,
        TrackPoint,
        Marker,
        Goal
    }
    public class Coordinate
    {

        /// <summary>
        /// The latitude or northing in decimal degress
        /// <para>use negative values for southing</para>
        /// </summary>
        public double Latitude { get; private set; }

        /// <summary>
        /// The longitude or easting in decimal degrees
        /// <para>use negative values for westing</para>
        /// </summary>
        public double Longitude { get; private set; }

        /// <summary>
        /// The alitude in meters
        /// </summary>
        public double Altitude { get; private set; }

        /// <summary>
        /// The time stamp when this coordiante was create in GPS time
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The type of the coordinate
        /// </summary>
        public CoordinateTypes CoordinateType { get; private set; }

        /// <summary>
        /// Create a new coordinate using decimal degree, meters and gps time
        /// </summary>
        /// <param name="latitude">the latitude in decimal degree</param>
        /// <param name="longitude">the longitude in decimal degree</param>
        /// <param name="altitude">the altidude in meters</param>
        /// <param name="timeStamp">the time stamp of the coordinate in GPS time</param>
        public Coordinate(double latitude, double longitude, double altitude, DateTime timeStamp, CoordinateTypes coordinateType)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            TimeStamp = timeStamp;
            CoordinateType = coordinateType;
        }

    }
}
