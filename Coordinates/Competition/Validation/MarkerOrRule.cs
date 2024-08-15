using Coordinates;
using System.Collections.Generic;

namespace Competition.Validation;
public class MarkerOrRule : IMarkerValidationRules
{
    public List<IMarkerValidationRules> ValidationRules
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

    public void SetupRule(List<IMarkerValidationRules> rules)
    {
        ValidationRules = rules;
    }
}
