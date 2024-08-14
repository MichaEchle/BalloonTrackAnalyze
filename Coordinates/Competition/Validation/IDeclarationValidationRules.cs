using Coordinates;

namespace Competition
{
    public interface IDeclarationValidationRules
    {
        public bool IsComplaintToRule(Declaration declaration);
    }
}
