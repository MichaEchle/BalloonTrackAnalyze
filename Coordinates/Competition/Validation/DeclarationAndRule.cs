using Coordinates;
using System.Collections.Generic;

namespace Competition.Validation;
public class DeclarationAndRule : IDeclarationValidationRule
{
    public List<IDeclarationValidationRule> ValidationRules
    {
        get; set;
    }

    public bool IsComplaintToRule(Declaration declaration)
    {
        bool isConform = true;
        foreach (var validationRule in ValidationRules)
        {
            isConform &= validationRule.IsComplaintToRule(declaration);
        }
        return isConform;
    }

    public void SetupRule(List<IDeclarationValidationRule> rules)
    {
        ValidationRules = rules;
    }
}

