using Competition;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Bson;
using OfficeOpenXml.Export.ToDataTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Tegernsee_2026;

internal class Flight3
{
    private readonly ILogger<Flight3> Logger = LogConnector.LoggerFactory.CreateLogger<Flight3>();
    internal readonly Flight _flight = Flight.GetInstance();
    internal readonly double _separationAltitude = CoordinateHelpers.ConvertToMeter(3500);
    internal Coordinate Task8_FIN = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 703921, 5295035, 762);
    internal Coordinate Task9_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 702064, 5296322, 773);
    internal Coordinate Task9_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 701297, 5295944, 769);

    internal void ScoreFlight()
    {
        Logger.LogInformation("Scoring flight 3");
        _flight.FlightNumber = 3;
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\flight_03\tracks\scoring", true, null, defaultGoalAltitude: _separationAltitude))
            Console.WriteLine("Failed to parse track files");
        _flight.MapPilotNamesToTracks(@"C:\Users\micechle\Nextcloud\2026 DM Tegernsee\software\DM_Tegernsee2026_Pilots.csv");
        List<Track> tracks = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber)];
    }

    internal void Task8(List<Track> tracks)
    {
        //TODO check ILP to goal <1000
        //TODO check launch till 05:15
        //TODO check marker drop1 before 06:15
        //TODO calculate marker 1 to goal with separation alt, best result is 50m
    }

    internal void Task9(List<Track> tracks)
    {
        //TODO check ILP to goals <1000
        //TODO check marker drop2 before 06:30
        //TODO calculate marker 2 to goals with separation alt, take better result, but best result is 50m
    }

    internal void Task10(List<Track> tracks)
    {
        //TODO check declaration after marker drop 2
        //TODO take first declaration
        //TODO calculate 2D distance between first track point 15min after timestamp of position at declaration and declared goal
    }

    internal void Task11(List<Track> tracks)
    {
        //TODO check ILP to goal <1000
        //TODO only consider trackpoints after reference trackpoint from T10
        //TODO only consider trackpoints after declared goal 2
        //TODO only consider trackpoints till 07:30
        //TODO check distance between declaration and declared goal <2500
        //TODO take declaration with first trackpoint within cake, compare timing of first trackpoint in cake with time of next declaration
        //TODO calculate weighted 2D distance in cake
    }

    internal void Task12(List<Track> tracks)
    {
        //TODO check distance between declaration and declared goal <1000
        //TODO check declared goal 3 not in cake
        //TODO check ILP to goal <1000
        //TODO check distance between declared goal and goal T8 <1000
        //TODO check distance between declared goal and goal T9 <1000
        //TODO check distance between declared goal and goal 10 <1000
        //TODO check distance between declared goal and goal T11 <1000
        //TODO check marker drop 3 before 07:45
        //TODO calculate 3D distance between declared goal 3 and marker 3
    }

}
