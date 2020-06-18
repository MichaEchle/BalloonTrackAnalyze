using System;
using System.Collections.Generic;
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
        public List<DeclaredGoal> DeclaredGoals { get; private set; } = new List<DeclaredGoal>();

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


        public Track()
        {

        }

    }
}
