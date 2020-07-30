using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public class Flight
    {
        #region Properties

        /// <summary>
        /// The number of the flight
        /// </summary>
        public int FlightNumber { get; set; } = -1;

        /// <summary>
        /// The list of tasks for that flight
        /// </summary>
        public List<ICompetitionTask> Tasks { get; set; } = new List<ICompetitionTask>();

        /// <summary>
        /// The list of tracks form the pilots
        /// </summary>
        public List<Track> Tracks { get; set; } = new List<Track>();
        #endregion

        #region Singleton

        /// <summary>
        /// private instance object
        /// </summary>
        private static Flight flight=null;

        /// <summary>
        /// Lock object for thread safety
        /// </summary>
        private static readonly object lockObject = new object();

        /// <summary>
        /// private constructor for singleton pattern
        /// </summary>
        private Flight()
        {

        }
        #endregion

        #region API
        /// <summary>
        /// Get the one and only instance
        /// </summary>
        /// <returns>the instance of flight</returns>
        public static Flight GetInstance()
        {
            lock (lockObject)
            {
                if (flight == null)
                    flight = new Flight();
                return flight;
            }
        }
        #endregion
    }
}
