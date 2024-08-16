using Coordinates;

namespace Competition
{
    public interface IDeclarationValidationRule
    {
        public bool IsComplaintToRule(Declaration declaration);
    }
}
