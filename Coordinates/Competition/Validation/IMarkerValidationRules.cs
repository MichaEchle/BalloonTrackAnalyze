using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public interface IMarkerValidationRules
    {
        public bool IsComplaintToRule(MarkerDrop marker);
    }
}
