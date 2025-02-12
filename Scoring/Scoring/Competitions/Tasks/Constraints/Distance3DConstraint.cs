using Scoring.Coordinates;

namespace Scoring.Competitions.Tasks.Constraints;
internal class Distance3DConstraint : BaseConstraint<Coordinate>, IConstraint<Coordinate>
{
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

    public override (Coordinate? selectedOrNull, double penalty) CheckConstraint()
    {
        return CheckConstraint(InfringementCheck, PenaltyCalculation);
    }

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
                //TODO: check if infringement should be in percent rather than absolute value
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

    private int PenaltyCalculation(double infringementAmount)
    {
        //TODO convert infringement to penalty
        throw new NotImplementedException();
    }
}