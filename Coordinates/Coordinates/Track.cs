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
        public List<Coordinate> DeclaredGoals { get; private set; } = new List<Coordinate>();

        /// <summary>
        /// The list of marker drops
        /// </summary>
        public List<Coordinate> MarkerDrops { get; private set; } = new List<Coordinate>();

        /// <summary>
        /// The pilot which created this track
        /// </summary>
        public Pilot Pilot { get; private set; }

        /// <summary>
        /// Create a track for that pilot
        /// </summary>
        /// <param name="pilot">the pilot of that track</param>
        public Track(Pilot pilot)
        {
            Pilot = pilot;
        }

        /// <summary>
        /// Add a track point to the list of track points
        /// </summary>
        /// <param name="trackPoint">the track point</param>
        public void AddTrackPoint(Coordinate trackPoint)
        {
            if (trackPoint is null)
            {
                throw new ArgumentNullException(nameof(trackPoint));
            }
            if (trackPoint.CoordinateType != CoordinateTypes.TrackPoint)
            {
                throw new ArgumentException("The entered coordinate is not a track point");
            }
            TrackPoints.Add(trackPoint);
        }

        /// <summary>
        /// Add a marker drop to the list of marker drops
        /// </summary>
        /// <param name="markerDrop">the marker drop</param>
        public void AddMarkerDrop(Coordinate markerDrop)
        {
            if (markerDrop is null)
            {
                throw new ArgumentNullException(nameof(markerDrop));
            }
            if (markerDrop.CoordinateType != CoordinateTypes.Marker)
            {
                throw new ArgumentException("The entered coordinate is not a marker");
            }
            MarkerDrops.Add(markerDrop);
        }

        /// <summary>
        /// Add a declared goal to the list of declared goals
        /// </summary>
        /// <param name="declaredGoal">the declared goal</param>
        public void AddDeclaredGoal(Coordinate declaredGoal)
        {
            if (declaredGoal is null)
            {
                throw new ArgumentNullException(nameof(declaredGoal));
            }
            if (declaredGoal.CoordinateType != CoordinateTypes.Goal)
            {
                throw new ArgumentException("The entered coordinate is not a goal");
            }
            DeclaredGoals.Add(declaredGoal);
        }
    }
}
