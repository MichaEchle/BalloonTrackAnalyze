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
            get; private set;
        }

        /// <summary>
        /// The target or goal which has been declared
        /// </summary>
        public Coordinate DeclaredGoal
        {
            get; private set;
        }

        /// <summary>
        /// Flag to indicate whether the pilot has declared the goal with an altitude or the position only
        /// </summary>
        public bool HasPilotDelaredGoalAltitude
        {
            get; private set;
        }

        public int OrignalEastingDeclarationUTM
        {
            get; private set;
        }

        public int OrignalNorhtingDeclarationUTM
        {
            get; private set;
        }

        /// <summary>
        /// The position at which the goal has been declared
        /// </summary>
        public Coordinate PositionAtDeclaration
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a gaol
        /// </summary>
        /// <param name="goalNumber">The number of the goal</param>
        /// <param name="declaredGoal">The target or goal which has been declared</param>
        /// <param name="positionAtDeclaration">The position at which the goal has been declared</param>
        public Declaration(int goalNumber, Coordinate declaredGoal, Coordinate positionAtDeclaration, bool hasPilotDelaredGoalAltitude,int orignalEastingDeclarationUTM,int orignalNorhtingDeclarationUTM)
        {
            GoalNumber = goalNumber;
            DeclaredGoal = declaredGoal;
            PositionAtDeclaration = positionAtDeclaration;
            HasPilotDelaredGoalAltitude = hasPilotDelaredGoalAltitude;
            OrignalEastingDeclarationUTM = orignalEastingDeclarationUTM;
            OrignalNorhtingDeclarationUTM = orignalNorhtingDeclarationUTM;
        }

        public Declaration Clone()
        {
            return new Declaration(GoalNumber, DeclaredGoal, PositionAtDeclaration, HasPilotDelaredGoalAltitude, OrignalEastingDeclarationUTM, OrignalNorhtingDeclarationUTM);
        }
    }
}
