using Coordinates;
using System.Collections.Generic;

namespace Competition.Validation;
public class DeclarationAndRule : IDeclarationValidationRules
{
    public List<IDeclarationValidationRules> ValidationRules
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

    public void SetupRule(List<IDeclarationValidationRules> rules)
    {
        ValidationRules = rules;
    }
}

