namespace Scoring.Converters;

public class Ellipsoid
{
    public double EquatorialRadius
    {
        get;
    }

    public double PolarRadius
    {
        get;
    }

    public double Flattening
    {
        get;
    }

    public double Eccentricity
    {
        get;
    }

    public double Ellipticity
    {
        get;
    }

    private Ellipsoid(double equatorialRadius, double polarRadius)
    {
        EquatorialRadius = equatorialRadius;
        PolarRadius = polarRadius;
        Flattening = (EquatorialRadius - PolarRadius) / EquatorialRadius;
        Eccentricity = Math.Sqrt(1.0 - Math.Pow(PolarRadius / EquatorialRadius, 2));
        Ellipticity = (EquatorialRadius - PolarRadius) / (EquatorialRadius + PolarRadius);
    }

    public static Ellipsoid WGS84
    {
        get
        {
            return new Ellipsoid(6_378_137.000, 6_356_752.3142);
        }
    }

    public static Ellipsoid GRS80
    {
        get
        {
            return new Ellipsoid(6_378_137.000, 6_356_752.3141);
        }
    }

}
