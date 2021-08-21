using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coordinates
{
    public class Track
    {
        /// <summary>
        /// The list of track points
        /// </summary>
        public List<Coordinate> TrackPoints { get; private set; } = new List<Coordinate>();

        /// <summary>
        /// The list of declared goals
        /// </summary>
        public List<Declaration> Declarations { get; private set; } = new List<Declaration>();

        /// <summary>
        /// The list of marker drops
        /// </summary>
        public List<MarkerDrop> MarkerDrops { get; private set; } = new List<MarkerDrop>();

        /// <summary>
        /// The pilot which created this track
        /// </summary>
        public Pilot Pilot
        {
            get;set;
        }

        public Dictionary<string, string> AdditionalPropertiesFromIGCFile
        {
            get; private set;
        } = new Dictionary<string, string>();

        public Track()
        {

        }

        public Declaration GetLatestDeclaration( int goalNumber)
        {
            List<Declaration> declarations = Declarations.Where(x => x.GoalNumber == goalNumber).ToList();
            if (declarations.Count == 0)
                return null;
            else
                return declarations.OrderByDescending(x => x.PositionAtDeclaration.TimeStamp).ToList()[0];

        }

        public List<int> GetAllGoalNumbers()
        {
            List<int> allGoalNumbers = Declarations.Select(x => x.GoalNumber).Distinct().ToList();
            return allGoalNumbers;
        }

        public List<int> GetAllMarkerNumbers()
        {
            List<int> allMarkerNumbers = MarkerDrops.Select(x => x.MarkerNumber).Distinct().ToList();
            return allMarkerNumbers;
        }

    }
}
