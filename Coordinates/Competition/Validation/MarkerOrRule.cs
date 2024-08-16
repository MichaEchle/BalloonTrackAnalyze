using Coordinates;
using System.Collections.Generic;

namespace Competition.Validation;
public class MarkerOrRule : IMarkerValidationRule
{
    public List<IMarkerValidationRule> ValidationRules
    {
        get; set;
    }

    public bool IsComplaintToRule(MarkerDrop marker)
    {
        bool isConform = false;
        foreach (var validationRule in ValidationRules)
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
