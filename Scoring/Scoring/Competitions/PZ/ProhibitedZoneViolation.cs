using Scoring.Competitions.Tracks;
using Scoring.Coordinates;

namespace Scoring.Competitions.PZ;

/// <summary>
/// Represents a detected violation of a prohibited zone during a pilot's track.
/// </summary>
/// <remarks>
/// <para>
/// This internal class records the details of a single violation event: which coordinates
/// entered the zone, how long the pilot spent there, the calculated penalty, the track involved,
/// and the zone that was violated.
/// </para>
/// <para>
/// Instances are immutable with required init-only properties, used internally by <see cref="ProhibitedZoneChecker"/>.
/// </para>
/// </remarks>
internal class ProhibitedZoneViolation
{
	/// <summary>Gets the list of track coordinates that were inside the prohibited zone.</summary>
	/// <value>A list of <see cref="Coordinate"/> objects.</value>
	internal required List<Coordinate> CoordinatesInZone
	{
		get; init;
	}

	/// <summary>Gets the total time the pilot spent inside the prohibited zone.</summary>
	/// <remarks>
	/// Computed from the number of coordinates in the zone and the track's <see cref="Track.TrackPointInterval"/>.
	/// </remarks>
	/// <value>A <see cref="TimeSpan"/> representing duration in the zone.</value>
	internal required TimeSpan TimeInZone
	{
		get; init;
	}

	/// <summary>Gets the total penalty in points for this violation.</summary>
	/// <remarks>
	/// Calculated according to the zone type (<see cref="ProhibitedZone.TypeOfZone"/>) and severity.
	/// </remarks>
	/// <value>The penalty in points.</value>
	internal required int Penalties
	{
		get; init;
	}

	/// <summary>Gets the track that generated this violation.</summary>
	/// <value>A <see cref="Track"/> object.</value>
	internal required Track Track
	{
		get; init;
	}

	/// <summary>Gets the prohibited zone that was violated.</summary>
	/// <value>A <see cref="ProhibitedZone"/> object.</value>
	internal required ProhibitedZone PZ
	{
		get; init;
	}
}
