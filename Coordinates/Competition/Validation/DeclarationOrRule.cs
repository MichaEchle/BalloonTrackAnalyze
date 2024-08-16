using Coordinates;
using System.Collections.Generic;

namespace Competition.Validation;
public class DeclarationOrRule : IDeclarationValidationRule
{
    public List<IDeclarationValidationRule> ValidationRules
    {
        get; set;
    }

    public bool IsComplaintToRule(Declaration declaration)
    {
        bool isConform = false;
        foreach (var validationRule in ValidationRules)
        {
            isConform |= validationRule.IsComplaintToRule(declaration);
        }
        return isConform;
    }

    public void SetupRule(List<IDeclarationValidationRule> rules)
    {
        ValidationRules = rules;
    }
}
