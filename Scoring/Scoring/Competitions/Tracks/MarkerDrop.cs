using Scoring.Coordinates;

namespace Scoring.Competitions.Tracks;

/// <summary>
/// Represents a physical or electronic marker dropped by a pilot during a flight.
/// </summary>
/// <remarks>
/// <para>
/// Markers are used for scoring certain task types. This class records the marker number
/// (as declared by the pilot or encoded in the track) and its location.
/// </para>
/// <para>
/// Instances are immutable with required init-only properties.
/// </para>
/// </remarks>
public class MarkerDrop
{
	/// <summary>Gets the marker number assigned to this drop.</summary>
	/// <value>A positive integer identifying the marker.</value>
	public required int MarkerNumber
	{
		get; init;
	}

	/// <summary>Gets the geographic location where the marker was dropped.</summary>
	/// <value>A <see cref="Coordinate"/> in WGS84 latitude/longitude with optional altitude.</value>
	public required Coordinate MarkerLocation
	{
		get; init;
	}
}
