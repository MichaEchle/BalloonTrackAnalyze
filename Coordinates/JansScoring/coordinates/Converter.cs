using System;

namespace JansScoring.coordinates;

public class Converter
{
    public struct UTMCoordinates
    {
        public double Easting;
        public double Northing;
        public string ZoneLetter;
        public int ZoneNumber;

        public UTMCoordinates(int zoneNumber, string zoneLetter, double easting, double northing)
        {
            Easting = easting;
            Northing = northing;
            ZoneLetter = zoneLetter;
            ZoneNumber = zoneNumber;
        }
    }

    public struct GeoCoordinates
    {
        public double Latitude;
        public double Longitude;
    }

    public static UTMCoordinates GeoToUTM(double latitude, double longitude)
    {
        double semiMajorAxis = 6378137; // WGS 84 semi-major axis
        double eccentricity = 0.081819191; // WGS 84 eccentricity

        int zoneNumber = (int)Math.Floor((longitude + 180.0) / 6) + 1;

        double latRad = latitude * (Math.PI / 180);
        double lonRad = longitude * (Math.PI / 180);

        double k0 = 0.9996;
        double e2 = Math.Pow(eccentricity, 2);
        double e4 = Math.Pow(eccentricity, 4);
        double e6 = Math.Pow(eccentricity, 6);

        double N = semiMajorAxis / Math.Sqrt(1 - e2 * Math.Pow(Math.Sin(latRad), 2));
        double T = Math.Pow(Math.Tan(latRad), 2);
        double C = e2 * Math.Pow(Math.Cos(latRad), 2);
        double A = (lonRad - ((6 * (zoneNumber - 1) - 180 + 3) * Math.PI / 180)) * Math.Cos(latRad);
        double M = semiMajorAxis * ((1 - e2 / 4 - 3 * e4 / 64 - 5 * e6 / 256) * latRad -
                                    (3 * e2 / 8 + 3 * e4 / 32 + 45 * e6 / 1024) * Math.Sin(2 * latRad) +
                                    (15 * e4 / 256 + 45 * e6 / 1024) * Math.Sin(4 * latRad) -
                                    35 * e6 / 3072 * Math.Sin(6 * latRad));

        double easting = k0 * N * (A + (1 - T + C) * Math.Pow(A, 3) / 6 +
                                   (5 - 18 * T + T * T + 72 * C - 58 * e2) * Math.Pow(A, 5) / 120) + 500000;

        double northing = k0 * (M + N * Math.Tan(latRad) * (Math.Pow(A, 2) / 2 +
                                                            (5 - T + 9 * C + 4 * C * C) * Math.Pow(A, 4) / 24 +
                                                            (61 - 58 * T + T * T + 600 * C - 330 * e2) *
                                                            Math.Pow(A, 6) / 720));

        if (latitude < 0)
        {
            northing += 10000000; // Southern hemisphere
        }

        string zoneLetter = GetUTMZoneLetter(latitude);

        return new UTMCoordinates
        {
            Easting = easting, Northing = northing, ZoneNumber = zoneNumber, ZoneLetter = zoneLetter,
        };
    }

    public static GeoCoordinates UTMToGeo( int zoneNumber, string zoneLetter, double easting, double northing)
    {
         double semiMajorAxis = 6378137; // WGS 84 semi-major axis
        double eccentricity = 0.081819191; // WGS 84 eccentricity
        double e2 = Math.Pow(eccentricity, 2);
        double e4 = Math.Pow(eccentricity, 4);
        double e6 = Math.Pow(eccentricity, 6);

        double k0 = 0.9996;
        double E1 = (1 - Math.Sqrt(1 - e2)) / (1 + Math.Sqrt(1 - e2));
        double N1 = semiMajorAxis / Math.Sqrt(1 - e2 * Math.Pow(Math.Sin(easting / (k0 * 0.9996)), 2));

        double rho = Math.Sqrt(Math.Pow(easting - 500000, 2) + Math.Pow(northing, 2));
        double mu = rho / (N1 * k0);

        double footLat = mu + (3 * E1 / 2 - 27 * Math.Pow(E1, 3) / 32) * Math.Sin(2 * mu) +
            (21 * E1 * E1 / 16 - 55 * Math.Pow(E1, 4) / 32) * Math.Sin(4 * mu) +
            (151 * Math.Pow(E1, 3) / 96) * Math.Sin(6 * mu) + (1097 * Math.Pow(E1, 4) / 512) * Math.Sin(8 * mu);

        double C1 = e2 * Math.Pow(Math.Cos(footLat), 2);
        double T1 = Math.Pow(Math.Tan(footLat), 2);
        double N1p = semiMajorAxis / Math.Sqrt(1 - e2 * Math.Pow(Math.Sin(footLat), 2));
        double R1 = semiMajorAxis * (1 - e2) / Math.Pow(1 - e2 * Math.Pow(Math.Sin(footLat), 2), 1.5);
        double D = (northing) / (N1p * k0);

        double latRad = footLat - (N1p * Math.Tan(footLat) / R1) *
            (Math.Pow(D, 2) / 2 - (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * e2) * Math.Pow(D, 4) / 24 +
            (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 252 * e2 - 3 * C1 * C1) * Math.Pow(D, 6) / 720);

        double latitude = latRad * (180 / Math.PI);
        double longitude = (zoneNumber - 1) * 6 - 180 + 3 + (6 * (zoneNumber - 1) - 180 + 3) * Math.PI / 180 * Math.Sin(footLat) * Math.Tan(latRad) *
            (Math.Pow(D, 2) / 2 + (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * e2) * Math.Pow(D, 4) / 24 +
            (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 252 * e2 - 3 * C1 * C1) * Math.Pow(D, 6) / 720) / Math.Cos(footLat);

        if (zoneLetter.ToLower() == "n")
        {
            latitude = -latitude;
        }

        return new GeoCoordinates
        {
            Latitude = latitude,
            Longitude = longitude
        };
    }

    private static string GetUTMZoneLetter(double latitude)
    {
        if (latitude >= -80 && latitude < 84)
        {
            string[] letters = "CDEFGHJKLMNPQRSTUVWX".ToLower().Split("");
            int index = (int)Math.Floor((latitude + 80) / 8);
            return letters[index];
        }
        else
        {
            throw new ArgumentOutOfRangeException("Latitude out of UTM zone bounds");
        }
    }
}