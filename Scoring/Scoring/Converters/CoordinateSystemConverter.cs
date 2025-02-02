using Scoring.Competitions;
using Scoring.Coordinates;

namespace Scoring.Converters;

internal class CoordinateSystemConverter
{
    public static (string utmZone, double easting, double northing) ConvertToUTMPrecise(double latitude, double longitude)
    {
        //if (latitude < -80 || latitude > 84)
        //{
        //    throw new ArgumentOutOfRangeException("Latitude must be between -80 and 84 degrees.");
        //}

        double utmScalingFactor = 0.9996;//scaling factor

        (string latitudeZone, int longitudeZone) = GetUTMZoneFromLatLong(latitude, longitude);
        double zoneCentralMeridian = (longitudeZone - 1.0) * 6.0 - 177.0; // Central meridian for the zone

        double degreestoRadiansFactor = Math.PI / 180.0;
        double latRad = latitude * degreestoRadiansFactor;
        double lonRad = longitude * degreestoRadiansFactor;
        double zoneCentralMeridianRad = zoneCentralMeridian * degreestoRadiansFactor;

        double eccentricitySquared = Competition.Instance.Ellipsoid.Eccentricity * Competition.Instance.Ellipsoid.Eccentricity;
        double eccentricityQuadrupled = eccentricitySquared * eccentricitySquared;

        double e = Competition.Instance.Ellipsoid.EquatorialRadius / Math.Sqrt(1.0 - eccentricitySquared * Math.Sin(latRad) * Math.Sin(latRad));
        double t = Math.Tan(latRad) * Math.Tan(latRad);
        double c = (eccentricitySquared / (1.0 - eccentricitySquared)) * Math.Cos(latRad) * Math.Cos(latRad);
        double a = Math.Cos(latRad) * (lonRad - zoneCentralMeridianRad);
        double aSquared = a * a;
        double aQuadrupled = aSquared * aSquared;

        double m = Competition.Instance.Ellipsoid.EquatorialRadius * (
            (1.0 - eccentricitySquared / 4.0 - 3.0 * eccentricityQuadrupled / 64.0 - 5.0 * eccentricityQuadrupled * eccentricitySquared / 256.0) * latRad
            - (3.0 * eccentricitySquared / 8.0 + 3.0 * eccentricityQuadrupled / 32.0 + 45.0 * eccentricityQuadrupled * eccentricitySquared / 1024.0) * Math.Sin(2.0 * latRad)
            + (15.0 * eccentricityQuadrupled / 256.0 + 45.0 * eccentricityQuadrupled * eccentricitySquared / 1024.0) * Math.Sin(4.0 * latRad)
            - (35.0 * eccentricityQuadrupled * eccentricitySquared / 3072.0) * Math.Sin(6.0 * latRad));

        double easting = utmScalingFactor * e * (a + (1.0 - t + c) * aSquared * a / 6.0 +
            (5.0 - 18.0 * t + t * t + 72.0 * c - 58.0 * eccentricitySquared) * aQuadrupled * a / 120.0) + 500000.0;

        double northing = utmScalingFactor * (m + e * Math.Tan(latRad) *
            (a * a / 2.0 + (5.0 - t + 9.0 * c + 4.0 * c * c) * aQuadrupled / 24.0 +
            (61.0 - 58.0 * t + t * t + 600.0 * c - 330.0 * eccentricitySquared) * aQuadrupled * aSquared / 720.0));

        if (latitude < 0)
        {
            northing += 10000000; // Offset for southern hemisphere
        }

        return ($"{latitudeZone}{longitudeZone}", easting, northing);
    }

    public static (string utmZone, double easting, double northing) ConvertToUTMPrecise(Coordinate coordinate)
    {
        return ConvertToUTMPrecise(coordinate.Latitude, coordinate.Longitude);
    }

    public static (string utmZone, int easting, int northing) ConvertToUTM(double latitude, double longitude)
    {
        (string utmZone, double easting, double northing) = ConvertToUTMPrecise(latitude, longitude);
        return (utmZone, (int)Math.Round(easting, 0, MidpointRounding.AwayFromZero), (int)Math.Round(northing, 0, MidpointRounding.AwayFromZero));
    }

    public static (string utmZone, int easting, int northing) ConvertToUTM(Coordinate coordinate)
    {
        return ConvertToUTM(coordinate.Latitude, coordinate.Longitude);
    }

    public static (double latitude, double longitude) ConvertToLatitudeLongitudePrecise(string utmZone, double easting, double northing)
    {
        (char latitudeZone, int longitudeZone) = GetUTMZoneComponents(utmZone);

        bool isSouthHemisphere = latitudeZone < 'N';
        double degreestoRadiansFactor = Math.PI / 180.0;
        double centrealMeridian = (-183.0 + (longitudeZone * 6.0)) * degreestoRadiansFactor;

        double utmScalingFactor = 0.9996;

        easting -= 500000.0;
        easting /= utmScalingFactor;


        if (isSouthHemisphere)
        {
            northing -= 10000000.0;
        }

        northing /= utmScalingFactor;


        double footpointLongitude, nf, nfpow, nuf2, ep2, tf, tf2, tf4, cf;
        double x1frac, x2frac, x3frac, x4frac, x5frac, x6frac, x7frac, x8frac;
        double x2poly, x3poly, x4poly, x5poly, x6poly, x7poly, x8poly;

        /* Precalculate alpha_ (Eq. 10.22) */
        /* (Same as alpha in Eq. 10.17) */
        double alpha = ((Competition.Instance.Ellipsoid.EquatorialRadius + Competition.Instance.Ellipsoid.PolarRadius) / 2.0) * (1 + (Math.Pow(Competition.Instance.Ellipsoid.Ellipticity, 2.0) / 4) + (Math.Pow(Competition.Instance.Ellipsoid.Ellipticity, 4.0) / 64));

        /* Precalculate y_ (Eq. 10.23) */
        double northing_ = northing / alpha;

        /* Precalculate beta_ (Eq. 10.22) */
        double beta = (3.0 * Competition.Instance.Ellipsoid.Ellipticity / 2.0) + (-27.0 * Math.Pow(Competition.Instance.Ellipsoid.Ellipticity, 3.0) / 32.0)
            + (269.0 * Math.Pow(Competition.Instance.Ellipsoid.Ellipticity, 5.0) / 512.0);

        /* Precalculate gamma_ (Eq. 10.22) */
        double gamma = (21.0 * Math.Pow(Competition.Instance.Ellipsoid.Ellipticity, 2.0) / 16.0)
            + (-55.0 * Math.Pow(Competition.Instance.Ellipsoid.Ellipticity, 4.0) / 32.0);

        /* Precalculate delta_ (Eq. 10.22) */
        double delta = (151.0 * Math.Pow(Competition.Instance.Ellipsoid.Ellipticity, 3.0) / 96.0)
            + (-417.0 * Math.Pow(Competition.Instance.Ellipsoid.Ellipticity, 5.0) / 128.0);

        /* Precalculate epsilon_ (Eq. 10.22) */
        double epsilon = (1097.0 * Math.Pow(Competition.Instance.Ellipsoid.Ellipticity, 4.0) / 512.0);

        /* Now calculate the sum of the series (Eq. 10.21) */
        footpointLongitude = northing_ + (beta * Math.Sin(2.0 * northing_))
            + (gamma * Math.Sin(4.0 * northing_))
            + (delta * Math.Sin(6.0 * northing_))
            + (epsilon * Math.Sin(8.0 * northing_));

        /* Precalculate ep2 */
        ep2 = (Math.Pow(Competition.Instance.Ellipsoid.EquatorialRadius, 2.0) - Math.Pow(Competition.Instance.Ellipsoid.PolarRadius, 2.0))
              / Math.Pow(Competition.Instance.Ellipsoid.PolarRadius, 2.0);

        /* Precalculate cos (phif) */
        cf = Math.Cos(footpointLongitude);

        /* Precalculate nuf2 */
        nuf2 = ep2 * Math.Pow(cf, 2.0);

        /* Precalculate Nf and initialize Nfpow */
        nf = Math.Pow(Competition.Instance.Ellipsoid.EquatorialRadius, 2.0) / (Competition.Instance.Ellipsoid.PolarRadius * Math.Sqrt(1 + nuf2));
        nfpow = nf;

        /* Precalculate tf */
        tf = Math.Tan(footpointLongitude);
        tf2 = tf * tf;
        tf4 = tf2 * tf2;

        /* Precalculate fractional coefficients for x**Ellipsoid.Ellipticity in the equations
           below to simplify the expressions for latitude and longitude. */
        x1frac = 1.0 / (nfpow * cf);

        nfpow *= nf;   /* now equals Nf**2) */
        x2frac = tf / (2.0 * nfpow);

        nfpow *= nf;   /* now equals Nf**3) */
        x3frac = 1.0 / (6.0 * nfpow * cf);

        nfpow *= nf;   /* now equals Nf**4) */
        x4frac = tf / (24.0 * nfpow);

        nfpow *= nf;   /* now equals Nf**5) */
        x5frac = 1.0 / (120.0 * nfpow * cf);

        nfpow *= nf;   /* now equals Nf**6) */
        x6frac = tf / (720.0 * nfpow);

        nfpow *= nf;   /* now equals Nf**7) */
        x7frac = 1.0 / (5040.0 * nfpow * cf);

        nfpow *= nf;   /* now equals Nf**8) */
        x8frac = tf / (40320.0 * nfpow);

        /* Precalculate polynomial coefficients for x**Ellipsoid.Ellipticity.
           -- x**1 does not have a polynomial coefficient. */
        x2poly = -1.0 - nuf2;

        x3poly = -1.0 - 2 * tf2 - nuf2;

        x4poly = 5.0 + 3.0 * tf2 + 6.0 * nuf2 - 6.0 * tf2 * nuf2
            - 3.0 * (nuf2 * nuf2) - 9.0 * tf2 * (nuf2 * nuf2);

        x5poly = 5.0 + 28.0 * tf2 + 24.0 * tf4 + 6.0 * nuf2 + 8.0 * tf2 * nuf2;

        x6poly = -61.0 - 90.0 * tf2 - 45.0 * tf4 - 107.0 * nuf2
            + 162.0 * tf2 * nuf2;

        x7poly = -61.0 - 662.0 * tf2 - 1320.0 * tf4 - 720.0 * (tf4 * tf2);

        x8poly = 1385.0 + 3633.0 * tf2 + 4095.0 * tf4 + 1575 * (tf4 * tf2);

        /* Calculate latitude */
        double latitudeRad = footpointLongitude + x2frac * x2poly * (easting * easting)
            + x4frac * x4poly * Math.Pow(easting, 4.0)
            + x6frac * x6poly * Math.Pow(easting, 6.0)
            + x8frac * x8poly * Math.Pow(easting, 8.0);

        /* Calculate longitude */
        double longitudeRad = centrealMeridian + x1frac * easting
            + x3frac * x3poly * Math.Pow(easting, 3.0)
            + x5frac * x5poly * Math.Pow(easting, 5.0)
            + x7frac * x7poly * Math.Pow(easting, 7.0);
        double radiansToDegreesFactor = 180.0 / Math.PI;
        double latitude = latitudeRad * radiansToDegreesFactor;
        double longitude = longitudeRad * radiansToDegreesFactor;
        latitude = NormalizeAngle(latitude);
        longitude = NormalizeAngle(longitude);
        if (latitude > 90)
        {
            latitude = 90;
        }
        if (latitude < -90)
        {
            latitude = -90;
        }
        if (longitude > 180)
        {
            longitude = 180;
        }
        if (longitude < -180)
        {
            longitude = -180;
        }

        return (latitude, longitude);
    }

    public static Coordinate ConvertToLatitudeLongitudePrecise(string utmZone, double easting, double northing, double altitude = 0)
    {
        (double latitude, double longitude) = ConvertToLatitudeLongitudePrecise(utmZone, easting, northing);
        return new Coordinate()
        {
            Latitude = latitude,
            Longitude = longitude,
            Altitude = altitude,
            TimeStamp = DateTime.Now
        };
    }

    public static (double latitude, double longitude) ConvertToLatitudeLongitude(string utmZone, int easting, int northing)
    {
        return ConvertToLatitudeLongitudePrecise(utmZone, easting, northing);
    }

    public static Coordinate ConvertToLatitudeLongitude(string utmZone, int easting, int northing, double altitude = 0)
    {
        (double latitude, double longitude) = ConvertToLatitudeLongitude(utmZone, easting, northing);
        return new Coordinate()
        {
            Latitude = latitude,
            Longitude = longitude,
            Altitude = altitude,
            TimeStamp = DateTime.Now
        };
    }

    private static double NormalizeAngle(double angle)
    {
        // force it to be the positive remainder, so that 0 <= angle < 360  
        angle = (angle + 360) % 360;

        // force into the minimum absolute value residue class, so that -180 < angle <= 180  
        if (angle > 180)
        {
            angle -= 360;
        }

        return angle;
    }

    private static (string latitudeZone, int longitudeZone) GetUTMZoneFromLatLong(double latitude, double longitude)
    {
        int longitudeZone = (int)(1.0 + Math.Floor((longitude + 180.0) / 6.0));
        longitudeZone = (longitudeZone % 61) + 1;
        string latitudeZone = latitude switch
        {
            < -72 => "C",
            < -64 => "D",
            < -56 => "E",
            < -48 => "F",
            < -40 => "G",
            < -32 => "H",
            < -24 => "J",
            < -16 => "K",
            < -8 => "L",
            < 0 => "M",
            < 8 => "N",
            < 16 => "P",
            < 24 => "Q",
            < 32 => "R",
            < 40 => "S",
            < 48 => "T",
            < 56 => "U",
            < 64 => "V",
            < 72 => "W",
            _ => "X"
        };
        return (latitudeZone, longitudeZone);
    }

    private static (char latitudeZone, int longitudeZone) GetUTMZoneComponents(string utmZone)
    {
        if (utmZone.Length is < 2 or > 4)
        {
            throw new ArgumentException("Invalid UTM zone", nameof(utmZone));
        }
        char latitudeZone;
        if (char.IsLetter(utmZone[0]) && int.TryParse(utmZone[1..], out int longitudeZone))
        {
            latitudeZone = char.ToUpperInvariant(utmZone[0]);
        }
        else if (char.IsLetter(utmZone[^1]) && int.TryParse(utmZone[..^1], out longitudeZone))
        {
            latitudeZone = char.ToUpperInvariant(utmZone[^1]);
        }
        else
        {
            throw new ArgumentException("Invalid UTM zone", nameof(utmZone));
        }
        if (longitudeZone is < 1 or > 60)
        {
            throw new ArgumentOutOfRangeException(nameof(utmZone), "Longitude zone must be between 1 and 60");
        }

        return (latitudeZone, longitudeZone);
    }
}
