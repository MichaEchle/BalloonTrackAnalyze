using Coordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapes
{
    public class Circle : Shapes2D
    {
        public Coordinate CenterPoint
        {
            get;private set;
        }

        public double Radius
        {
            get;private set;
        }

        public Circle(Coordinate centerPoint, double radius)
        {
            CenterPoint = centerPoint;
            Radius = radius;
        }

        public override bool IsWithin(Coordinate coordinate)
        {
            return CoordinateHelpers.Calculate2DDistance(CenterPoint, coordinate) <= Radius;
        }
    }
}
