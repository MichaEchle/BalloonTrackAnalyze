namespace Scoring.Competitions.Tasks.Constraints;


public enum EvaluationOrderType
{
    First,
    Last
}

public enum InfringementActionType
{
    InfringementInvalidates,
    InfringementPenalizes
}

public enum ReturnCriteriaType
{
    WithoutInfringement,
    WithInfringement
}

/// <summary>
/// Generic interface for constraints that defines the evaluation order, the action to take in case of infringement, the criteria of the evaluation and the targets of the constraint.
/// </summary>
/// <para>First, WithoutInfringement, InfringementInvalidates=> checks until the first valid is found, otherwise null is returned</para>
/// <para>First, WithoutInfringement, InfringementPenalizes=> checks until the first valid is found, otherwise null is returned </para>
/// <para>First, WithInfringement, InfringementInvalidates=> checks only the first entry, returns it if valid otherwise null</para></para>
/// <para>First, WithInfringement, InfringementPenalizes=> checks until the first valid is found returns it with penalties (high amount of infringement may cause entry to be considered invalid rather then valid but with penalty), otherwise null</para>
/// <typeparam name="T"></typeparam>
public interface IConstraint<T>
{
    public EvaluationOrderType EvaluationOrder
    {
        get; init;
    }

    public InfringementActionType InfringementAction
    {
        get; init;
    }

    public ReturnCriteriaType ReturnCriteria
    {
        get; init;
    }

    public List<T> ConstraintTargets
    {
        get; init;
    }

    public (T? selectedOrNull, double penalty) CheckConstraint();
}




