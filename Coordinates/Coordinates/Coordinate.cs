using LoggingConnector;
using Microsoft.Extensions.Logging;
using System;

namespace Coordinates;

public class Coordinate
{

    private readonly ILogger<Coordinate> Logger = LogConnector.LoggerFactory.CreateLogger<Coordinate>();

    /// <summary>
    /// The latitude or northing in decimal degrees
    /// <para>use negative values for southing</para>
    /// </summary>
    public double Latitude
    {
        get; private set;
    }

    /// <summary>
    /// The longitude or easting in decimal degrees
    /// <para>use negative values for westing</para>
    /// </summary>
    public double Longitude
    {
        get; private set;
    }

    /// <summary>
    /// The GPS altitude in meters
    /// </summary>
    public double AltitudeGPS
    {
        get; private set;
    }

    /// <summary>
    /// The barometric altitude in meters
    /// </summary>
    public double AltitudeBarometric
    {
        get; private set;
    }

    /// <summary>
    /// The time stamp when this coordinate was create in GPS time
    /// </summary>
    public DateTime TimeStamp
    {
        get; private set;
    }


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
    /// <summary>
    /// Allows to set the default altitude for the GPS and barometric altitude if they are zero
    /// </summary>
    /// <param name="defaultAltitude">the default altitude to be used in meters.</param>
    /// <returns>true:success; false:error (Altitude was already set to non zero value)</returns>
    public bool SetDefaultAltitude(double defaultAltitude)
    {
        if (AltitudeBarometric is not 0 || AltitudeGPS is not 0)
        {
            Logger?.LogError("Setting a default altitude is not allowed as the altitudes are not zero");
            return false;
        }
        AltitudeGPS = defaultAltitude;
        AltitudeBarometric = defaultAltitude;
        return true;
    }

}
