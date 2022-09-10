using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights;

public abstract class Flight
{
    public abstract int getFlightNumber();


    /// <summary>
    /// The beginning of the launch-periode
    ///  
    /// Time need to be in UTC
    /// </summary>
    /// <returns></returns>
    public abstract DateTime getStartOfLaunchPeriode();

    /// <summary>
    /// The time how long the launch-periode is. In minutes
    /// </summary>
    /// <returns></returns>
    public abstract int launchPeriode();

    /// <summary>
    /// If GPS Altituse should be used for calculations
    /// </summary>
    /// <returns></returns>
    public abstract bool useGPSAltitude();

    /// <summary>
    /// The distance, witch need to be between start-point and all goals
    /// </summary>
    /// <returns></returns>
    public abstract int distanceToAllGoals();

    /// <summary>
    /// The System Path of the tracks
    /// </summary>
    /// <returns></returns>
    public abstract string getTracksPath();

    public abstract Task[] getTasks();

    public abstract CalculationType getCalculationType();

    public abstract double getSeperationAltitudeFeet();

    public double getSeperationAltitudeMeters()
    {
        return CoordinateHelpers.ConvertToMeter(getSeperationAltitudeFeet());
    }

   
}