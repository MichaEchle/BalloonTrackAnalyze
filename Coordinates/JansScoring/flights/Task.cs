using Competition;
using Coordinates;
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
}