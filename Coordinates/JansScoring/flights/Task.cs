using Competition;
using Coordinates;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights;

public abstract class Task
{
    protected Task(Flight flight)
    {
        this.flight = flight;
    }

    /// <summary>
    /// Only for scoring perpurse
    /// </summary>
    protected Flight flight;


    public abstract int number();

    /// <summary>
    /// Here the scoring should be done
    /// </summary>
    /// <param name="track">Parses the Track of a pilot</param>
    /// <returns>Returns the result and the comment</returns>
    public abstract string[] score(Track track);


    /// <summary>
    /// All goals, need for this Task
    /// </summary>
    /// <returns></returns>
    public abstract Coordinate[] goals();

    /// <summary>
    /// The end of the scoring-periode
    ///
    /// Time need to be in UTC
    /// </summary>
    /// <returns></returns>
    public abstract DateTime getScoringPeriodeUntil();

    public bool checkScoringPeriodeForMarker(Track track, MarkerDrop markerDrop, out string comment)
    {
        comment = null;
        if (markerDrop.MarkerTime == DateTime.MinValue)
        {
            comment += "Cant get Scoringperiode because MarkerTime is null";
            return false;
        }

        Coordinate trackPoint = getTrackPointByTime(track, markerDrop.MarkerTime);
        if (trackPoint.TimeStamp >= getScoringPeriodeUntil())
        {
            TimeSpan timeOutsideScoringPeriode =
                getScoringPeriodeUntil() - trackPoint.TimeStamp;
            comment +=
                $"Pilots last Trackpoint was outside the scoringperiode of task {number()}({timeOutsideScoringPeriode.ToString(@"hh\:mm\:ss")}) | ";
            return false;
        }

        return true;
    }


    private Coordinate getTrackPointByTime(Track track, DateTime time)
    {
        foreach (Coordinate currentTrackPoint in track.TrackPoints)
        {
            if (currentTrackPoint.TimeStamp == time)
            {
                return currentTrackPoint;
            }
        }

        return null;
    }

    public void Log(LogSeverityType logSeverity, string text)
    {
        Logger.Log("Task " + number(), logSeverity, text);
        Console.WriteLine($"LOG: {logSeverity.ToString()} | {text}");
    }
}