using Coordinates;

namespace Competition.Validation;
public class DeclarationAndRule : IDeclarationValidationRule
{
    public required List<IDeclarationValidationRule> ValidationRules
    {
        get; set;
    }

    public bool IsComplaintToRule(Declaration declaration)
    {
        bool isConform = true;
        foreach (IDeclarationValidationRule validationRule in ValidationRules)
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

