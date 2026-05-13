using Scoring.Coordinates;

namespace Scoring.Competitions.Tracks;

/// <summary>
/// Represents a pilot's declared goal for a task in a track.
/// </summary>
/// <remarks>
/// <para>
/// A declaration includes the goal number, the declared goal coordinate (in lat/lon),
/// the position and time at which the goal was declared, and the original UTM coordinates
/// as entered by the pilot for reference.
/// </para>
/// <para>
/// Instances are immutable with required init-only properties.
/// </para>
/// </remarks>
public class Declaration
{
	/// <summary>Gets the declared goal number for the task.</summary>
	/// <value>A positive integer.</value>
	public required int GoalNumber
	{
		get; init;
	}

	/// <summary>Gets the declared goal as a coordinate in latitude/longitude (WGS84).</summary>
	/// <value>A <see cref="Coordinate"/> representing the declared goal position.</value>
	public required Coordinate DeclaredGoal
	{
		get; init;
	}

	/// <summary>Gets the pilot's position and altitude at the time of declaration.</summary>
	/// <value>A <see cref="Coordinate"/> representing the pilot's position when the goal was declared.</value>
	public required Coordinate PositionAtDeclaration
	{
		get; init;
	}

	/// <summary>Gets the easting (UTM east-west coordinate) as originally declared by the pilot, in meters.</summary>
	/// <value>The easting value from the pilot's UTM declaration.</value>
	public required int OriginalEastingDeclarationUTM
	{
		get; init;
	}

	/// <summary>Gets the northing (UTM north-south coordinate) as originally declared by the pilot, in meters.</summary>
	/// <value>The northing value from the pilot's UTM declaration.</value>
	public required int OriginalNorthingDeclarationUTM
	{
		get; init;
	}

	/// <summary>Gets a value indicating whether the pilot explicitly declared a goal altitude.</summary>
	/// <value><see langword="true"/> if the pilot declared an altitude; otherwise <see langword="false"/>.</value>
	public required bool HasPilotDeclaredGoalAltitude
	{
		get; init;
	}
}
