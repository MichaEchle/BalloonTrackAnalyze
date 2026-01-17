using System;

namespace Coordinates;

public class AltitudeHelpers
{


    //CREATED BY KI (not intelligent :))


    // Constants for ISA troposphere
    private const double P0 = 1013.25;      // hPa, standard sea-level pressure
    private const double T0 = 288.15;       // K, standard sea-level temperature
    private const double L  = 0.0065;       // K/m, lapse rate
    private const double g0 = 9.80665;      // m/s^2, gravity
    private const double R  = 287.05287;    // J/(kg·K), specific gas constant for dry air

    // Convert geometric altitude (m) to pressure (hPa) under ISA
    private static double PressureFromAltitude(double hMeters)
    {
        // p = P0 * (1 - L*h / T0)^(g0/(R*L))
        double exponent = g0 / (R * L);
        double term = 1.0 - (L * hMeters) / T0;
        if (term <= 0) term = 1e-9; // guard
        return P0 * Math.Pow(term, exponent);
    }

    // Convert pressure (hPa) to altitude (m) under ISA
    private static double AltitudeFromPressure(double pHpa)
    {
        // h = (T0/L) * [1 - (p/P0)^(R*L/g0)]
        double n = (R * L) / g0;
        return (T0 / L) * (1.0 - Math.Pow(pHpa / P0, n));
    }

    // Correct indicated/barometric altitude (set to STD 1013.25) to QNH-referenced altitude
    // altitudeMeters: altitude indicated with standard setting (FL altitude) in meters
    // qnhHpa: local QNH in hPa
    public static double CorrectAltitudeWithQnh(double altitudeMeters, double qnhHpa)
    {
        // 1) Get ISA pressure at the indicated altitude (with STD setting)
        double pAtIndicated = PressureFromAltitude(altitudeMeters);

        // 2) Compute the ratio between local sea-level pressure (QNH) and standard
        // Under QNH setting, the same static pressure would correspond to a different indicated altitude.
        // We want the altitude that, under ISA, would yield the same static pressure
        // when sea-level pressure is QNH instead of P0.
        //
        // The pressure at altitude h under QNH can be modeled by replacing P0 with QNH in the ISA relation:
        // p = QNH * (1 - L*h/T0)^(g0/(R*L))
        //
        // We know p (actual static pressure) ≈ pAtIndicated; solve for h using QNH as the sea-level pressure:
        // h_corrected = (T0/L) * [1 - (p / QNH)^(R*L/g0)]
        double n = (R * L) / g0;
        double hCorrected = (T0 / L) * (1.0 - Math.Pow(pAtIndicated / qnhHpa, n));

        return hCorrected;
    }

    // Convenience: quick linear approximation near sea level:
    // About 27–30 ft per hPa ≈ 8.2–9.1 m per hPa. Use ~8.5 m/hPa if you need a rough estimate:
    public static double CorrectAltitudeWithQnhApprox(double altitudeMeters, double qnhHpa)
    {
        const double metersPerHpa = 8.5; // rough near-sea-level
        double deltaH = (qnhHpa - P0) * metersPerHpa;
        return altitudeMeters - deltaH;
    }
}