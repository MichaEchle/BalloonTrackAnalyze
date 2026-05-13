using Scoring.Coordinates;

namespace Scoring.Competitions.Tasks.Constraints;

/// <summary>
/// Constraint that evaluates 3D (horizontal and vertical) distance targets against minimum and maximum distance requirements.
/// </summary>
/// <remarks>
/// <para>
/// This constraint checks whether the 3D distance from a target coordinate (including altitude) to one or more reference coordinates
/// falls within a specified range. 3D distance includes both Haversine (2D) distance and altitude difference.
/// Infringement amounts measure how far the distance falls outside the allowed range.
/// </para>
/// <para>
/// Use <see cref="double.NaN"/> to indicate an unset limit.
/// </para>
/// </remarks>
internal class Distance3DConstraint : BaseConstraint<Coordinate>, IConstraint<Coordinate>
{
	/// <summary>Gets the list of reference coordinates used for 3D distance calculations.</summary>
	/// <remarks>
	/// 3D distance is measured from the target coordinate to each reference coordinate.
	/// </remarks>
	/// <value>A list of reference <see cref="Coordinate"/> objects.</value>
	public required List<Coordinate> ConstraintReferences
	{
		get;
		init;
	}

	/// <summary>Gets the minimum allowed 3D distance in meters, or <see cref="double.NaN"/> if not set.</summary>
	/// <value>Minimum distance in meters, or <see cref="double.NaN"/>.</value>
	public required double MinimumDistance
	{
		get;
		init;
	}

	/// <summary>Gets the maximum allowed 3D distance in meters, or <see cref="double.NaN"/> if not set.</summary>
	/// <value>Maximum distance in meters, or <see cref="double.NaN"/>.</value>
	public required double MaximumDistance
	{
		get;
		init;
	}

	/// <inheritdoc/>
	public override (Coordinate? selectedOrNull, double penalty) CheckConstraint()
	{
		return CheckConstraint(InfringementCheck, PenaltyCalculation);
	}

	/// <summary>
	/// Checks whether a target coordinate has a 3D distance infringement and calculates the amount.
	/// </summary>
	/// <param name="coordinate">The target coordinate to check.</param>
	/// <returns>A tuple indicating infringement and the infringement amount.</returns>
	private (bool, double) InfringementCheck(Coordinate coordinate)
	{
		bool hasInfringement = false;
		double infringementAmount = 0;
		bool isMinimumDistanceSet = !double.IsNaN(MinimumDistance);
		bool isMaximumDistanceSet = !double.IsNaN(MaximumDistance);
		foreach (var reference in ConstraintReferences)
		{
			double distance = CoordinateMath.Calculate3DDistance(coordinate, reference);
			switch (isMinimumDistanceSet, isMaximumDistanceSet)
			{
				case (true, true):
					hasInfringement |= distance < MinimumDistance || distance > MaximumDistance;
					if (distance < MinimumDistance)
					{
						infringementAmount += MinimumDistance - distance;
					}
					else if (distance > MaximumDistance)
					{
						infringementAmount += distance - MaximumDistance;
					}
					break;
				case (true, false):
					hasInfringement |= distance < MinimumDistance;
					infringementAmount += MinimumDistance - distance;
					break;
				case (false, true):
					hasInfringement |= distance > MaximumDistance;
					infringementAmount += distance - MaximumDistance;
					break;
				case (false, false):
					hasInfringement |= false;
					break;
			}
		}
		return (hasInfringement, infringementAmount);
	}

	/// <summary>
	/// Converts a 3D distance infringement amount to a penalty value.
	/// </summary>
	/// <remarks>
	/// Currently not implemented; throws <see cref="NotImplementedException"/>.
	/// </remarks>
	/// <param name="infringementAmount">The infringement amount in meters.</param>
	/// <returns>Penalty points.</returns>
	/// <exception cref="NotImplementedException">Always thrown.</exception>
	private int PenaltyCalculation(double infringementAmount)
	{
		throw new NotImplementedException();
	}
}