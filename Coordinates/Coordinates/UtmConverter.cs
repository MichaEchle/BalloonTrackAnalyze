using System;
using static System.Math;

namespace Coordinates
{
    /// <summary>
    /// Ellipsoid-agnostic UTM forward / inverse conversion using the Krüger
    /// series expansion in the third flattening n = f / (2 − f).  Sixth-order
    /// terms are retained, which keeps the worst-case projection error well
    /// below one millimetre anywhere inside the 6° zone — for the latitudes a
    /// hot-air balloon ever sees, the error is at the nanometre level.
    ///
    /// The same code services both <c>UTM_WGS84</c> and <c>UTM_ETRS89</c>:
    /// only the supplied ellipsoid changes.  GRS80 (ETRS89) and WGS84 differ
    /// only in the 9th significant digit of the flattening, so the
    /// <em>projection</em> step is numerically identical to within roughly a
    /// nanometre.  The <em>datum</em> offset between current-epoch WGS84 and
    /// ETRS89 in Europe is ≈ 0.5 m today and grows about 2.5 cm per year, but
    /// it is far below GPS receiver noise and well below the minimum
    /// measuring distances used for balloon scoring, so it is intentionally
    /// not corrected here.  If sub-decimetre geodesy is ever required, plug a
    /// Helmert transformation in between the two ellipsoids.
    /// </summary>
    public static class UtmConverter
    {
        public readonly struct Ellipsoid
        {
            public readonly double A;   // semi-major axis [m]
            public readonly double F;   // flattening (1 / inverse)

            public Ellipsoid(double a, double f)
            {
                A = a;
                F = f;
            }

            public static readonly Ellipsoid Wgs84 = new(6_378_137.0, 1.0 / 298.257_223_563);
            public static readonly Ellipsoid Grs80 = new(6_378_137.0, 1.0 / 298.257_222_101);
        }

        private const double K0 = 0.9996;
        private const double FalseEasting = 500_000.0;
        private const double FalseNorthingSouth = 10_000_000.0;

        /// <summary>
        /// Project geographic coordinates onto UTM.  The zone number is
        /// derived from the longitude unless <paramref name="forceZone"/> is
        /// supplied (useful when working on the seam of two zones).
        /// </summary>
        public static (int zoneNumber, char zoneLetter, double easting, double northing)
            GeographicToUtm(double latitudeDeg, double longitudeDeg, Ellipsoid ellipsoid,
                            int? forceZone = null)
        {
            if (latitudeDeg < -80.0 || latitudeDeg > 84.0)
                throw new ArgumentOutOfRangeException(nameof(latitudeDeg),
                    "UTM is only defined for latitudes between 80°S and 84°N.");

            int zone = forceZone ?? (int)Floor((longitudeDeg + 180.0) / 6.0) + 1;
            double lambda0 = ((zone - 1) * 6.0 - 180.0 + 3.0) * PI / 180.0;

            double phi = latitudeDeg * PI / 180.0;
            double lambda = longitudeDeg * PI / 180.0;

            double f = ellipsoid.F;
            double a = ellipsoid.A;
            double n = f / (2.0 - f);
            double n2 = n * n;
            double n3 = n2 * n;
            double n4 = n3 * n;
            double n5 = n4 * n;
            double n6 = n5 * n;

            // Rectifying radius A = a/(1+n) · Σ (n^{2k} · (1/4)^k binomial term).
            double rectifyingA = a / (1.0 + n) *
                                 (1.0 + n2 / 4.0 + n4 / 64.0 + n6 / 256.0);

            // Krüger α coefficients (6th order, from Karney 2011 eq. 35).
            double[] alpha =
            {
                1.0 / 2.0 * n - 2.0 / 3.0 * n2 + 5.0 / 16.0 * n3 + 41.0 / 180.0 * n4
                    - 127.0 / 288.0 * n5 + 7891.0 / 37800.0 * n6,
                13.0 / 48.0 * n2 - 3.0 / 5.0 * n3 + 557.0 / 1440.0 * n4
                    + 281.0 / 630.0 * n5 - 1983433.0 / 1935360.0 * n6,
                61.0 / 240.0 * n3 - 103.0 / 140.0 * n4 + 15061.0 / 26880.0 * n5
                    + 167603.0 / 181440.0 * n6,
                49561.0 / 161280.0 * n4 - 179.0 / 168.0 * n5 + 6601661.0 / 7257600.0 * n6,
                34729.0 / 80640.0 * n5 - 3418889.0 / 1995840.0 * n6,
                212378941.0 / 319334400.0 * n6
            };

            double e = Sqrt(f * (2.0 - f));

            // Conformal latitude on the auxiliary sphere.
            double sinPhi = Sin(phi);
            double t = Sinh(Atanh(sinPhi) - e * Atanh(e * sinPhi));

            double dLambda = lambda - lambda0;
            double xiPrime = Atan2(t, Cos(dLambda));
            double etaPrime = Asinh(Sin(dLambda) / Sqrt(t * t + Cos(dLambda) * Cos(dLambda)));

            double xi = xiPrime;
            double eta = etaPrime;
            for (int j = 1; j <= 6; j++)
            {
                xi += alpha[j - 1] * Sin(2.0 * j * xiPrime) * Cosh(2.0 * j * etaPrime);
                eta += alpha[j - 1] * Cos(2.0 * j * xiPrime) * Sinh(2.0 * j * etaPrime);
            }

            double easting = FalseEasting + K0 * rectifyingA * eta;
            double northing = K0 * rectifyingA * xi;
            if (latitudeDeg < 0.0)
                northing += FalseNorthingSouth;

            return (zone, ZoneLetter(latitudeDeg), easting, northing);
        }

        /// <summary>
        /// Recover WGS84-style geographic coordinates from a UTM position.
        /// The ellipsoid must match the one used for the forward step.
        /// </summary>
        public static (double latitudeDeg, double longitudeDeg) UtmToGeographic(
            int zoneNumber, char zoneLetter, double easting, double northing, Ellipsoid ellipsoid)
        {
            bool northern = zoneLetter >= 'N';
            double x = easting - FalseEasting;
            double y = northern ? northing : northing - FalseNorthingSouth;

            double f = ellipsoid.F;
            double a = ellipsoid.A;
            double n = f / (2.0 - f);
            double n2 = n * n;
            double n3 = n2 * n;
            double n4 = n3 * n;
            double n5 = n4 * n;
            double n6 = n5 * n;

            double rectifyingA = a / (1.0 + n) *
                                 (1.0 + n2 / 4.0 + n4 / 64.0 + n6 / 256.0);

            // Krüger β coefficients (inverse series, Karney 2011 eq. 36).
            double[] beta =
            {
                1.0 / 2.0 * n - 2.0 / 3.0 * n2 + 37.0 / 96.0 * n3 - 1.0 / 360.0 * n4
                    - 81.0 / 512.0 * n5 + 96199.0 / 604800.0 * n6,
                1.0 / 48.0 * n2 + 1.0 / 15.0 * n3 - 437.0 / 1440.0 * n4 + 46.0 / 105.0 * n5
                    - 1118711.0 / 3870720.0 * n6,
                17.0 / 480.0 * n3 - 37.0 / 840.0 * n4 - 209.0 / 4480.0 * n5
                    + 5569.0 / 90720.0 * n6,
                4397.0 / 161280.0 * n4 - 11.0 / 504.0 * n5 - 830251.0 / 7257600.0 * n6,
                4583.0 / 161280.0 * n5 - 108847.0 / 3991680.0 * n6,
                20648693.0 / 638668800.0 * n6
            };

            double xi = y / (K0 * rectifyingA);
            double eta = x / (K0 * rectifyingA);

            double xiPrime = xi;
            double etaPrime = eta;
            for (int j = 1; j <= 6; j++)
            {
                xiPrime -= beta[j - 1] * Sin(2.0 * j * xi) * Cosh(2.0 * j * eta);
                etaPrime -= beta[j - 1] * Cos(2.0 * j * xi) * Sinh(2.0 * j * eta);
            }

            double e = Sqrt(f * (2.0 - f));
            double e2 = e * e;

            // τ' = tan of the conformal latitude on the auxiliary sphere.
            // Karney (2011) eq. 19:  τ' = sin(ξ') / sqrt(sinh²(η') + cos²(ξ')).
            double tauPrime = Sin(xiPrime) / Sqrt(Sinh(etaPrime) * Sinh(etaPrime)
                                                  + Cos(xiPrime) * Cos(xiPrime));

            // Newton iteration on Karney's eq. 23 / 24 (well-conditioned for
            // every latitude including the equator and the poles):
            //     τ_φ(φ) = sin φ · √(1 + σ²)  −  cos φ · σ
            //     σ      = sinh( e · atanh(e · sin φ) )
            //     dτ_φ / dφ = (1 − e²) · √(1 + τ_φ²) · sec²φ
            //                 / (1 + (1 − e²) · tan²φ)
            // We iterate until the absolute residual is below ≈ 1e-12 m on
            // the ground (about three steps in practice).
            double tau = tauPrime / (1.0 - e2);   // good seed
            for (int i = 0; i < 16; i++)
            {
                double sinPhi = tau / Sqrt(1.0 + tau * tau);
                double sigma = Sinh(e * Atanh(e * sinPhi));
                double tauI = tau * Sqrt(1.0 + sigma * sigma)
                              - sigma * Sqrt(1.0 + tau * tau);
                double dTau = (1.0 - e2) * Sqrt(1.0 + tauI * tauI)
                              / (Sqrt(1.0 + tau * tau) * (1.0 + sigma * sigma)
                                 - tau * sigma);
                double delta = (tauPrime - tauI) * (1.0 + (1.0 - e2) * tau * tau)
                               / ((1.0 - e2) * Sqrt(1.0 + tau * tau) * Sqrt(1.0 + tauI * tauI));
                _ = dTau;
                tau += delta;
                if (Abs(delta) < 1e-14) break;
            }
            double phi = Atan(tau);

            double lambda0 = ((zoneNumber - 1) * 6.0 - 180.0 + 3.0) * PI / 180.0;
            double lambda = lambda0 + Atan2(Sinh(etaPrime), Cos(xiPrime));

            return (phi * 180.0 / PI, lambda * 180.0 / PI);
        }

        /// <summary>
        /// Return the MGRS-style latitude band letter for a given latitude.
        /// </summary>
        public static char ZoneLetter(double latitudeDeg)
        {
            if (latitudeDeg < -80.0 || latitudeDeg > 84.0)
                throw new ArgumentOutOfRangeException(nameof(latitudeDeg));

            const string letters = "CDEFGHJKLMNPQRSTUVWXX";
            int index = (int)Floor((latitudeDeg + 80.0) / 8.0);
            if (index < 0) index = 0;
            if (index > 20) index = 20;
            return letters[index];
        }
    }
}
