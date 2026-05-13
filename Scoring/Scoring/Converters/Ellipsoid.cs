namespace Scoring.Converters;

/// <summary>
/// Represents an ellipsoid model used for coordinate system conversions (e.g., UTM, lat/lon).
/// </summary>
/// <remarks>
/// Ellipsoid instances are immutable and created via static factory properties <see cref="WGS84"/> and <see cref="GRS80"/>.
/// Common parameters include equatorial radius (a), polar radius (b), flattening (f), eccentricity (e), and ellipticity.
/// </remarks>
public class Ellipsoid
{
    /// <summary>Gets the equatorial radius (semi-major axis) in meters.</summary>
    /// <value>The equatorial radius length.</value>
    public double EquatorialRadius
    {
        get;
    }

    /// <summary>Gets the polar radius (semi-minor axis) in meters.</summary>
    /// <value>The polar radius length.</value>
    public double PolarRadius
    {
        get;
    }

    /// <summary>Gets the flattening ratio: (a - b) / a, where a is equatorial radius and b is polar radius.</summary>
    /// <value>The flattening ratio.</value>
    public double Flattening
    {
        get;
    }

    /// <summary>Gets the first eccentricity: sqrt(1 - (b/a)²).</summary>
    /// <value>The first eccentricity.</value>
    public double Eccentricity
    {
        get;
    }

    /// <summary>Gets the ellipticity: (a - b) / (a + b), an alternative measure of Earth's oblateness.</summary>
    /// <value>The ellipticity ratio.</value>
    public double Ellipticity
    {
        get;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Ellipsoid"/> with the specified radii.
    /// </summary>
    /// <remarks>
    /// The constructor is private; use <see cref="WGS84"/> or <see cref="GRS80"/> factory properties to obtain instances.
    /// </remarks>
    /// <param name="equatorialRadius">The equatorial radius (semi-major axis) in meters.</param>
    /// <param name="polarRadius">The polar radius (semi-minor axis) in meters.</param>
    private Ellipsoid(double equatorialRadius, double polarRadius)
    {
        EquatorialRadius = equatorialRadius;
        PolarRadius = polarRadius;
        Flattening = (EquatorialRadius - PolarRadius) / EquatorialRadius;
        Eccentricity = Math.Sqrt(1.0 - Math.Pow(PolarRadius / EquatorialRadius, 2));
        Ellipticity = (EquatorialRadius - PolarRadius) / (EquatorialRadius + PolarRadius);
    }

    /// <summary>Gets the WGS84 ellipsoid (world standard for GPS and general geographic calculations).</summary>
    /// <value>An <see cref="Ellipsoid"/> instance with WGS84 parameters.</value>
    public static Ellipsoid WGS84
    {
        get
        {
            return new Ellipsoid(6_378_137.000, 6_356_752.3142);
        }
    }

    /// <summary>Gets the GRS80 ellipsoid (used for UTM and European coordinate systems).</summary>
    /// <value>An <see cref="Ellipsoid"/> instance with GRS80 parameters.</value>
    public static Ellipsoid GRS80
    {
        get
        {
            return new Ellipsoid(6_378_137.000, 6_356_752.3141);
        }
    }

}
