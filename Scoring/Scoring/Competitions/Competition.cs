using Scoring.Competitions.Pilots;
using Scoring.Competitions.PZ;
using Scoring.Converters;
using Scoring.Coordinates;

namespace Scoring.Competitions;

/// <summary>
/// Specifies the source of altitude measurements for the competition.
/// </summary>
public enum AltitudeSourceType
{
	/// <summary>Altitude is measured via GPS (typically less accurate for steady altitude).</summary>
	GPS,
	/// <summary>Altitude is measured via barometric pressure sensor (typically more accurate for steady altitude).</summary>
	Barometric
}

/// <summary>
/// Enumeration of common launch points used in competitions.
/// </summary>
public enum CommonLaunchPointType
{
	/// <summary>Common launch point A.</summary>
	CommonLaunchPointA,
	/// <summary>Common launch point B.</summary>
	CommonLaunchPointB,
	/// <summary>Common launch point C.</summary>
	CommonLaunchPointC,
	/// <summary>Common launch point D.</summary>
	CommonLaunchPointD,
	/// <summary>Common launch point E.</summary>
	CommonLaunchPointE,
}

/// <summary>
/// Represents a hot air balloon competition, serving as the global singleton entry point for all competition data.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Competition"/> is a singleton created via <see cref="Create(string, double, AltitudeSourceType, IList{Pilot}, Ellipsoid)"/>.
/// Once created, it is accessed via <see cref="Instance"/>.
/// </para>
/// <para>
/// The competition holds:
/// </para>
/// <list type="bullet">
/// <item><description>Metadata: name, altitude source type, separation altitude (for traffic safety).</description></item>
/// <item><description>Participants: list of <see cref="Pilot"/> objects.</description></item>
/// <item><description>Geographic data: <see cref="Ellipsoid"/> for coordinate conversions, common launch points, prohibited zones.</description></item>
/// </list>
/// </remarks>
public class Competition
{
	/// <summary>Gets the name of the competition.</summary>
	/// <value>A non-empty string.</value>
	public string Name
	{
		get;
	}

	/// <summary>Gets the separation altitude in meters, used for traffic safety calculations.</summary>
	/// <value>Altitude in meters.</value>
	public double SeparationAltitude
	{
		get;
	}

	/// <summary>Gets the source type for altitude measurements in this competition.</summary>
	/// <value>An <see cref="AltitudeSourceType"/> value.</value>
	public AltitudeSourceType AltitudeSource
	{
		get;
	}

	/// <summary>Gets the list of pilots competing in this competition.</summary>
	/// <value>A list of <see cref="Pilot"/> objects.</value>
	public IList<Pilot> Pilots
	{
		get;
	}

	/// <summary>Gets the ellipsoid model used for coordinate system conversions.</summary>
	/// <value>An <see cref="Ellipsoid"/> instance (e.g., WGS84 or GRS80).</value>
	public Ellipsoid Ellipsoid
	{
		get;
	}

	/// <summary>
	/// Initializes a new instance of <see cref="Competition"/> with the specified parameters.
	/// </summary>
	/// <remarks>
	/// This constructor is private; use <see cref="Create(string, double, AltitudeSourceType, IList{Pilot}, Ellipsoid)"/> to create instances.
	/// </remarks>
	/// <param name="name">The name of the competition.</param>
	/// <param name="separationAltitude">The separation altitude in meters.</param>
	/// <param name="altitudeSource">The altitude measurement source type.</param>
	/// <param name="pilots">The list of competing pilots.</param>
	/// <param name="ellipsoid">The ellipsoid model for coordinate conversions.</param>
	private Competition(string name, double separationAltitude, AltitudeSourceType altitudeSource, IList<Pilot> pilots, Ellipsoid ellipsoid)
	{
		Name = name;
		SeparationAltitude = separationAltitude;
		AltitudeSource = altitudeSource;
		Pilots = pilots;
		Ellipsoid = ellipsoid;
	}

	/// <summary>Gets the global singleton instance of the active competition.</summary>
	/// <value>The <see cref="Competition"/> instance.</value>
	/// <exception cref="InvalidOperationException">Thrown if <see cref="Create(string, double, AltitudeSourceType, IList{Pilot}, Ellipsoid)"/> has not been called yet.</exception>
	public static Competition Instance
	{
		get
		{
			if (field is null)
			{
				throw new InvalidOperationException("Competition has not been created yet");
			}
			return field;
		}
		private set;
	}

	/// <summary>
	/// Creates and initializes the global competition singleton.
	/// </summary>
	/// <remarks>
	/// Must be called exactly once before <see cref="Instance"/> is accessed.
	/// </remarks>
	/// <param name="name">The name of the competition (must not be null or whitespace).</param>
	/// <param name="separationAltitude">The separation altitude in meters.</param>
	/// <param name="altitudeSource">The altitude measurement source type.</param>
	/// <param name="pilots">The list of competing pilots (must not be null).</param>
	/// <param name="ellipsoid">The ellipsoid model for coordinate conversions (must not be null).</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is null or whitespace.</exception>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="pilots"/> or <paramref name="ellipsoid"/> is <see langword="null"/>.</exception>
	public static void Create(string name, double separationAltitude, AltitudeSourceType altitudeSource, IList<Pilot> pilots, Ellipsoid ellipsoid)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
		ArgumentNullException.ThrowIfNull(pilots);
		ArgumentNullException.ThrowIfNull(ellipsoid);

		Instance = new Competition(name, separationAltitude, altitudeSource, pilots, ellipsoid);
	}

	/// <summary>Gets a dictionary mapping each common launch point to its coordinate.</summary>
	/// <value>A dictionary with <see cref="CommonLaunchPointType"/> keys and <see cref="Coordinate"/> values.</value>
	public Dictionary<CommonLaunchPointType, Coordinate> CommonLaunchPoints
	{
		get;
	} = new(Enum.GetValues<CommonLaunchPointType>().Length);

	/// <summary>Gets the list of active prohibited zones in this competition.</summary>
	/// <value>A list of <see cref="ProhibitedZone"/> objects.</value>
	public IList<ProhibitedZone> PZs
	{
		get;
	} = [];


}
