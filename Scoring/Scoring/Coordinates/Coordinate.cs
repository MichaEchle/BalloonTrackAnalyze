using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring.Coordinates;
public class Coordinate
{
    public required double Longitude
    {
        get; init;
    }

    public required double Latitude
    {
        get; init;
    }

    public required double Altitude
    {
        get; init;
    }

    public DateTime? TimeStamp
    {
        get; init;
    }
}
