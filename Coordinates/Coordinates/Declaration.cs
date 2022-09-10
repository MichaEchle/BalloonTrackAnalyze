using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinates
{
    public class Declaration


    {
        /// <summary>
        /// The number of the goal
        /// </summary>
        public int GoalNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// The target or goal which has been declared
        /// </summary>
        public Coordinate DeclaredGoal
        {
            get;
            private set;
        }

        /// <summary>
        /// The position at which the goal has been declared
        /// </summary>
        public Coordinate PositionAtDeclaration
        {
            get;
            private set;
        }
        

        /// <summary>
        /// Creates a gaol
        /// </summary>
        /// <param name="goalNumber">The number of the goal</param>
        /// <param name="declaredGoal">The target or goal which has been declared</param>
        /// <param name="positionAtDeclaration">The position at which the goal has been declared</param>
        public Declaration(int goalNumber, Coordinate declaredGoal, Coordinate positionAtDeclaration)
        {
            GoalNumber = goalNumber;
            DeclaredGoal = declaredGoal;
            PositionAtDeclaration = positionAtDeclaration;
        }
    }
}