using Scoring.Competitions.Pilots;
using Scoring.Coordinates;

namespace Scoring.Competitions.Tracks;

public class Track
{

    /// <summary>
    /// The list of track points
    /// </summary>
    public List<Coordinate> TrackPoints { get; } = [];

    /// <summary>
    /// The list of declared goals
    /// </summary>
    public List<Declaration> Declarations { get; } = [];

    /// <summary>
    /// The list of marker drops
    /// </summary>
    public List<MarkerDrop> MarkerDrops { get; } = [];

    /// <summary>
    /// The pilot which created this track
    /// </summary>
    public required Pilot Pilot
    {
        get; init;
    }

    public TimeSpan TrackPointInterval
    {
        get; set;
    } = TimeSpan.FromSeconds(1);

    public Dictionary<string, string> AdditionalPropertiesFromIGCFile
    {
        get;
    } = [];
}
