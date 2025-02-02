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

public enum EvaluationReturnType
{
    WithoutInfringement,
    WithInfringement
}

public interface IConstraint<T> where T : class?
{
    public EvaluationOrderType EvaluationOrder
    {
        get; init;
    }

    public InfringementActionType InfringementAction
    {
        get; init;
    }

    public EvaluationReturnType EvaluationReturnType
    {
        get; init;
    }

    public List<T> ConstraintTargets
    {
        get; init;
    }

    public (T? selectedOrNull, double penalty) CheckConstraint();
}

//TODO How to create an API to cover these cases:
// Use first no matter what
// Use last no matter what
// Use first with no infringement
// Use last with no infringement
// Use first with infringement
// Use last with infringement


