using Coordinates;

namespace Competition.Validation;

public interface IMarkerValidationRule
{
    bool IsComplaintToRule(MarkerDrop marker);
}
