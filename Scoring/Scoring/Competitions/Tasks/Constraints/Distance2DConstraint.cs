using Scoring.Coordinates;

namespace Scoring.Competitions.Tasks.Constraints;

public class Distance2DConstraint : IConstraint<Coordinate>
{

    public required List<Coordinate> ConstraintTargets
    {
        get;
        init;
    }

    public required List<Coordinate> ConstraintReferences
    {
        get;
        init;
    }

    public required double MinimumDistance
    {
        get;
        init;
    }

    public required double MaximumDistance
    {
        get;
        init;
    }

    public required EvaluationOrderType EvaluationOrder
    {
        get; init;
    }

    public required InfringementActionType InfringementAction
    {
        get; init;
    }

    public required EvaluationReturnType EvaluationReturnType
    {
        get; init;
    }

    public (Coordinate? selectedOrNull, double penalty) CheckConstraint()
    {
        if (EvaluationOrder==EvaluationOrderType.Last)
        {
            ConstraintTargets.Reverse();
        }
        bool minimumDistanceSet = !double.IsNaN(MinimumDistance);
        bool maximumDistanceSet = !double.IsNaN(MaximumDistance);
        double penalty = 0;
        Coordinate? selectedOrNull = null;
        for (int i = 0; i < ConstraintTargets.Count; i++)
        {
            //bool isValid = true;
            penalty = 0;
            foreach (var reference in ConstraintReferences)
            {
                var distance = CoordinateMath.Calculate2DDistance(ConstraintTargets[i], reference);
                bool isInBounds = (minimumDistanceSet, maximumDistanceSet) switch
                {
                    (true, true) => distance >= MinimumDistance && distance <= MaximumDistance,
                    (true, false) => distance >= MinimumDistance,
                    (false, true) => distance <= MaximumDistance,
                    (false, false) => true,
                };
                //if (!isInBounds && !InfringementInvalidates)
                //{
                //    //TODO calculate temp penalty and add to penalty
                //}
                //isValid &= (isInBounds) || !InfringementInvalidates;
            }
            //if (((int)ConstraintEvaluation & 0b10) == 0b10) // first or last valid
            //{
            //    if (isValid)
            //    {
            //        selectedOrNull = ConstraintTargets[i];
            //        break;
            //    }
            //}
            //else // first or last
            //{
            //    if (isValid)
            //    {
            //        selectedOrNull = ConstraintTargets[i];
            //    }
            //    break;
            //}
        }
        return (selectedOrNull, penalty);
    }
}


