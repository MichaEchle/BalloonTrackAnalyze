using Coordinates;

namespace Competition.Validation;
public class DeclarationOrRule : IDeclarationValidationRule
{
    public required List<IDeclarationValidationRule> ValidationRules
    {
        get; set;
    }

    public bool IsComplaintToRule(Declaration declaration)
    {
        bool isConform = false;
        foreach (IDeclarationValidationRule validationRule in ValidationRules)
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
