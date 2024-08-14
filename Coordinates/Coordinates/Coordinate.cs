using System;

namespace Coordinates
{
    public class Coordinate
    {

        /// <summary>
        /// The latitude or northing in decimal degrees
        /// <para>use negative values for southing</para>
        /// </summary>
        public double Latitude { get; private set; }

        /// <summary>
        /// The longitude or easting in decimal degrees
        /// <para>use negative values for westing</para>
        /// </summary>
        public double Longitude { get; private set; }

        /// <summary>
        /// The GPS altitude in meters
        /// </summary>
        public double AltitudeGPS { get; set; }

        /// <summary>
        /// The barometric altitude in meters
        /// </summary>
        public double AltitudeBarometric { get; set; }

        /// <summary>
        /// If the altitude is corrected with the current qnh
        /// </summary>
        public bool CorrectedBarometric
        {
            get;
            set;
        } = false;

        /// <summary>
        /// The time stamp when this coordinate was create in GPS time
        /// </summary>
        public DateTime TimeStamp { get; private set; }


        /// <summary>
        /// Create a new coordinate using decimal degree, meters and gps time
        /// </summary>
        /// <param name="latitude">the latitude in decimal degree</param>
        /// <param name="longitude">the longitude in decimal degree</param>
        /// <param name="altitudeGPS">the gps altitude in meters</param>
        /// <param name="altitudeBarometric">the barometric altitude in meters</param>
        /// <param name="timeStamp">the time stamp of the coordinate in GPS time</param>
        public Coordinate(double latitude, double longitude, double altitudeGPS, double altitudeBarometric, DateTime timeStamp)
        {
            Latitude = latitude;
            Longitude = longitude;
            AltitudeGPS = altitudeGPS;
            AltitudeBarometric = altitudeBarometric;
            TimeStamp = timeStamp;
        }

        
        public string utmZone
        {
            get;
            set;
        }

        public double easting
        {
            get;
            set;
        }

        public double northing
        {
            get;
            set;
        }

        public Coordinate Clone()
        {
            return new Coordinate(Latitude, Longitude, AltitudeGPS, AltitudeBarometric, TimeStamp);
        }

        public void CorrectBarometricHeight(double qnh)
        {
            AltitudeBarometric = CoordinateHelpers.ConvertBarometricHeight(AltitudeBarometric, qnh);
            CorrectedBarometric = true;
        }
    }
}
