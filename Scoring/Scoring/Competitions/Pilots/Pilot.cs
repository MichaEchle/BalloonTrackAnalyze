namespace Scoring.Competitions.Pilots;

/// <summary>
/// Represents a pilot competing in a hot air balloon competition.
/// </summary>
/// <remarks>
/// Pilot instances are immutable and created via object initializers.
/// All properties are required and use init-only setters.
/// </remarks>
public class Pilot
{
	/// <summary>Gets the first name of the pilot.</summary>
	/// <value>A non-empty string.</value>
	public required string FirstName
	{
		get; init;
	}

	/// <summary>Gets the last name of the pilot.</summary>
	/// <value>A non-empty string.</value>
	public required string LastName
	{
		get; init;
	}

	/// <summary>Gets the pilot's competition number.</summary>
	/// <value>A unique positive integer assigned to the pilot.</value>
	public required int PilotNumber
	{
		get; init;
	}

	/// <summary>Gets the pilot's identifier as recorded in track files.</summary>
	/// <value>A string identifier (e.g., beacon code, logger ID).</value>
	public required string PilotIdentifier
	{
		get; init;
	}

	/// <summary>
	/// Returns a formatted string combining the pilot's number, last name, and first name.
	/// </summary>
	/// <remarks>
	/// The format is: "#[PilotNumber] [LASTNAME], [FirstName]"
	/// (Last name is converted to uppercase).
	/// </remarks>
	/// <returns>A formatted string representation of the pilot.</returns>
	public string GetPilotNameAndNumber()
	{
		return $"#{PilotNumber} {LastName.ToUpperInvariant()}, {FirstName}";
	}
}
