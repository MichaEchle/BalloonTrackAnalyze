using Scoring.Competitions.Tracks;
using Scoring.Coordinates;
using Scoring.Shapes;

namespace Scoring.Competitions.PZ;

/// <summary>
/// Specifies the type and penalty severity of a prohibited zone.
/// </summary>
public enum ProhibitedZoneType
{
	/// <summary>
	/// Blue zone: time-based penalty determined by altitude infringement above the lower boundary.
	/// </summary>
	Blue,
	/// <summary>
	/// Yellow zone: area-based penalty proportional to depth of penetration.
	/// </summary>
	Yellow,
	/// <summary>
	/// Red zone: strictest area-based penalty (typically equivalent to task disqualification).
	/// </summary>
	Red,
}

/// <summary>
/// Represents an airspace zone where balloons are prohibited or restricted during a competition.
/// </summary>
/// <remarks>
/// <para>
/// Prohibited zones are defined by a 3D shape (e.g., <see cref="Cylinder"/>, <see cref="Sphere"/>, <see cref="UniformPrism"/>).
/// Each zone has a type that determines how violations are penalized:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <see cref="ProhibitedZoneType.Blue"/>: Penalties are based on altitude infringement time and amount.
/// </description></item>
/// <item><description>
/// <see cref="ProhibitedZoneType.Yellow"/>: Penalties scale with depth and lateral penetration.
/// </description></item>
/// <item><description>
/// <see cref="ProhibitedZoneType.Red"/>: Highest penalty; typically used for dangerous airspace or no-fly zones.
/// </description></item>
/// </list>
/// <para>
/// Zones can be temporarily deactivated via <see cref="IsActive"/>.
/// </para>
/// </remarks>
public class ProhibitedZone
{
	/// <summary>Gets or sets a value indicating whether this zone is active for enforcement.</summary>
	/// <remarks>
	/// Inactive zones are not checked during scoring. Defaults to <see langword="true"/>.
	/// </remarks>
	/// <value><see langword="true"/> if the zone is active; otherwise <see langword="false"/>.</value>
	public bool IsActive
	{
		get; set;
	} = true;

	/// <summary>Gets a unique identifier for this prohibited zone.</summary>
	/// <value>A non-empty string (e.g., "PZ_001", "CTR_AIRSPACE").</value>
	public required string Identifier
	{
		get; init;
	}

	/// <summary>Gets the type of this prohibited zone.</summary>
	/// <remarks>
	/// The zone type determines how penalties are calculated for violations.
	/// </remarks>
	/// <value>A <see cref="ProhibitedZoneType"/> value.</value>
	public required ProhibitedZoneType TypeOfZone
	{
		get; init;
	}

	/// <summary>Gets the 3D geometric definition of this zone.</summary>
	/// <remarks>
	/// Can be a <see cref="Cylinder"/>, <see cref="Sphere"/>, or <see cref="UniformPrism"/>.
	/// </remarks>
	/// <value>A <see cref="Shapes3D"/> subclass.</value>
	public required Shapes3D ZoneDefinition
	{
		get; init;
	}


}
