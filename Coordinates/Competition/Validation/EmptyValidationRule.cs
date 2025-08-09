using Coordinates;

namespace Competition.Validation;

public class EmptyValidationRule : IDeclarationValidationRule
{
    public bool IsComplaintToRule(Declaration declaration)
    {
        return true;
    }
}