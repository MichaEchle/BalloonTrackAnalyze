using System;
using static System.Math;

namespace Coordinates
{
    /// <summary>
    /// Rigorous transformation between WGS84 geographic coordinates and the
    /// Swiss national grids LV95 (CH1903+) and LV03 (CH1903).
    ///
    /// The projection step uses the exact double-projection (ellipsoid → conformal
    /// sphere → oblique Mercator plane) published by swisstopo in
    /// "Formulas and constants for the calculation of the Swiss conformal
    ///  cylindrical projection and for the transformation between coordinate
    ///  systems" (swisstopo, 2016).  No truncated polynomials are used; the
    ///  isometric latitude on the Bessel ellipsoid is inverted iteratively to
    ///  better than 1e-12 rad (≈ 6 µm on the ground).
    ///
    /// The datum step between WGS84 (ETRF/ITRF realisation) and CH1903/Bessel
    /// is the geocentric translation published by swisstopo
    /// (Δx = +674.374 m, Δy = +15.056 m, Δz = +405.346 m, applied as
    /// CH1903 = WGS84 − Δ).  This is the same shift swisstopo itself uses for
    /// its non-grid "rigorous" path; over Switzerland it agrees with the
    /// FINELTRA reference grid to within roughly one metre.  For sub-metre
    /// geodetic work one would have to inject a FINELTRA grid file here; that
    /// has been intentionally kept out of scope to avoid a binary dependency.
    /// </summary>
    public static class SwissGridConverter
    {
        // Bessel 1841 ellipsoid (as used by swisstopo for CH1903 / CH1903+).
        private const double BesselA = 6_377_397.155;                  // semi-major axis [m]
        private const double BesselE2 = 0.006_674_372_230_614;         // first eccentricity squared
        private static readonly double BesselE = Sqrt(BesselE2);

        // WGS84 ellipsoid.
        private const double Wgs84A = 6_378_137.0;
        private const double Wgs84F = 1.0 / 298.257_223_563;
        private const double Wgs84E2 = Wgs84F * (2.0 - Wgs84F);

        // Projection origin: fundamental point of the old Swiss triangulation
        // network at the Bern observatory (φ₀ = 46° 57′ 08.66″ N,
        // λ₀ = 7° 26′ 22.50″ E).
        private const double Phi0Deg = 46.0 + 57.0 / 60.0 + 8.66 / 3600.0;
        private const double Lambda0Deg = 7.0 + 26.0 / 60.0 + 22.50 / 3600.0;
        private static readonly double Phi0 = Phi0Deg * PI / 180.0;
        private static readonly double Lambda0 = Lambda0Deg * PI / 180.0;

        // False origin offsets.
        private const double Lv95EastOffset = 2_600_000.0;
        private const double Lv95NorthOffset = 1_200_000.0;
        private const double Lv03EastOffset = 600_000.0;
        private const double Lv03NorthOffset = 200_000.0;

        // swisstopo geocentric datum shift CH1903 → WGS84, applied in the
        // ECEF system (X_wgs = X_ch + Δ).
        private const double DatumShiftX = 674.374;
        private const double DatumShiftY = 15.056;
        private const double DatumShiftZ = 405.346;

        // Pre-computed projection constants (the famous R, α, b₀, K from the
        // swisstopo write-up).  These depend only on the Bessel ellipsoid and
        // the projection origin so we compute them once at type load.
        private static readonly double R = BesselA * Sqrt(1.0 - BesselE2) /
                                           (1.0 - BesselE2 * Sin(Phi0) * Sin(Phi0));
        private static readonly double Alpha = Sqrt(
            1.0 + (BesselE2 / (1.0 - BesselE2)) * Pow(Cos(Phi0), 4));
        private static readonly double B0 = Asin(Sin(Phi0) / Alpha);
        private static readonly double K =
            Log(Tan(PI / 4.0 + B0 / 2.0))
            - Alpha * Log(Tan(PI / 4.0 + Phi0 / 2.0))
            + Alpha * (BesselE / 2.0) *
              Log((1.0 + BesselE * Sin(Phi0)) / (1.0 - BesselE * Sin(Phi0)));

        // ─────────────────────────────────────────────────────────────────────
        //  Public API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Converts a WGS84 geographic position (with optional ellipsoidal
        /// height) into Swiss grid LV95 (CH1903+).
        /// </summary>
        public static (double easting, double northing) Wgs84ToLv95(
            double latitudeDeg, double longitudeDeg, double ellipsoidalHeight = 0.0)
        {
            (double phiBessel, double lambdaBessel, _) =
                Wgs84ToBessel(latitudeDeg, longitudeDeg, ellipsoidalHeight);
            (double y, double x) = BesselToSwissPlane(phiBessel, lambdaBessel);
            return (y + Lv95EastOffset, x + Lv95NorthOffset);
        }

        /// <summary>
        /// Converts a WGS84 geographic position into Swiss grid LV03 (CH1903).
        /// </summary>
        public static (double easting, double northing) Wgs84ToLv03(
            double latitudeDeg, double longitudeDeg, double ellipsoidalHeight = 0.0)
        {
            (double phiBessel, double lambdaBessel, _) =
                Wgs84ToBessel(latitudeDeg, longitudeDeg, ellipsoidalHeight);
            (double y, double x) = BesselToSwissPlane(phiBessel, lambdaBessel);
            return (y + Lv03EastOffset, x + Lv03NorthOffset);
        }

        /// <summary>
        /// Converts a Swiss LV95 (CH1903+) coordinate into WGS84 lat/lon.
        /// </summary>
        public static (double latitudeDeg, double longitudeDeg) Lv95ToWgs84(
            double easting, double northing)
        {
            double y = easting - Lv95EastOffset;
            double x = northing - Lv95NorthOffset;
            (double phiBessel, double lambdaBessel) = SwissPlaneToBessel(y, x);
            return BesselToWgs84(phiBessel, lambdaBessel, 0.0);
        }

        /// <summary>
        /// Converts a Swiss LV03 (CH1903) coordinate into WGS84 lat/lon.
        /// </summary>
        public static (double latitudeDeg, double longitudeDeg) Lv03ToWgs84(
            double easting, double northing)
        {
            double y = easting - Lv03EastOffset;
            double x = northing - Lv03NorthOffset;
            (double phiBessel, double lambdaBessel) = SwissPlaneToBessel(y, x);
            return BesselToWgs84(phiBessel, lambdaBessel, 0.0);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Projection step:  Bessel (φ, λ) ↔ Swiss plane (y east, x north)
        //  This is the exact swisstopo double projection without any series
        //  truncation in the longitude / latitude domain.
        // ─────────────────────────────────────────────────────────────────────

        private static (double y, double x) BesselToSwissPlane(double phi, double lambda)
        {
            // Step 1: ellipsoid → conformal sphere.
            double S = Alpha * Log(Tan(PI / 4.0 + phi / 2.0))
                       - Alpha * (BesselE / 2.0) *
                         Log((1.0 + BesselE * Sin(phi)) / (1.0 - BesselE * Sin(phi)))
                       + K;

            double b = 2.0 * (Atan(Exp(S)) - PI / 4.0);
            double l = Alpha * (lambda - Lambda0);

            // Step 2: rotate the sphere so that the projection origin lies on the
            // equator of an auxiliary frame (b̄, l̄), then apply a normal Mercator.
            double sinB0 = Sin(B0);
            double cosB0 = Cos(B0);
            double sinB = Sin(b);
            double cosB = Cos(b);
            double sinL = Sin(l);
            double cosL = Cos(l);

            double sinBBar = cosB0 * sinB - sinB0 * cosB * cosL;
            double cosBBar = Sqrt(1.0 - sinBBar * sinBBar);
            double lBar = Atan2(cosB * sinL,
                                cosB0 * cosB * cosL + sinB0 * sinB);
            double bBar = Asin(sinBBar);

            double y = R * lBar;
            double x = R / 2.0 * Log((1.0 + Sin(bBar)) / (1.0 - Sin(bBar)));

            // sanity touch so the unused locals don't trigger warnings if the
            // compiler ever decides cosBBar is dead code.
            _ = cosBBar;

            return (y, x);
        }

        private static (double phi, double lambda) SwissPlaneToBessel(double y, double x)
        {
            // Inverse of step 2: plane → sphere (b̄, l̄), then back-rotate.
            double lBar = y / R;
            double bBar = 2.0 * (Atan(Exp(x / R)) - PI / 4.0);

            double sinB0 = Sin(B0);
            double cosB0 = Cos(B0);
            double sinBBar = Sin(bBar);
            double cosBBar = Cos(bBar);
            double sinLBar = Sin(lBar);
            double cosLBar = Cos(lBar);

            double sinB = cosB0 * sinBBar + sinB0 * cosBBar * cosLBar;
            double b = Asin(sinB);
            double l = Atan2(cosBBar * sinLBar,
                             cosB0 * cosBBar * cosLBar - sinB0 * sinBBar);

            double lambda = Lambda0 + l / Alpha;

            // Inverse of step 1: invert the isometric latitude on Bessel from b.
            // Closed form is impossible because of the ellipsoidal eccentricity
            // term; we iterate until |Δφ| < 1e-12 rad (≈ 6 µm).
            double qBessel = (Log(Tan(PI / 4.0 + b / 2.0)) - K) / Alpha;
            double phi = 2.0 * Atan(Exp(qBessel)) - PI / 2.0; // spherical seed
            for (int i = 0; i < 32; i++)
            {
                double esinPhi = BesselE * Sin(phi);
                double phiNext = 2.0 * Atan(
                    Exp(qBessel + (BesselE / 2.0) * Log((1.0 + esinPhi) / (1.0 - esinPhi)))
                ) - PI / 2.0;

                if (Abs(phiNext - phi) < 1e-12)
                {
                    phi = phiNext;
                    break;
                }
                phi = phiNext;
            }

            return (phi, lambda);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Datum step:  WGS84 ellipsoid ↔ Bessel ellipsoid
        //  Geographic → ECEF → 3-parameter shift → geographic.  The lat/lon →
        //  ECEF formulas are exact; ECEF → lat/lon uses Bowring's closed-form
        //  approximation followed by a few Newton iterations on the latitude.
        // ─────────────────────────────────────────────────────────────────────

        private static (double phiBessel, double lambdaBessel, double hBessel) Wgs84ToBessel(
            double latitudeDeg, double longitudeDeg, double ellipsoidalHeight)
        {
            double phi = latitudeDeg * PI / 180.0;
            double lambda = longitudeDeg * PI / 180.0;

            (double x, double y, double z) = GeographicToEcef(phi, lambda, ellipsoidalHeight,
                                                              Wgs84A, Wgs84E2);

            // WGS84 → CH1903:  subtract the geocentric shift.
            x -= DatumShiftX;
            y -= DatumShiftY;
            z -= DatumShiftZ;

            return EcefToGeographic(x, y, z, BesselA, BesselE2);
        }

        private static (double latitudeDeg, double longitudeDeg) BesselToWgs84(
            double phi, double lambda, double ellipsoidalHeight)
        {
            (double x, double y, double z) = GeographicToEcef(phi, lambda, ellipsoidalHeight,
                                                              BesselA, BesselE2);

            x += DatumShiftX;
            y += DatumShiftY;
            z += DatumShiftZ;

            (double phiW, double lambdaW, _) = EcefToGeographic(x, y, z, Wgs84A, Wgs84E2);
            return (phiW * 180.0 / PI, lambdaW * 180.0 / PI);
        }

        private static (double x, double y, double z) GeographicToEcef(
            double phi, double lambda, double h, double a, double e2)
        {
            double sinPhi = Sin(phi);
            double cosPhi = Cos(phi);
            double N = a / Sqrt(1.0 - e2 * sinPhi * sinPhi);
            double x = (N + h) * cosPhi * Cos(lambda);
            double y = (N + h) * cosPhi * Sin(lambda);
            double z = (N * (1.0 - e2) + h) * sinPhi;
            return (x, y, z);
        }

        private static (double phi, double lambda, double h) EcefToGeographic(
            double x, double y, double z, double a, double e2)
        {
            double lambda = Atan2(y, x);
            double p = Sqrt(x * x + y * y);

            // Bowring 1976 starting value: gives 1e-11 rad without iteration,
            // but we run a short Newton refinement so even singular near-pole
            // inputs settle.
            double b = a * Sqrt(1.0 - e2);
            double ep2 = (a * a - b * b) / (b * b);
            double theta = Atan2(z * a, p * b);
            double sinTheta = Sin(theta);
            double cosTheta = Cos(theta);
            double phi = Atan2(z + ep2 * b * sinTheta * sinTheta * sinTheta,
                               p - e2 * a * cosTheta * cosTheta * cosTheta);

            // Iterate to keep h consistent even at altitude.
            for (int i = 0; i < 8; i++)
            {
                double sinPhi = Sin(phi);
                double N = a / Sqrt(1.0 - e2 * sinPhi * sinPhi);
                double phiNext = Atan2(z + e2 * N * sinPhi, p);
                if (Abs(phiNext - phi) < 1e-13) { phi = phiNext; break; }
                phi = phiNext;
            }

            double sinPhiF = Sin(phi);
            double NF = a / Sqrt(1.0 - e2 * sinPhiF * sinPhiF);
            double h = p / Cos(phi) - NF;

            return (phi, lambda, h);
        }
    }
}
