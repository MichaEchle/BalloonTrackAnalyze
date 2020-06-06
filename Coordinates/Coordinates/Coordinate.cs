using System;

namespace Coordinates
{
    public class Coordinate
    {
        public double Longitude { get; private set; }

        public double Latitude { get; private set; }

        public double Altitude { get; private set; }

        public DateTime TimeStamp { get; set; }

    }
}
