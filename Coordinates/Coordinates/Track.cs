namespace Coordinates;

public class Track
{
    /// <summary>
    /// The list of track points
    /// </summary>
    public List<Coordinate> TrackPoints { get; private set; } = [];

    /// <summary>
    /// The list of declared goals
    /// </summary>
    public List<Declaration> Declarations { get; private set; } = [];

    /// <summary>
    /// The list of marker drops
    /// </summary>
    public List<MarkerDrop> MarkerDrops { get; private set; } = [];

    /// <summary>
    /// The pilot which created this track
    /// </summary>
    public Pilot Pilot
    {
        get; set;
    }

    public Dictionary<string, string> AdditionalPropertiesFromIGCFile
    {
        get; private set;
    } = [];

    public Track()
    {

    }

    public Declaration? GetLatestDeclaration(int goalNumber)
    {
        return Declarations.OrderBy(x=>x.PositionAtDeclaration.TimeStamp)
            .Where(x => x.GoalNumber == goalNumber)
            .LastOrDefault();
    }

    public MarkerDrop? GetFirstMarkerDrop(int markerNumber)
    {
        return MarkerDrops.OrderBy(x=>x.MarkerLocation.TimeStamp).FirstOrDefault(x => x.MarkerNumber == markerNumber);
    }

    public List<int> GetAllGoalNumbers()
    {
        List<int> allGoalNumbers = [.. Declarations.Select(x => x.GoalNumber).Distinct()];
        return allGoalNumbers;
    }

    public List<int> GetAllMarkerNumbers()
    {
        List<int> allMarkerNumbers = [.. MarkerDrops.Select(x => x.MarkerNumber).Distinct()];
        return allMarkerNumbers;
    }

}
