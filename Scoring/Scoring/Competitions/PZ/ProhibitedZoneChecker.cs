using Scoring.Competitions.Tracks;
using Scoring.Converters;
using Scoring.Coordinates;
using Scoring.Shapes;

namespace Scoring.Competitions.PZ;
internal static class ProhibitedZoneChecker
{
    internal static List<ProhibitedZoneViolation> CheckProhibitedZones(List<ProhibitedZone> prohibitedZones, Track track)
    {
        List<ProhibitedZoneViolation> violations = new ();
        foreach (ProhibitedZone prohibitedZone in prohibitedZones)
        {
            CheckProhibitedZone(prohibitedZone, track, out List<Coordinate> coordinatesInZone, out TimeSpan timeInZone, out int penalties);
            if (coordinatesInZone.Count > 0)
            {
                // add the violation to the list
                violations.Add(new ProhibitedZoneViolation
                {
                    CoordinatesInZone = coordinatesInZone,
                    TimeInZone = timeInZone,
                    Penalties = penalties,
                    Track = track,
                    PZ = prohibitedZone
                });
            }
        }
        return violations;
    }

    private static void CheckProhibitedZone(ProhibitedZone prohibitedZone, Track track, out List<Coordinate> coordinatesInZone, out TimeSpan timeInZone, out int penalties)
    {
        coordinatesInZone = new List<Coordinate>();
        timeInZone = TimeSpan.Zero;
        penalties = 0;
        if (!prohibitedZone.IsActive)
        {
            return;
        }
        foreach (Coordinate trackPoint in track.TrackPoints)
        {
            if (prohibitedZone.ZoneDefinition.IsCoordinateInside(trackPoint))
            {
                coordinatesInZone.Add(trackPoint);
            }
        }
        timeInZone = TimeSpan.FromSeconds(coordinatesInZone.Count * track.TrackPointInterval.TotalSeconds);
        penalties = prohibitedZone.TypeOfZone switch
        {
            ProhibitedZoneType.Red => CalculateRedPZPenalties(prohibitedZone, coordinatesInZone),
            ProhibitedZoneType.Yellow => CalculateYellowPZPenalties(prohibitedZone, coordinatesInZone),
            ProhibitedZoneType.Blue => CalculateBluePZPenalites(prohibitedZone, coordinatesInZone, track.TrackPointInterval),
            _ => 0,
        };
    }

    private static int CalculateRedPZPenalties(ProhibitedZone prohibitedZone, List<Coordinate> coordinatesInZone)
    {
        int penalties = 0;
        switch (prohibitedZone.ZoneDefinition)
        {
            case Cylinder cylinder:
                double verticalDistanceCylinder = CoordinateMath.Calculate2DDistanceBetweenPoints(coordinatesInZone);
                double averageAltitude = coordinatesInZone.Average(c => c.Altitude);
                double verticalPercentageCylinder = verticalDistanceCylinder / (cylinder.Circle.Radius * 2.0) * 100.0;
                double altitudePercentageCylinder = 100.0 - averageAltitude / (cylinder.UpperBoundary - cylinder.LowerBoundary) * 100.0;
                double averagePercentageCylinder = (verticalPercentageCylinder + altitudePercentageCylinder) / 2.0;
                double tempPenalitesCylinder = 500.0 * averagePercentageCylinder;
                penalties = (int)Math.Ceiling(tempPenalitesCylinder / 10.0) * 10;
                break;
            case Sphere sphere:
                // SHPERICAL PZs ARE NOT OFFICIALLY DOCUMENTED
                double verticalDistanceSphere = CoordinateMath.Calculate2DDistanceBetweenPoints(coordinatesInZone);
                double averageAltitudeSphere = coordinatesInZone.Max(c => c.Altitude) - coordinatesInZone.Min(c => c.Altitude);
                double verticalPercentageSphere = verticalDistanceSphere / (sphere.Radius * 2.0) * 100.0;
                double altitudePercentageSphere = 100.0 - averageAltitudeSphere / (sphere.Radius * 2.0) * 100.0;
                double averagePercentageSphere = (verticalPercentageSphere + altitudePercentageSphere) / 2.0;
                double tempPenalitesSphere = 500.0 * averagePercentageSphere;
                penalties = (int)Math.Ceiling(tempPenalitesSphere / 10.0) * 10;
                break;
            case UniformPrism uniformPrism:
                // PRISM PZs ARE NOT OFFICIALLY DOCUMENTED
                double verticalDistancePrism = CoordinateMath.Calculate2DDistanceBetweenPoints(coordinatesInZone);
                double averageAltitudePrism = coordinatesInZone.Max(c => c.Altitude) - coordinatesInZone.Min(c => c.Altitude);
                double verticalPercentagePrism = verticalDistancePrism / uniformPrism.Polygon.CalculateArea() * 100.0;
                double altitudePercentagePrism = 100.0 - averageAltitudePrism / (uniformPrism.UpperBoundary - uniformPrism.LowerBoundary) * 100.0;
                double averagePercentagePrism = (verticalPercentagePrism + altitudePercentagePrism) / 2.0;
                double tempPenalitesPrism = 500.0 * averagePercentagePrism;
                penalties = (int)Math.Ceiling(tempPenalitesPrism / 10.0) * 10;
                break;
            default:
                break;
        }
        return penalties;
    }

    private static int CalculateYellowPZPenalties(ProhibitedZone prohibitedZone, List<Coordinate> coordinatesInZone)
    {
        int penalties = 0;
        switch (prohibitedZone.ZoneDefinition)
        {
            case Cylinder cylinder:
                double verticalDistanceCylinder = CoordinateMath.Calculate2DDistanceBetweenPoints(coordinatesInZone);
                double averageAltitude = coordinatesInZone.Average(c => c.Altitude);
                double verticalPercentageCylinder = verticalDistanceCylinder / (cylinder.Circle.Radius * 2.0) * 100.0;
                double altitudePercentageCylinder = 100.0 - averageAltitude / (cylinder.UpperBoundary - cylinder.LowerBoundary) * 100.0;
                double averagePercentageCylinder = (verticalPercentageCylinder + altitudePercentageCylinder) / 2.0;
                double tempPenalitesCylinder = 250.0 * averagePercentageCylinder;
                penalties = (int)Math.Ceiling(tempPenalitesCylinder / 10.0) * 10;
                break;
            case Sphere sphere:
                // SHPERICAL PZs ARE NOT OFFICIALLY DOCUMENTED
                double verticalDistanceSphere = CoordinateMath.Calculate2DDistanceBetweenPoints(coordinatesInZone);
                double averageAltitudeSphere = coordinatesInZone.Max(c => c.Altitude) - coordinatesInZone.Min(c => c.Altitude);
                double verticalPercentageSphere = verticalDistanceSphere / (sphere.Radius * 2.0) * 100.0;
                double altitudePercentageSphere = 100.0 - averageAltitudeSphere / (sphere.Radius * 2.0) * 100.0;
                double averagePercentageSphere = (verticalPercentageSphere + altitudePercentageSphere) / 2.0;
                double tempPenalitesSphere = 250.0 * averagePercentageSphere;
                penalties = (int)Math.Ceiling(tempPenalitesSphere / 10.0) * 10;
                break;
            case UniformPrism uniformPrism:
                // PRISM PZs ARE NOT OFFICIALLY DOCUMENTED
                double verticalDistancePrism = CoordinateMath.Calculate2DDistanceBetweenPoints(coordinatesInZone);
                double averageAltitudePrism = coordinatesInZone.Max(c => c.Altitude) - coordinatesInZone.Min(c => c.Altitude);
                double verticalPercentagePrism = verticalDistancePrism / uniformPrism.Polygon.CalculateArea() * 100.0;
                double altitudePercentagePrism = 100.0 - averageAltitudePrism / (uniformPrism.UpperBoundary - uniformPrism.LowerBoundary) * 100.0;
                double averagePercentagePrism = (verticalPercentagePrism + altitudePercentagePrism) / 2.0;
                double tempPenalitesPrism = 250.0 * averagePercentagePrism;
                penalties = (int)Math.Ceiling(tempPenalitesPrism / 10.0) * 10;
                break;
            default:
                break;
        }
        return penalties;
    }

    private static int CalculateBluePZPenalites(ProhibitedZone prohibitedZone, List<Coordinate> coordinatesInZone, TimeSpan trackpointInterval)
    {
        int penalties = 0;
        switch (prohibitedZone.ZoneDefinition)
        {
            case Cylinder cylinder:
                double altiudeInfringementCylinder = 0.0;
                foreach (Coordinate coordinateInZone in coordinatesInZone)
                {
                    altiudeInfringementCylinder += Math.Abs(coordinateInZone.Altitude - cylinder.LowerBoundary);
                }
                double altidueInfringementFeetCylinder = AltitudeConverter.ConvertToFeet(altiudeInfringementCylinder);
                double tempPenaltiesCylinder = altidueInfringementFeetCylinder * trackpointInterval.TotalSeconds / 100.0;
                penalties = (int)Math.Ceiling(tempPenaltiesCylinder / 10.0) * 10;

                break;
            case Sphere sphere:
                // SHPERICAL PZs ARE NOT OFFICIALLY DOCUMENTED
                double altiudeInfringementShpere = 0.0;
                foreach (Coordinate coordinateInZone in coordinatesInZone)
                {
                    altiudeInfringementShpere += Math.Abs(coordinateInZone.Altitude - (sphere.CenterPoint.Altitude - sphere.Radius));
                }
                double altidueInfringementFeetShpere = AltitudeConverter.ConvertToFeet(altiudeInfringementShpere);
                double tempPenaltiesShpere = altidueInfringementFeetShpere * trackpointInterval.TotalSeconds / 100.0;
                penalties = (int)Math.Ceiling(tempPenaltiesShpere / 10.0) * 10;

                break;
            case UniformPrism uniformPrism:
                double altiudeInfringementPrism = 0.0;
                foreach (Coordinate coordinateInZone in coordinatesInZone)
                {
                    altiudeInfringementPrism += Math.Abs(coordinateInZone.Altitude - uniformPrism.LowerBoundary);
                }
                double altidueInfringementFeetPrism = AltitudeConverter.ConvertToFeet(altiudeInfringementPrism);
                double tempPenaltiesPrism = altidueInfringementFeetPrism * trackpointInterval.TotalSeconds / 100.0;
                penalties = (int)Math.Ceiling(tempPenaltiesPrism / 10.0) * 10;

                break;
            default:
                break;
        }
        return penalties;

    }
}
