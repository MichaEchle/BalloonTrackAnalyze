namespace Scoring.Competitions.Pilots;

public class Pilot
{
    /// <summary>
    /// The first name of the pilot
    /// </summary>
    public required string FirstName
    {
        get; init;
    }

    /// <summary>
    /// The last name of the pilot
    /// </summary>
    public required string LastName
    {
        get; init;
    }

    /// <summary>
    /// A number of the pilot
    /// </summary>
    public required int PilotNumber
    {
        get; init;
    }

    /// <summary>
    /// A list of identifiers associated with that pilot as issued in the track file
    /// </summary>
    public required string PilotIdentifier
    {
        get; init;
    }

    public string GetPilotNameAndNumber()
    {
        return $"#{PilotNumber} {LastName.ToUpperInvariant()}, {FirstName}";
    }
}
