using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public interface IDeclarationValidationRules
    {
        public bool CheckConformance(DeclaredGoal declaredGoal);
    }
}
