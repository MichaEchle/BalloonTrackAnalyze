using Coordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Competition
{
    public abstract class CompetitionTask
    {
        public int TaskNumber { get; set; }

        public abstract bool CalculateResults(Track track,bool useGPSAltitude, out double result);

        public DeclaredGoal GetValidGoal(Track track,int goalNumber,List<IDeclarationValidationRules> declarationValidationRules)
        {
            List<DeclaredGoal> declarations= track.DeclaredGoals.Where(x => x.GoalNumber == goalNumber).ToList();
            List<DeclaredGoal> validDeclarations = new List<DeclaredGoal>();
            foreach (DeclaredGoal declaredGoal in declarations)
            {
                bool isValid = true;
                foreach (IDeclarationValidationRules declarationValidationRule in declarationValidationRules)
                {
                    if (!declarationValidationRule.CheckConformance(declaredGoal))
                    {
                        isValid = false;
                        break;
                    }    
                }
                if (isValid)
                    validDeclarations.Add(declaredGoal);
            }
            if (validDeclarations.Count == 0)
                return null;
            else if (validDeclarations.Count == 1)
                return validDeclarations[0];
            else
                return validDeclarations.OrderByDescending(x => x.PositionAtDeclaration.TimeStamp).ToList()[0];        
        }
    }
}
