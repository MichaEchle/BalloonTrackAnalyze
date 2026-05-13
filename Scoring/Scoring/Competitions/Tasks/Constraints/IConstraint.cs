namespace Scoring.Competitions.Tasks.Constraints;

/// <summary>
/// Specifies the order in which constraint targets are evaluated.
/// </summary>
public enum EvaluationOrderType
{
	/// <summary>Evaluate targets from first to last in the list.</summary>
	First,
	/// <summary>Evaluate targets from last to first in the list.</summary>
	Last
}

/// <summary>
/// Specifies the action to take when a constraint infringement is detected.
/// </summary>
public enum InfringementActionType
{
	/// <summary>An infringement invalidates the target (target is rejected).</summary>
	InfringementInvalidates,
	/// <summary>An infringement results in a penalty but does not invalidate the target.</summary>
	InfringementPenalizes
}

/// <summary>
/// Specifies the criteria for which targets are returned by constraint evaluation.
/// </summary>
public enum ReturnCriteriaType
{
	/// <summary>Return only targets with no infringement.</summary>
	WithoutInfringement,
	/// <summary>Return targets even if they have an infringement (and compute penalties).</summary>
	WithInfringement
}

/// <summary>
/// Generic interface for evaluating constraints against a list of targets.
/// </summary>
/// <remarks>
/// <para>
/// Constraint evaluation combines three orthogonal properties:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <see cref="EvaluationOrderType"/>: Which direction to scan the targets (First or Last).
/// </description></item>
/// <item><description>
/// <see cref="ReturnCriteriaType"/>: Accept only valid targets or also return infringements.
/// </description></item>
/// <item><description>
/// <see cref="InfringementActionType"/>: Whether infringements invalidate or penalize.
/// </description></item>
/// </list>
/// <para>
/// Behavior examples:
/// </para>
/// <list type="bullet">
/// <item><description>
/// First + WithoutInfringement + InfringementInvalidates: Returns first valid target, skipping infringements.
/// </description></item>
/// <item><description>
/// First + WithInfringement + InfringementPenalizes: Returns first target (valid or infringed) with penalties computed.
/// </description></item>
/// <item><description>
/// Last + WithoutInfringement + InfringementInvalidates: Returns last valid target, scanning backward.
/// </description></item>
/// </list>
/// </remarks>
/// <typeparam name="T">The type of constraint targets (e.g., <see cref="Coordinate"/>, <see langword="double"/>).</typeparam>
public interface IConstraint<T>
{
	/// <summary>Gets the order in which constraint targets are evaluated.</summary>
	/// <value>An <see cref="EvaluationOrderType"/> value.</value>
	public EvaluationOrderType EvaluationOrder
	{
		get; init;
	}

	/// <summary>Gets the action to take when an infringement is detected.</summary>
	/// <value>An <see cref="InfringementActionType"/> value.</value>
	public InfringementActionType InfringementAction
	{
		get; init;
	}

	/// <summary>Gets the criteria for which targets are returned by evaluation.</summary>
	/// <value>A <see cref="ReturnCriteriaType"/> value.</value>
	public ReturnCriteriaType ReturnCriteria
	{
		get; init;
	}

	/// <summary>Gets the list of targets to be evaluated against this constraint.</summary>
	/// <value>A list of targets of type <typeparamref name="T"/>.</value>
	public List<T> ConstraintTargets
	{
		get; init;
	}

	/// <summary>
	/// Evaluates the constraint against its targets.
	/// </summary>
	/// <returns>
	/// A tuple containing the selected target (or <see langword="null"/> if none qualified) and the total penalty.
	/// </returns>
	public (T? selectedOrNull, double penalty) CheckConstraint();
}




