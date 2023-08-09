using System;

namespace JansScoring
{
    class Programm
    {
        private static FlightManager _flightManager;

        static void Main(string[] args)
        {
            _flightManager = new FlightManager();
            _flightManager.register();
            
            //_flightManager.scoreFlight(4);
        }
    }
}