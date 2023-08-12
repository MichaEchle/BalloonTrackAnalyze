using Coordinates;
using JansScoring.flights.impl._01;
using JansScoring.pz_rework.type;
using System;

namespace JansScoring
{
    class Programm
    {
        private static FlightManager _flightManager;

        static void Main(string[] args)
        {
            Flight01 flight01 = new Flight01();
            BluePZ bluePz = new BluePZ(1,
                @"C:\Users\Jan M\OneDrive\Ballonveranstaltungen\2023\2023 Bayr. Meisterschaft\maps\EDDN_D_3500ft.plt",
                Int32.MinValue, Int32.MaxValue);
            Console.WriteLine(bluePz.IsInsidePz(flight01, null,
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 623219, 5495666,
                    CoordinateHelpers.ConvertToMeter(1453)), out String comment1) + comment1); //OUT
            Console.WriteLine(bluePz.IsInsidePz(flight01, null,
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 627975, 5493015,
                    CoordinateHelpers.ConvertToMeter(1453)), out String comment2) + comment2); //IN

            return;
            _flightManager = new FlightManager();
            _flightManager.register();
        }
    }
}