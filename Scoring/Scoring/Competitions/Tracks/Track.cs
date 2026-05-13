using Scoring.Competitions.Pilots;
using Scoring.Coordinates;

namespace Scoring.Competitions.Tracks;

/// <summary>
/// Represents the complete flight track data for a single pilot in a competition flight.
/// </summary>
/// <remarks>
/// <para>
/// A track contains a time-series of geographic positions (track points) from an IGC file,
/// along with the pilot's goal declarations and marker drops for that flight.
/// </para>
/// <para>
/// Track points are typically recorded at regular intervals (see <see cref="TrackPointInterval"/>).
/// </para>
/// </remarks>
public class Track
{

	/// <summary>Gets the ordered list of track points for this flight.</summary>
	/// <remarks>
	/// Track points are typically recorded at one-second intervals (see <see cref="TrackPointInterval"/>)
	/// and contain latitude, longitude, and altitude data.
	/// </remarks>
	/// <value>A list of <see cref="Coordinate"/> objects.</value>
	public IList<Coordinate> TrackPoints { get; } = [];

	/// <summary>Gets the list of goals declared by the pilot during the flight.</summary>
	/// <remarks>
	/// One or more goals may be declared depending on the task design.
	/// </remarks>
	/// <value>A list of <see cref="Declaration"/> objects.</value>
	public IList<Declaration> Declarations { get; } = [];

	/// <summary>Gets the list of markers dropped by the pilot during the flight.</summary>
	/// <remarks>
	/// Marker drops are recorded with their location. Physical markers take precedence over electronic markers during scoring.
	/// </remarks>
	/// <value>A list of <see cref="MarkerDrop"/> objects.</value>
	public IList<MarkerDrop> MarkerDrops { get; } = [];

	/// <summary>Gets the pilot who created this track.</summary>
	/// <value>A <see cref="Pilot"/> object.</value>
	public required Pilot Pilot
	{
		get; init;
	}

	/// <summary>Gets or sets the time interval between consecutive track points.</summary>
	/// <remarks>
	/// This is typically one second for modern GPS/logging devices, but may vary.
	/// Used for penalty calculations and time-in-zone computations.
	/// </remarks>
	/// <value>A <see cref="TimeSpan"/> (default: one second).</value>
	public TimeSpan TrackPointInterval
	{
		get; set;
	} = TimeSpan.FromSeconds(1);

	/// <summary>Gets a dictionary of additional metadata from the IGC file header.</summary>
	/// <remarks>
	/// May contain fields like equipment type, pilot name, flight date, logger serial number, etc.,
	/// parsed from the IGC header but not explicitly modeled.
	/// </remarks>
	/// <value>A dictionary with string keys and string values.</value>
	public Dictionary<string, string> AdditionalPropertiesFromIGCFile
	{
		get;
	} = [];
}
