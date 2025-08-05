using Coordinates;

namespace Competition.Validation;

public interface IDeclarationValidationRule
{
    bool IsComplaintToRule(Declaration declaration);
}
