namespace Scoring.Competitions.Tasks.Constraints;
public abstract class BaseConstraint<T> : IConstraint<T>
{
    public virtual required EvaluationOrderType EvaluationOrder { get; init; }
    public virtual required InfringementActionType InfringementAction { get; init; }
    public virtual required ReturnCriteriaType ReturnCriteria { get; init; }
    public virtual required List<T> ConstraintTargets { get; init; }

    public abstract (T? selectedOrNull, double penalty) CheckConstraint();

    public virtual (T? selectedOrNull, double penaltiy) CheckConstraint<U>(Func<T, (bool hasInfringement, U infrigementAmount)> infringementCheck, Func<U, int> penaltyCalculation)
    {
        if (EvaluationOrder == EvaluationOrderType.Last)
        {
            ConstraintTargets.Reverse();
        }
        double penalty = 0;
        T? selectedOrNull = default;
        for (int i = 0; i < ConstraintTargets.Count; i++)
        {
            (bool hasInfringement, U infrigementAmount) = infringementCheck(ConstraintTargets[i]);
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
                        penalty = penaltyCalculation(infrigementAmount);
                        break;
                }
            }

        }
        return (selectedOrNull, penalty);
    }
}
