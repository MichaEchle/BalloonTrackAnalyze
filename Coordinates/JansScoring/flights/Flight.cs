using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights;

public abstract class Flight
{
    public abstract int FlightNumber();


    /// <summary>
    /// The beginning of the launch-periode
    ///  
    /// Time need to be in UTC
    /// </summary>
    /// <returns></returns>
    public abstract DateTime StartOfLaunchPeriode();

    /// <summary>
    /// The time how long the launch-periode is. In minutes
    /// </summary>
    /// <returns></returns>
    public abstract int LaunchPeriode();

    /// <summary>
    /// If GPS Altituse should be used for calculations
    /// </summary>
    /// <returns></returns>
    public abstract bool UseGPSAltitude();

    /// <summary>
    /// The distance, witch need to be between start-point and all goals
    /// </summary>
    /// <returns></returns>
    public abstract double DistanceToAllGoals();

    /// <summary>
    /// The System Path of the tracks
    /// </summary>
    /// <returns></returns>
    public abstract string TracksPath();

    public abstract Task[] Tasks();

    public abstract CalculationType CalculationType();

    public abstract double SeperationAltitudeFeet();

    public abstract Coordinate BackupCoordinates();

    public abstract int QNH();

    public double SeperationAltitudeMeters()
    {
        return CoordinateHelpers.ConvertToMeter(SeperationAltitudeFeet());
    }
    public Task GetTaskByNumber(int number)
    {
        foreach (Task task in Tasks())
        {
            if (task.TaskNumber() == number)
            {
                return task;
            }
        }

        return null;
    }
}