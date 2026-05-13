using Scoring.Converters;

namespace Scoring.Coordinates;

/// <summary>
/// Represents a geographic coordinate with latitude, longitude, and optional altitude.
/// </summary>
/// <remarks>
/// Coordinates are immutable. All values use WGS84 decimal degrees internally.
/// Altitude is expressed in meters; use <see cref="AltitudeConverter"/> to convert from feet.
/// When <see langword="null"/>, <see cref="Altitude"/> and <see cref="TimeStamp"/> indicate values not recorded.
/// </remarks>
public class Coordinate
{
    /// <summary>Gets the longitude in decimal degrees (WGS84).</summary>
    /// <value>A value in the range [-180, 180].</value>
    public required double Longitude
    {
        get; init;
    }

    /// <summary>Gets the latitude in decimal degrees (WGS84).</summary>
    /// <value>A value in the range [-90, 90].</value>
    public required double Latitude
    {
        get; init;
    }

    /// <summary>Gets the altitude in meters above sea level, or <see langword="null"/> if not recorded.</summary>
    /// <value>Altitude in meters, or <see langword="null"/>.</value>
    public required double Altitude
    {
        get; init;
    }

    /// <summary>Gets the UTC timestamp of this track point, or <see langword="null"/> for static coordinates.</summary>
    /// <value>A <see cref="DateTime"/> in UTC, or <see langword="null"/>.</value>
    public DateTime? TimeStamp
    {
        get; init;
    }
}
