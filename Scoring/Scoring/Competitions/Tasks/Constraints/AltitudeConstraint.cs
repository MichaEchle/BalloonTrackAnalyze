namespace Scoring.Competitions.Tasks.Constraints;

internal class AltitudeConstraint : BaseConstraint<double>, IConstraint<double>
{
    public required List<double> ConstraintReferences
    {
        get;
        init;
    }

    public required double MinimumAltitude
    {
        get;
        init;
    }

    public required double MaximumAltitude
    {
        get;
        init;
    }

    public override (double selectedOrNull, double penalty) CheckConstraint()
    {
        return CheckConstraint(InfringementCheck, PenaltyCalculation);
    }

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

    private int PenaltyCalculation(double infringementAmount)
    {
        //TODO convert infringement to penalty
        throw new NotImplementedException();
    }
}
