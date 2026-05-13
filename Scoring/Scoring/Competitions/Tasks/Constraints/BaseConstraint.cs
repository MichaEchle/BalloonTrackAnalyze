namespace Scoring.Competitions.Tasks.Constraints;

/// <summary>
/// Abstract template method base class for implementing constraints.
/// </summary>
/// <remarks>
/// <para>
/// Provides a generic evaluation loop that drives constraint checking based on evaluation order, return criteria, and infringement action.
/// Concrete constraint subclasses define their own infringement check and penalty calculation logic via lambdas.
/// </para>
/// <para>
/// The template method scans targets according to <see cref="EvaluationOrder"/>, accumulates infringement amounts, and applies the configured
/// <see cref="InfringementAction"/> and <see cref="ReturnCriteria"/> to determine which target (if any) to return and what penalty to apply.
/// </para>
/// </remarks>
/// <typeparam name="T">The type of constraint targets.</typeparam>
public abstract class BaseConstraint<T> : IConstraint<T>
{
	/// <inheritdoc/>
	public virtual required EvaluationOrderType EvaluationOrder { get; init; }

	/// <inheritdoc/>
	public virtual required InfringementActionType InfringementAction { get; init; }

	/// <inheritdoc/>
	public virtual required ReturnCriteriaType ReturnCriteria { get; init; }

	/// <inheritdoc/>
	public virtual required List<T> ConstraintTargets { get; init; }

	/// <inheritdoc/>
	public abstract (T? selectedOrNull, double penalty) CheckConstraint();

	/// <summary>
	/// Template method for evaluating targets against a custom infringement check and penalty calculation.
	/// </summary>
	/// <remarks>
	/// Scans the targets in order (determined by <see cref="EvaluationOrder"/>), checking each with the
	/// infringement check lambda. Returns the selected target and computed penalty according to
	/// <see cref="ReturnCriteria"/> and <see cref="InfringementAction"/>.
	/// </remarks>
	/// <typeparam name="U">The infringement amount type (e.g., <see langword="double"/>, <see langword="int"/>).</typeparam>
	/// <param name="infringementCheck">
	/// A function that returns <see langword="true"/> if the target has an infringement and the infringement amount.
	/// </param>
	/// <param name="penaltyCalculation">
	/// A function that converts an infringement amount to a penalty value.
	/// </param>
	/// <returns>
	/// A tuple containing the selected target (or <see langword="null"/>) and the total penalty.
	/// </returns>
	public virtual (T? selectedOrNull, double penalty) CheckConstraint<U>(Func<T, (bool hasInfringement, U infringementAmount)> infringementCheck, Func<U, int> penaltyCalculation)
	{
		if (EvaluationOrder == EvaluationOrderType.Last)
		{
			ConstraintTargets.Reverse();
		}
		double penalty = 0;
		T? selectedOrNull = default;
		for (int i = 0; i < ConstraintTargets.Count; i++)
		{
			(bool hasInfringement, U infringementAmount) = infringementCheck(ConstraintTargets[i]);
			if (!hasInfringement)
			{
				selectedOrNull = ConstraintTargets[i];
				break;
			}
			else
			{
				switch (ReturnCriteria, InfringementAction)
				{
					case (ReturnCriteriaType.WithoutInfringement, InfringementActionType.InfringementInvalidates):
					case (ReturnCriteriaType.WithoutInfringement, InfringementActionType.InfringementPenalizes):
						continue;
					case (ReturnCriteriaType.WithInfringement, InfringementActionType.InfringementInvalidates):
						break;
					case (ReturnCriteriaType.WithInfringement, InfringementActionType.InfringementPenalizes):
						selectedOrNull = ConstraintTargets[i];
						penalty = penaltyCalculation(infringementAmount);
						break;
				}
			}

		}
		return (selectedOrNull, penalty);
	}
}
