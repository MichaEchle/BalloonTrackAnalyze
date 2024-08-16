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
        Flight = flight;
    }

    /// <summary>
    /// Only for scoring perpurse
    /// </summary>
    public readonly Flight Flight;


    public abstract int TaskNumber();

    /// <summary>
    /// Here the scoring should be done
    /// </summary>
    /// <param name="track">Parses the Track of a pilot</param>
    /// <returns>returns if the result will be n/r. True = NR | False = Normal Scoring</returns>
    public abstract bool ScoringChecks(Track track, ref string comment);
    /// <summary>
    /// Here the scoring should be done
    /// </summary>
    /// <param name="track">Parses the Track of a pilot</param>
    /// <returns>Returns the result and the comment</returns>
    public abstract void Score(Track track, ref string comment, out double result);


    /// <summary>
    /// All goals, need for this Task
    /// </summary>
    /// <returns></returns>
    public abstract Coordinate[] Goals(int pilot);

    /// <summary>
    /// The end of the scoring-periode
    ///
    /// Time need to be in UTC
    /// </summary>
    /// <returns></returns>
    public abstract DateTime GetScoringPeriodUntil();
}