using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public interface ITask
    {
        public int TaskNumber { get; set; }
        public double CalculateResults(Track track,bool useGPSAltiude);
    }
}
