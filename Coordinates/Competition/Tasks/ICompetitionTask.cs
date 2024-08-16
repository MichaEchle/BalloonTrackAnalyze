using Coordinates;

namespace Competition;

public enum DistanceCalculationType
{
    TwoDimensional,
    ThreeDimensional,
    WithSeparationAlitude
}

public interface ICompetitionTask
{
    /// <summary>
    /// The task number
    /// </summary>
    public int TaskNumber
    {
        get; set;
    }

    /// <summary>
    /// Calculate result of the task with the given track
    /// </summary>
    /// <param name="track">the track to be used</param>
    /// <param name="useGPSAltitude">true: use GPS altitude;false: use barometric altitude</param>
    /// <param name="result">the result of the task</param>
    /// <returns>true:success;false:error</returns>
    public abstract bool CalculateResults(Track track, bool useGPSAltitude, out double result);



}
