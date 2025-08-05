using Coordinates;

namespace Competition.Validation;
public class MarkerOrRule : IMarkerValidationRule
{
    public required List<IMarkerValidationRule> ValidationRules
    {
        get; set;
    }

    public bool IsComplaintToRule(MarkerDrop marker)
    {
        bool isConform = false;
        foreach (IMarkerValidationRule validationRule in ValidationRules)
        {
            isConform |= validationRule.IsComplaintToRule(marker);
        }

        return isConform;
    }

    public void SetupRule(List<IMarkerValidationRule> rules)
    {
        ValidationRules = rules;
    }
}
