using Coordinates;
using System.Collections.Generic;

namespace Competition.Validation
{
    public class MarkerAndRule : IMarkerValidationRules
    {
        public List<IMarkerValidationRules> ValidationRules
        {
            get; set;
        }

        public bool IsComplaintToRule(MarkerDrop marker)
        {
            bool isConform = true;

            foreach (var validationRule in ValidationRules)
            {
                isConform &= validationRule.IsComplaintToRule(marker);
            }

            return isConform;
        }

        public void SetupRule(List<IMarkerValidationRules> rules)
        {
            ValidationRules = rules;
        }
    }
}
