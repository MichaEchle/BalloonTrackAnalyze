using Scoring.Competitions.Tracks;
using Scoring.Converters;
using Scoring.Coordinates;
using Scoring.Shapes;

namespace Scoring.Competitions.PZ;

/// <summary>
/// Utility for detecting and penalizing prohibited zone violations in flight tracks.
/// </summary>
/// <remarks>
/// <para>
/// This static class checks a pilot's track against a list of prohibited zones and calculates
/// violations and penalties according to zone type:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <see cref="ProhibitedZoneType.Red"/> and <see cref="ProhibitedZoneType.Yellow"/>:
/// Penalties scale with lateral/vertical penetration depth.
/// </description></item>
/// <item><description>
/// <see cref="ProhibitedZoneType.Blue"/>: Penalties scale with altitude infringement and time duration.
/// </description></item>
/// </list>
/// </remarks>
internal static class ProhibitedZoneChecker
{
	/// <summary>
	/// Checks a track against all active prohibited zones and returns detected violations.
	/// </summary>
	/// <remarks>
	/// Only zones with <see cref="ProhibitedZone.IsActive"/> set to <see langword="true"/> are checked.
	/// </remarks>
	/// <param name="prohibitedZones">The list of zones to check.</param>
	/// <param name="track">The track to validate.</param>
	/// <returns>A list of <see cref="ProhibitedZoneViolation"/> objects for each zone that was violated.</returns>
	internal static List<ProhibitedZoneViolation> CheckProhibitedZones(List<ProhibitedZone> prohibitedZones, Track track)
	{
		List<ProhibitedZoneViolation> violations = [];
		foreach (ProhibitedZone prohibitedZone in prohibitedZones)
		{
			CheckProhibitedZone(prohibitedZone, track, out List<Coordinate> coordinatesInZone, out TimeSpan timeInZone, out int penalties);
			if (coordinatesInZone.Count > 0)
			{
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

	/// <summary>
	/// Checks a single track against a single prohibited zone.
	/// </summary>
	/// <remarks>
	/// Returns via <see langword="out"/> parameters the coordinates in the zone, time in zone, and penalties.
	/// </remarks>
	/// <param name="prohibitedZone">The zone to check.</param>
	/// <param name="track">The track to validate.</param>
	/// <param name="coordinatesInZone">Output: coordinates found inside the zone.</param>
	/// <param name="timeInZone">Output: total time spent in the zone.</param>
	/// <param name="penalties">Output: penalty points for the violation.</param>
	private static void CheckProhibitedZone(ProhibitedZone prohibitedZone, Track track, out List<Coordinate> coordinatesInZone, out TimeSpan timeInZone, out int penalties)
	{
		coordinatesInZone = [];
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

	/// <summary>
	/// Calculates penalties for a Red prohibited zone violation.
	/// </summary>
	/// <remarks>
	/// Red zone penalties are the highest severity; they scale with both lateral penetration and altitude infringement.
	/// </remarks>
	/// <param name="prohibitedZone">The red zone.</param>
	/// <param name="coordinatesInZone">Coordinates inside the zone.</param>
	/// <returns>Penalty points.</returns>
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
				double verticalDistanceSphere = CoordinateMath.Calculate2DDistanceBetweenPoints(coordinatesInZone);
				double averageAltitudeSphere = coordinatesInZone.Max(c => c.Altitude) - coordinatesInZone.Min(c => c.Altitude);
				double verticalPercentageSphere = verticalDistanceSphere / (sphere.Radius * 2.0) * 100.0;
				double altitudePercentageSphere = 100.0 - averageAltitudeSphere / (sphere.Radius * 2.0) * 100.0;
				double averagePercentageSphere = (verticalPercentageSphere + altitudePercentageSphere) / 2.0;
				double tempPenalitesSphere = 500.0 * averagePercentageSphere;
				penalties = (int)Math.Ceiling(tempPenalitesSphere / 10.0) * 10;
				break;
			case UniformPrism uniformPrism:
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

	/// <summary>
	/// Calculates penalties for a Yellow prohibited zone violation.
	/// </summary>
	/// <remarks>
	/// Yellow zone penalties are intermediate severity; they scale similarly to Red zones but with lower multipliers.
	/// </remarks>
	/// <param name="prohibitedZone">The yellow zone.</param>
	/// <param name="coordinatesInZone">Coordinates inside the zone.</param>
	/// <returns>Penalty points.</returns>
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
				double verticalDistanceSphere = CoordinateMath.Calculate2DDistanceBetweenPoints(coordinatesInZone);
				double averageAltitudeSphere = coordinatesInZone.Max(c => c.Altitude) - coordinatesInZone.Min(c => c.Altitude);
				double verticalPercentageSphere = verticalDistanceSphere / (sphere.Radius * 2.0) * 100.0;
				double altitudePercentageSphere = 100.0 - averageAltitudeSphere / (sphere.Radius * 2.0) * 100.0;
				double averagePercentageSphere = (verticalPercentageSphere + altitudePercentageSphere) / 2.0;
				double tempPenalitesSphere = 250.0 * averagePercentageSphere;
				penalties = (int)Math.Ceiling(tempPenalitesSphere / 10.0) * 10;
				break;
			case UniformPrism uniformPrism:
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

	/// <summary>
	/// Calculates penalties for a Blue prohibited zone violation.
	/// </summary>
	/// <remarks>
	/// Blue zone penalties are the lowest severity and are based on altitude infringement above the lower boundary.
	/// Penalties scale with duration and altitude excess, converted to feet.
	/// </remarks>
	/// <param name="prohibitedZone">The blue zone.</param>
	/// <param name="coordinatesInZone">Coordinates inside the zone.</param>
	/// <param name="trackpointInterval">The track point interval for time calculation.</param>
	/// <returns>Penalty points.</returns>
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
