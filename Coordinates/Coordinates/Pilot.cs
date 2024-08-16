namespace Coordinates;

public class Pilot
{
    /// <summary>
    /// The first name of the pilot
    /// </summary>
    public string FirstName
    {
        get; set;
    }

    /// <summary>
    /// The last name of the pilot
    /// </summary>
    public string LastName
    {
        get; set;
    }

    /// <summary>
    /// A number of the pilot
    /// </summary>
    public int PilotNumber
    {
        get; private set;
    }

    /// <summary>
    /// A list of identifiers associated with that pilot as issued in the track file
    /// </summary>
    public string PilotIdentifier
    {
        get; private set;
    }

    /// <summary>
    /// Create a new pilot
    /// </summary>
    /// <param name="firstName">the first name of the pilot</param>
    /// <param name="lastName">the last name of the pilot</param>
    /// <param name="pilotNumber">the number of the pilot</param>
    /// <param name="pilotIdentifiers">a list of identifiers associated with that pilot as issued in the track file</param>
    public Pilot(string firstName, string lastName, int pilotNumber, string pilotIdentifier)
    {
        FirstName = firstName;
        LastName = lastName;
        PilotNumber = pilotNumber;
        PilotIdentifier = pilotIdentifier;
    }

    public Pilot(int pilotNumber, string pilotIdentifier)
    {
        PilotNumber = pilotNumber;
        PilotIdentifier = pilotIdentifier;
    }

}
