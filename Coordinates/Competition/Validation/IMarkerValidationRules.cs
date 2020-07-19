using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public interface IMarkerValidationRules
    {
        public bool CheckConformance(MarkerDrop marker);
    }
}
