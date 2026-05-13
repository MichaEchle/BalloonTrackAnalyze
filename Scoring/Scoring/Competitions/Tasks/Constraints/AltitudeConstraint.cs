namespace Scoring.Competitions.Tasks.Constraints;

/// <summary>
/// Constraint that evaluates altitude targets against minimum and maximum altitude requirements.
/// </summary>
/// <remarks>
/// <para>
/// This constraint checks whether a target altitude falls within a specified range.
/// Infringement amounts measure how far the altitude falls outside the allowed range.
/// </para>
/// <para>
/// Use <see cref="double.NaN"/> to indicate an unset limit.
/// </para>
/// </remarks>
internal class AltitudeConstraint : BaseConstraint<double>, IConstraint<double>
{
	/// <summary>Gets the list of reference altitudes used for comparisons.</summary>
	/// <remarks>
	/// The target altitude is compared against each reference altitude to determine infringement.
	/// </remarks>
	/// <value>A list of reference altitudes in meters.</value>
	public required List<double> ConstraintReferences
	{
		get;
		init;
	}

	/// <summary>Gets the minimum allowed altitude difference in meters, or <see cref="double.NaN"/> if not set.</summary>
	/// <value>Minimum altitude difference, or <see cref="double.NaN"/>.</value>
	public required double MinimumAltitude
	{
		get;
		init;
	}

	/// <summary>Gets the maximum allowed altitude difference in meters, or <see cref="double.NaN"/> if not set.</summary>
	/// <value>Maximum altitude difference, or <see cref="double.NaN"/>.</value>
	public required double MaximumAltitude
	{
		get;
		init;
	}

	/// <inheritdoc/>
	public override (double selectedOrNull, double penalty) CheckConstraint()
	{
		return CheckConstraint(InfringementCheck, PenaltyCalculation);
	}

	/// <summary>
	/// Checks whether a target altitude has an infringement and calculates the amount.
	/// </summary>
	/// <param name="altitude">The target altitude to check.</param>
	/// <returns>A tuple indicating infringement and the infringement amount.</returns>
	private (bool, double) InfringementCheck(double altitude)
	{
		bool hasInfringement = false;
		double infringementAmount = 0;
		bool isMinimumAltitudeSet = !double.IsNaN(MinimumAltitude);
		bool isMaximumAltitudeSet = !double.IsNaN(MaximumAltitude);
		foreach (var referenceAltitude in ConstraintReferences)
		{
			double altitudeDiffererence = altitude - referenceAltitude;
			switch (isMinimumAltitudeSet, isMaximumAltitudeSet)
			{
				case (true, true):
					hasInfringement |= altitudeDiffererence < MinimumAltitude || altitudeDiffererence > MaximumAltitude;
					if (altitudeDiffererence < MinimumAltitude)
					{
						infringementAmount += MinimumAltitude - altitudeDiffererence;
					}
					else if (altitudeDiffererence > MaximumAltitude)
					{
						infringementAmount += altitudeDiffererence - MaximumAltitude;
					}
					break;
				case (true, false):
					hasInfringement |= altitudeDiffererence < MinimumAltitude;
					infringementAmount += MinimumAltitude - altitudeDiffererence;
					break;
				case (false, true):
					hasInfringement |= altitudeDiffererence > MaximumAltitude;
					infringementAmount += altitudeDiffererence - MaximumAltitude;
					break;
				case (false, false):
					hasInfringement |= false;
					break;
			}
		}
		return (hasInfringement, infringementAmount);
	}

	/// <summary>
	/// Converts an altitude infringement amount to a penalty value.
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
