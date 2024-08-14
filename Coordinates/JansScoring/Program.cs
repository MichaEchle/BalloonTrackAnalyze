namespace JansScoring
{
    class Programm
    {
        private static FlightManager _flightManager;

        static void Main(string[] args)
        {
           /*
            Coordinate convertUtmToLatitudeLongitudeCoordinate = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 495600, 5367000);
            Console.WriteLine(convertUtmToLatitudeLongitudeCoordinate.Longitude + " | " + convertUtmToLatitudeLongitudeCoordinate.Latitude );

            Converter.GeoCoordinates geoCoordinates = Converter.UTMToGeo(32,"U", 495600, 5367000);
            Console.WriteLine(geoCoordinates.Longitude + " | " + geoCoordinates.Latitude);

            return;
            */


            _flightManager = new FlightManager();
            _flightManager.register();
        }
    }
}