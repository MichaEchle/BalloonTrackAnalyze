using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinates
{
    public class DeclaredGoal
    {
        /// <summary>
        /// The number of the goal
        /// </summary>
        public int GoalNumber { get; private set; }

        /// <summary>
        /// The target or goal which has been declared
        /// </summary>
        public Coordinate GoalDeclared { get; private set; }

        /// <summary>
        /// The position at which the goal has been declared
        /// </summary>
        public Coordinate PositionAtDeclaration { get; private set; }

        /// <summary>
        /// Creates a gaol
        /// </summary>
        /// <param name="goalNumber">The number of the goal</param>
        /// <param name="goalDeclared">The target or goal which has been declared</param>
        /// <param name="positionAtDeclaration">The position at which the goal has been declared</param>
        public DeclaredGoal(int goalNumber,Coordinate goalDeclared, Coordinate positionAtDeclaration)
        {
            GoalNumber = goalNumber;
            GoalDeclared = goalDeclared;
            PositionAtDeclaration = positionAtDeclaration;
        }

        
    }
}
