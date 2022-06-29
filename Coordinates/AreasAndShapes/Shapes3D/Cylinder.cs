using Coordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapes
{
    public class Cylinder : Shapes3D
    {
        public Circle Circle
        {
            get; private set;
        }

        public double LowerBoundary
        {
            get; private set;
        }

        public double UpperBoundary
        {
            get; private set;
        }

        public Cylinder(Circle circle, double lowerBoundary, double upperBoundary)
        {
            Circle = circle;
            LowerBoundary = lowerBoundary;
            UpperBoundary = upperBoundary;
        }

        public override bool IsWithin(Coordinate coordinate, bool useGPSAltitude)
        {
            if (!Circle.IsWithin(coordinate))
                return false;
            return IsWithinAltitudeBoundary(coordinate, useGPSAltitude);
        }

        private bool IsWithinAltitudeBoundary(Coordinate coordinate, bool useGPSAltitude)
        {
            double altitude = coordinate.AltitudeGPS;
            if (!useGPSAltitude)
                altitude = coordinate.AltitudeBarometric;

            bool isWithin = true;
            if (!double.IsNaN(LowerBoundary))
                isWithin &= altitude >= LowerBoundary;
            if (!double.IsNaN(UpperBoundary))
                isWithin &= altitude <= UpperBoundary;
            return isWithin;

        }
    }
}
