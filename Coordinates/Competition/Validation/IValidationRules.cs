using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    interface IValidationRules
    {
        public bool CheckConformance(DeclaredGoal declaredGoal);
    }
}
