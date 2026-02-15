using ABI.Windows.Security.Authentication.Web.Provider;
using Coordinates;
using Coordinates.Parsers;
using JansScoring.pz_rework.type;
using System.Text;

namespace PZTesting
{
    class Program
    {
        public static readonly string Version = "0.0.1";
        //public double horizontalInfringement;
        //public double verticalInfringement;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.Title = "PZ-Testing";
            ClearConsole();

            RequestCompetitionDetails();

            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Press r to restart and any other key to exit...");
                ConsoleKeyInfo input = Console.ReadKey();
                if (input.Key == ConsoleKey.R)
                {
                    Console.WriteLine("Restarting...");
                    Console.WriteLine();
                    RequestFileOrFolder();
                }
                else
                {
                    break;
                }
            }
        }


        private static void RequestCompetitionDetails()
        {
            Console.WriteLine("Please enter the height mode (gps/barometric).");
            Console.Write("> ");
            string? heightMode = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(heightMode))
            {
                ShowError("No height mode given.");
                RequestCompetitionDetails();
                return;
            }

            bool inputGpsMode = heightMode.ToLower() == "gps";
            bool inputBarometricMode = heightMode.ToLower() == "barometric";

            if (!inputGpsMode && !inputBarometricMode)
            {
                ShowError("Invalid height mode given.");
                RequestCompetitionDetails();
                return;
            }

            Console.WriteLine($"Height mode: {heightMode}");
            GpsMode = inputGpsMode;

            if (inputBarometricMode)
            {
                RequestQnh();
            }

            RequestBackupCoordinate();
        }

        private static void RequestBackupCoordinate()
        {
            Console.WriteLine("Please enter the backup coordinate: zone.");
            Console.Write("> ");
            string? zoneString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(zoneString))
            {
                ShowError("No zone given.");
                RequestBackupCoordinate();
                return;
            }

            Console.WriteLine("Please enter the backup coordinate: easting.");
            Console.Write("> ");
            string? eastingString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(eastingString))
            {
                ShowError("No easing given.");
                RequestBackupCoordinate();
                return;
            }

            int easting;
            try
            {
                easting = Convert.ToInt32(eastingString);
            }
            catch (Exception e)
            {
                ShowError($"Invalid easting given: {e.Message}");
                RequestBackupCoordinate();
                return;
            }

            Console.WriteLine("Please enter the backup coordinate: northing.");
            Console.Write("> ");
            string? nothingString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nothingString))
            {
                ShowError("No northing given.");
                RequestBackupCoordinate();
                return;
            }

            int northing;
            try
            {
                northing = Convert.ToInt32(nothingString);
            }
            catch (Exception e)
            {
                ShowError($"Invalid northing given: {e.Message}");
                RequestBackupCoordinate();
                return;
            }

            Console.WriteLine($"Backup coordinate: {zoneString} {easting} {northing}");
            BackupCoordinate = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate(zoneString, easting, northing);
            RequestPz();
        }

        private static Coordinate? BackupCoordinate = null;

        private static void RequestQnh()
        {
            Console.WriteLine("Please enter the qnh (in hPa).");
            Console.Write("> ");
            string? qnhString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(qnhString))
            {
                ShowError("No QNH given.");
                RequestQnh();
                return;
            }

            try
            {
                double inputQnh = Convert.ToDouble(qnhString);
                Console.WriteLine($"QNH: {inputQnh}");
                Qnh = inputQnh;
            }
            catch (Exception e)
            {
                ShowError($"Invalid QNH given: {e.Message}");
                RequestQnh();
            }
        }

        private static bool? GpsMode = null;
        private static double? Qnh = null;


        private static void RequestPz()
        {
            Console.WriteLine("Please enter the type of the pz. (circular/polygonal)");
            Console.Write("> ");
            string? pzTypeString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(pzTypeString))
            {
                ShowError("No pz type given.");
                RequestPz();
                return;
            }

            bool inputCircularPz = pzTypeString.ToLower() == "circular";
            bool inputPolygonalPz = pzTypeString.ToLower() == "polygonal";
            if (!inputCircularPz && !inputPolygonalPz)
            {
                ShowError("Invalid pz type given.");
                RequestPz();
                return;
            }

            Console.WriteLine($"Pz type: {pzTypeString}");

            if (inputCircularPz)
            {
                Console.WriteLine("Please enter the radius of the pz.");
                Console.Write("> ");
                string? radiusString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(radiusString))
                {
                    ShowError("No radius given.");
                    RequestPz();
                    return;
                }

                int radius;
                try
                {
                    radius = Convert.ToInt32(radiusString);
                }
                catch (Exception e)
                {
                    ShowError($"Invalid radius given: {e.Message}");
                    RequestPz();
                    return;
                }


                Console.WriteLine("Please enter the center of pz: zone.");
                Console.Write("> ");
                string? zoneString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(zoneString))
                {
                    ShowError("No zone given.");
                    RequestPz();
                    return;
                }

                Console.WriteLine("Please enter the center of pz: easting.");
                Console.Write("> ");
                string? eastingString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(eastingString))
                {
                    ShowError("No easting given.");
                    RequestPz();
                    return;
                }

                int easting;
                try
                {
                    easting = Convert.ToInt32(eastingString);
                }
                catch (Exception e)
                {
                    ShowError($"Invalid easting given: {e.Message}");
                    RequestPz();
                    return;
                }

                Console.WriteLine("Please enter the center of pz: northing.");
                Console.Write("> ");
                string? nothingString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(nothingString))
                {
                    ShowError("No northing given.");
                    RequestPz();
                    return;
                }

                int northing;
                try
                {
                    northing = Convert.ToInt32(nothingString);
                }
                catch (Exception e)
                {
                    ShowError($"Invalid northing given: {e.Message}");
                    RequestPz();
                    return;
                }

                Console.WriteLine($"Center coordinate: {zoneString} {easting} {northing}");
                var centerCoordinate =
                    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate(zoneString, easting, northing);

                Console.WriteLine("Please enter the height of the pz in m.");
                Console.Write("> ");
                string? heightString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(heightString))
                {
                    ShowError("No height given.");
                    RequestPz();
                    return;
                }

                int height;
                try
                {
                    height = Convert.ToInt32(heightString);
                }
                catch (Exception e)
                {
                    ShowError($"Invalid height given: {e.Message}");
                    RequestPz();
                    return;
                }

                Console.WriteLine($"PZ: {centerCoordinate} with height {height}m and radius {radius}m");
                Pz = new CirclePZ(centerCoordinate, height, radius);
            }
            else if (inputPolygonalPz)
            {
                Console.WriteLine("Please enter the path to the polygonal plt file.");
                Console.Write("> ");
                string? polygonalPzPath = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(polygonalPzPath))
                {
                    ShowError("No path given.");
                    RequestPz();
                    return;
                }

                bool isFile = File.Exists(polygonalPzPath);
                if (!isFile)
                {
                    ShowError("File does not exits.");
                    RequestPz();
                    return;
                }

                if (!polygonalPzPath.ToLower().EndsWith(".plt"))
                {
                    ShowError("File is not a plt file.");
                    RequestPz();
                    return;
                }

                Console.WriteLine("Please enter the min height of the pz in m.");
                Console.Write("> ");
                string? minHeightString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(minHeightString))
                {
                    ShowError("No min height given.");
                    RequestPz();
                    return;
                }

                int minHeight;
                try
                {
                    minHeight = Convert.ToInt32(minHeightString);
                }
                catch (Exception e)
                {
                    ShowError($"Invalid min height given: {e.Message}");
                    RequestPz();
                    return;
                }

                Console.WriteLine("Please enter the max height of the pz in m.");
                Console.Write("> ");
                string? maxHeightString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(maxHeightString))
                {
                    ShowError("No height given.");
                    RequestPz();
                    return;
                }

                int maxHeight;
                try
                {
                    maxHeight = Convert.ToInt32(maxHeightString);
                }
                catch (Exception e)
                {
                    ShowError($"Invalid max height given: {e.Message}");
                    RequestPz();
                    return;
                }

                Console.WriteLine("Please enter the height of the virtual pz center in m.");
                Console.Write("> ");
                string? virtualCenterHeightString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(virtualCenterHeightString))
                {
                    ShowError("No height given.");
                    RequestPz();
                    return;
                }

                int virtualCenterHeight;
                try
                {
                    virtualCenterHeight = Convert.ToInt32(virtualCenterHeightString);
                }
                catch (Exception e)
                {
                    ShowError($"Invalid max height given: {e.Message}");
                    RequestPz();
                    return;
                }

                Pz = new PolygonPz(polygonalPzPath, minHeight, maxHeight, virtualCenterHeight);
            }
            else
            {
                ShowError("Invalid pz type given.");
                RequestPz();
                return;
            }


            RequestFileOrFolder();
        }

        private static IPz? Pz = null;

        private static void RequestFileOrFolder()
        {
            Console.WriteLine("Please enter a path to a folder with the igc files in or the path to the files.");
            Console.Write("> ");
            string? path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path))
            {
                ShowError("No path given.");
                RequestFileOrFolder();
                return;
            }

            Console.WriteLine($"Path: '{path}'");
            bool isFile = File.Exists(path);
            bool isDirectory = Directory.Exists(path);
            if (!isFile && !isDirectory)
            {
                ShowError("Directory or file does not exits.");
                RequestFileOrFolder();
                return;
            }

            if (isFile)
            {
                Console.WriteLine("File detected. Parsing...");
                //ParseFile(path);
                ParseFolder(Directory.GetFiles(Path.GetDirectoryName(path)).ToList().FindAll(s => s.ToLower().EndsWith(".igc")));
            }
            else if (isDirectory)
            {
                Console.WriteLine("Folder detected. Parsing...");
                ParseFolder(Directory.GetFiles(path).ToList().FindAll(s => s.ToLower().EndsWith(".igc")));
            }
        }


        private static void ParseFolder(List<string> files)
        {
            if (files.Count == 0)
            {
                ShowError("No igc-files in folder found.");
                RequestFileOrFolder();
                return;
            }

            if (GpsMode == null)
            {
                ShowError("Invalid state. No height mode given.");
                return;
            }

            if (!GpsMode.Value && Qnh == null)
            {
                ShowError("Invalid state. No QNH given.");
                return;
            }

            if (BackupCoordinate == null)
            {
                ShowError("Invalid state. No backup coordinate given.");
                return;
            }

            if (Pz == null)
            {
                ShowError("Invalid state: No pz given.");
                return;
            }

            List<Track> tracks = new();

            foreach (string path in files)
            {
                Console.WriteLine($"Parsing '{path}'...");
                if (!BalloonLiveParser.ParseFile(path, out Track track, BackupCoordinate))
                {
                    Console.WriteLine($"Failed to parse track '{path}'");
                    continue;
                }

                track.trackPath = new FileInfo(path);


                if (!GpsMode.Value)
                {
                    foreach (Coordinate trackPoint in track.TrackPoints)
                    {
                        trackPoint.CorrectBarometricHeightSwitch(Qnh.Value);
                    }

                    foreach (MarkerDrop markerDrop in track.MarkerDrops)
                    {
                        markerDrop.MarkerLocation.CorrectBarometricHeightSwitch(Qnh.Value);
                    }

                    foreach (Declaration decleration in track.Declarations)
                    {
                        decleration.PositionAtDeclaration.CorrectBarometricHeightSwitch(Qnh.Value);
                    }
                }

                tracks.Add(track);
            }

            if (tracks.Count == 0)
            {
                ShowError("No valid igc-files in folder found.");
                RequestFileOrFolder();
                return;
            }

            tracks = tracks.OrderBy(x => x.Pilot.PilotNumber).ToList();
            Console.WriteLine($"Loaded {tracks.Count} from {files.Count} IGC-Files");

            //SUMMARY
            string genericCsvExport =
                "track;entry zone;entry easting;entry northing;entry time;exit zone;exit easting;exit northing;exit time;affected points;" +
                "Opt 1 CPs;hor inf 1; ver inf 1;" +
                "Opt 2A CPs;hor inf 2A; ver inf 2A;" +
                "Opt 2B CPs;hor inf 2B; ver inf 2B;" +
                "Opt 2C CPs;hor inf 2C; ver inf 2C;" +
                "Opt 2D CPs;hor inf 2D; ver inf 2D;" +
                "Opt 3A CPs;hor inf 3A; ver inf 3A;" +
                "Opt 3B CPs;hor inf 3B; ver inf 3B;" +
                "Opt 4A CPs;hor inf 4A; ver inf 4A;" +
                "Opt 4B CPs;hor inf 4B; ver inf 4B;";
            //"pilot;entry position zone;entry position easting;entry position northing;entry time;exit position zone;exit position easting;exit position northing;exit time;affected points;penalty by penalty calculation 1;penalty by penalty calculation 2;penalty by penalty calculation 3a;penalty by penalty calculation 3b;penalty by penalty calculation 3c;penalty by penalty calculation 4a;penalty by penalty calculation 4b;penalty by penalty calculation 5";
            
            //PILOT SUMMARY
            string pilotCsvExportTitle =
                "UTM zone;UTM easting;UTM northing;alt m;alt ft;time;is in;" +
                "Opt 1 CPs;hor inf 1; ver inf 1;" +
                "Opt 2A CPs;hor inf 2A; ver inf 2A;" +
                "Opt 2B CPs;hor inf 2B; ver inf 2B;" +
                "Opt 2C CPs;hor inf 2C; ver inf 2C;" +
                "Opt 2D CPs;hor inf 2D; ver inf 2D;" +
                "Opt 3A CPs;hor inf 3A; ver inf 3A;" +
                "Opt 3B CPs;hor inf 3B; ver inf 3B;" +
                "Opt 4A CPs;hor inf 4A; ver inf 4A;" +
                "Opt 4B CPs;hor inf 4B; ver inf 4B;";

            Dictionary<int, string> pilotExports = new();
            foreach (Track track in tracks)
            {
                string pilotExport = pilotCsvExportTitle;
                bool isInsidePz = false;
                List<Coordinate> pointsInPz = new();

                bool wasInPz = false;
                foreach (Coordinate trackPoint in track.TrackPoints)
                {
                    if (Pz.IsInsidePz(GpsMode.Value, trackPoint, out double horizontalInfringement, out double verticalInfringement))
                    {
                        if (!wasInPz)
                        {
                            var index = track.TrackPoints.IndexOf(trackPoint);
                            if (index > 5)
                            {
                                for (int i = 5; i > 0; i--)
                                {
                                    var prevTpCoords = track.TrackPoints[index - i].FillCoordinate();
                                    pilotExport +=
                                        $"\n{prevTpCoords.utmZone};{prevTpCoords.easting};{prevTpCoords.northing};{(GpsMode.Value ? prevTpCoords.AltitudeGPS : prevTpCoords.AltitudeBarometric)};{CoordinateHelpers.ConvertToFeet((GpsMode.Value ? prevTpCoords.AltitudeGPS : prevTpCoords.AltitudeBarometric))};{prevTpCoords.TimeStamp};no";
                                }
                            }

                            wasInPz = true;
                        }

                        var tpCoords = trackPoint.FillCoordinate();
                        Pz.CalculatePenaltyVariant1(GpsMode.Value, new List<Coordinate> { tpCoords }, out double singlePenaltyV01,  out double horizontalInfringementV01, out double verticalInfringementV01);
                        Pz.CalculatePenaltyVariant2A(GpsMode.Value, new List<Coordinate> { tpCoords }, out double singlePenaltyV2A,  out double horizontalInfringementV2A, out double verticalInfringementV2A);
                        Pz.CalculatePenaltyVariant2B(GpsMode.Value, new List<Coordinate> { tpCoords }, out double singlePenaltyV2B, out double horizontalInfringementV2B, out double verticalInfringementV2B);
                        Pz.CalculatePenaltyVariant2C(GpsMode.Value, new List<Coordinate> { tpCoords }, out double singlePenaltyV2C, out double horizontalInfringementV2C, out double verticalInfringementV2C);
                        Pz.CalculatePenaltyVariant2D(GpsMode.Value, new List<Coordinate> { tpCoords }, out double singlePenaltyV2D, out double horizontalInfringementV2D, out double verticalInfringementV2D);
                        Pz.CalculatePenaltyVariant3A(GpsMode.Value, new List<Coordinate> { tpCoords }, out double singlePenaltyV3A, out double horizontalInfringementV3A, out double verticalInfringementV3A);
                        Pz.CalculatePenaltyVariant3B(GpsMode.Value, new List<Coordinate> { tpCoords }, out double singlePenaltyV3B, out double horizontalInfringementV3B, out double verticalInfringementV3B);
                        Pz.CalculatePenaltyVariant4A(GpsMode.Value, new List<Coordinate> { tpCoords }, out double singlePenaltyV4A,  out double horizontalInfringementV4A, out double verticalInfringementV4A);
                        Pz.CalculatePenaltyVariant4B(GpsMode.Value, new List<Coordinate> { tpCoords }, out double singlePenaltyV4B,  out double horizontalInfringementV4B, out double verticalInfringementV4B);
                        //Console.WriteLine("V1 single double penalty: " double + singlePenaltyV1);
                        //Console.WriteLine("V2 single penalty: " + singlePenaltyV2);
                        //Console.WriteLine("V3A single penalty: " + singlePenaltyV3A);
                        //Console.WriteLine("V3B single penalty: " + singlePenaltyV3B);
                        //Console.WriteLine("V3C single penalty: " + singlePenaltyV3C);
                        //Console.WriteLine("V4A single penalty: " + singlePenaltyV4A);
                        //Console.WriteLine("V4B single penalty: " + singlePenaltyV4B);
                        //Console.WriteLine("V5 single penalty: " + singlePenaltyV5);
                        
                        //pilotExport +=
                        //    $"\n{tpCoords.utmZone};{tpCoords.easting};{tpCoords.northing};{(GpsMode.Value ? tpCoords.AltitudeGPS : tpCoords.AltitudeBarometric)};{CoordinateHelpers.ConvertToFeet((GpsMode.Value ? tpCoords.AltitudeGPS : tpCoords.AltitudeBarometric))};{tpCoords.TimeStamp};yes;{horizontalInfringement};{verticalInfringement};{singlePenaltyV1};{singlePenaltyV2};{singlePenaltyV3A};{singlePenaltyV3B};{singlePenaltyV3C};{singlePenaltyV4A};{singlePenaltyV4B};{singlePenaltyV5}";

                        //PILOT SUMMARY
                        pilotExport +=
                            $"\n{tpCoords.utmZone};{tpCoords.easting};{tpCoords.northing};{(GpsMode.Value ? tpCoords.AltitudeGPS : tpCoords.AltitudeBarometric)};{CoordinateHelpers.ConvertToFeet((GpsMode.Value ? tpCoords.AltitudeGPS : tpCoords.AltitudeBarometric))};{tpCoords.TimeStamp};yes;" +
                            $"{singlePenaltyV01};{horizontalInfringementV01};{verticalInfringementV01};" +
                            $"{singlePenaltyV2A};{horizontalInfringementV2A};{verticalInfringementV2A};" +
                            $"{singlePenaltyV2B};{horizontalInfringementV2B};{verticalInfringementV2B};" +
                            $"{singlePenaltyV2C};{horizontalInfringementV2C};{verticalInfringementV2C};" +
                            $"{singlePenaltyV2D};{horizontalInfringementV2D};{verticalInfringementV2D};" +
                            $"{singlePenaltyV3A};{horizontalInfringementV3A};{verticalInfringementV3A};" +
                            $"{singlePenaltyV3B};{horizontalInfringementV3B};{verticalInfringementV3B};" +
                            $"{singlePenaltyV4A};{horizontalInfringementV4A};{verticalInfringementV4A};" +
                            $"{singlePenaltyV4B};{horizontalInfringementV4B};{verticalInfringementV4B};";
                        isInsidePz = true;
                        pointsInPz.Add(trackPoint);
                    }
                    else
                    {
                        if (wasInPz)
                        {
                            var index = track.TrackPoints.IndexOf(trackPoint);
                            if (index + 5 < track.TrackPoints.Count)
                            {
                                for (int i = 1; i <= 5; i++)
                                {
                                    var afterTpCoords = track.TrackPoints[index + i].FillCoordinate();
                                    pilotExport +=
                                        $"\n{afterTpCoords.utmZone};{afterTpCoords.easting};{afterTpCoords.northing};{(GpsMode.Value ? afterTpCoords.AltitudeGPS : afterTpCoords.AltitudeBarometric)};{CoordinateHelpers.ConvertToFeet((GpsMode.Value ? afterTpCoords.AltitudeGPS : afterTpCoords.AltitudeBarometric))};{afterTpCoords.TimeStamp};no"; //;{horizontalInfringement};{verticalInfringement}";
                                }
                            }

                            wasInPz = false;
                        }
                    }
                }

                pilotExports.Add(track.Pilot.PilotNumber, pilotExport);

                if (!isInsidePz)
                {
                    Console.WriteLine($"No points inside pz by track of pilot #{track.Pilot.PilotNumber}.");
                    continue;
                }

                pointsInPz = pointsInPz.OrderBy(x => x.TimeStamp).ToList();
                Console.WriteLine(
                    $"Found {pointsInPz.Count} points inside pz by track of pilot #{track.Pilot.PilotNumber}.");
                Coordinate entryPoint = pointsInPz.First().FillCoordinate();
                Console.WriteLine(
                    $"Entry: {entryPoint.utmZone} {entryPoint.easting} {entryPoint.northing} at {entryPoint.TimeStamp}");
                Coordinate exitPoint = pointsInPz.Last().FillCoordinate();
                Console.WriteLine(
                    $"Exit: {exitPoint.utmZone} {exitPoint.easting} {exitPoint.northing} at {exitPoint.TimeStamp}");

                //horizontalInfringement = 9999;
                
//CALCULATING FOR SUMMARY
                Pz.CalculatePenaltyVariant1(GpsMode.Value, pointsInPz, out double penaltyV1 , out double distanceHorizontalV1, out double verticalHeightDifferenceV1); //, out horizontalInfringement, out verticalInfringement);
                Pz.CalculatePenaltyVariant2A(GpsMode.Value, pointsInPz, out double penaltyV2A, out double distanceHorizontalV2A, out double verticalHeightDifferenceV2A); //, out horizontInfr, out verticInfr);
                Pz.CalculatePenaltyVariant2B(GpsMode.Value, pointsInPz, out double penaltyV2B, out double distanceHorizontalV2B, out double verticalHeightDifferenceV2B); //, out horizontInfr, out verticInfr);
                Pz.CalculatePenaltyVariant2C(GpsMode.Value, pointsInPz, out double penaltyV2C, out double distanceHorizontalV2C, out double verticalHeightDifferenceV2C); //, out horizontInfr, out verticInfr);
                Pz.CalculatePenaltyVariant2D(GpsMode.Value, pointsInPz, out double penaltyV2D, out double distanceHorizontalV2D, out double verticalHeightDifferenceV2D); //, out horizontInfr, out verticInfr);
                Pz.CalculatePenaltyVariant3A(GpsMode.Value, pointsInPz, out double penaltyV3A, out double distanceHorizontalV3A, out double verticalHeightDifferenceV3A); //, out horizontInfr, out verticInfr);
                Pz.CalculatePenaltyVariant3B(GpsMode.Value, pointsInPz, out double penaltyV3B, out double distanceHorizontalV3B, out double verticalHeightDifferenceV3B); //, out horizontInfr, out verticInfr);
                Pz.CalculatePenaltyVariant4A(GpsMode.Value, pointsInPz, out double penaltyV4A, out double distanceHorizontalV4A, out double verticalHeightDifferenceV4A); //, out horizontInfr, out verticInfr);
                Pz.CalculatePenaltyVariant4B(GpsMode.Value, pointsInPz, out double penaltyV4B, out double distanceHorizontalV4B, out double verticalHeightDifferenceV4B); //, out horizontInfr, out verticInfr);
               
                //Console.WriteLine("V1 penalty: " + penaltyV1);
                //Console.WriteLine("V2 penalty: " + penaltyV2);
                //le.WriteLine("V3C penalty: " + penaltyV3C);
                //Console.WriteLine("V4A penalty: " + penaltyV4A);
                //Console.WriteLine("V4B penalty: " + penaltyV4B);
                //Console.WriteLine("V5 penalty: " + penaltyV5);


                genericCsvExport +=
                    $"\n{track.Pilot.PilotNumber};{entryPoint.utmZone};{entryPoint.easting};{entryPoint.northing};{entryPoint.TimeStamp};{exitPoint.utmZone};{exitPoint.easting};{exitPoint.northing};{exitPoint.TimeStamp};{pointsInPz.Count};" +
                    $"{penaltyV1};{distanceHorizontalV1};{verticalHeightDifferenceV1};" +
                    $"{penaltyV2A};{distanceHorizontalV2A};{verticalHeightDifferenceV2A};" +
                    $"{penaltyV2B};{distanceHorizontalV2B};{verticalHeightDifferenceV2B};" +
                    $"{penaltyV2C};{distanceHorizontalV2C};{verticalHeightDifferenceV2C};" +
                    $"{penaltyV2D};{distanceHorizontalV2D};{verticalHeightDifferenceV2D};" +
                    $"{penaltyV3A};{distanceHorizontalV3A};{verticalHeightDifferenceV3A};" +
                    $"{penaltyV3B};{distanceHorizontalV3B};{verticalHeightDifferenceV3B};" +
                    $"{penaltyV4A};{distanceHorizontalV4A};{verticalHeightDifferenceV4A};" +
                    $"{penaltyV4B};{distanceHorizontalV4B};{verticalHeightDifferenceV4B};";
                //    $"\n{track.Pilot.PilotNumber};{entryPoint.utmZone};{entryPoint.easting};{entryPoint.northing};{entryPoint.TimeStamp};{exitPoint.utmZone};{exitPoint.easting};{exitPoint.northing};{exitPoint.TimeStamp};{pointsInPz.Count};{horizontalInfringement};{verticalInfringement};{penaltyV1};{penaltyV2};{penaltyV3A};{penaltyV3B};{penaltyV3C};{penaltyV4A};{penaltyV4B};{penaltyV5}";

                Console.WriteLine($"Finished pilot #{track.Pilot.PilotNumber}.");
            }

            string? envTemp =
                Environment.GetEnvironmentVariable(
                    OperatingSystem.IsWindows() ? "TEMP" : "TMPDIR"
                )
                ?? "/tmp";
            var folder = Path.Combine(envTemp, "PZTesting_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            Directory.CreateDirectory(folder);
            string sumFilePath = Path.Combine(folder, "sum.csv");
            using (StreamWriter writer1 = new(sumFilePath, false))
            {
                writer1.Write(genericCsvExport);
                writer1.Close();
                Console.WriteLine($"Successful written sumfile to penalty calculation: {sumFilePath}");
            }

            foreach (var keyValuePair in pilotExports)
            {
                string pilotPath = Path.Combine(folder, $"p_{keyValuePair.Key}.csv");
                using (StreamWriter writer1 = new(pilotPath, false))
                {
                    writer1.Write(keyValuePair.Value);
                    writer1.Close();
                    Console.WriteLine(
                        $"Successful created penalty calculation for pilot #{keyValuePair.Key}: {pilotPath}.");
                }
            }
        }

        /*
         private static void ParseFile(string path)
        {
            if (GpsMode == null)
            {
                ShowError("Invalid state. No height mode given.");
                return;
            }

            if (!GpsMode.Value && Qnh == null)
            {
                ShowError("Invalid state. No QNH given.");
                return;
            }

            if (BackupCoordinate == null)
            {
                ShowError("Invalid state. No backup coordinate given.");
                return;
            }

            if (Pz == null)
            {
                ShowError("Invalid state: No pz given.");
                return;
            }

            Track track;

            Console.WriteLine($"Parsing '{path}'...");
            if (!BalloonLiveParser.ParseFile(path, out track, BackupCoordinate))
            {
                ShowError($"Failed to parse track '{path}'");
                RequestFileOrFolder();
                return;
            }

            track.trackPath = new FileInfo(path);


            if (!GpsMode.Value)
            {
                foreach (Coordinate trackPoint in track.TrackPoints)
                {
                    trackPoint.CorrectBarometricHeightSwitch(Qnh.Value);
                }

                foreach (MarkerDrop markerDrop in track.MarkerDrops)
                {
                    markerDrop.MarkerLocation.CorrectBarometricHeightSwitch(Qnh.Value);
                }

                foreach (Declaration decleration in track.Declarations)
                {
                    decleration.PositionAtDeclaration.CorrectBarometricHeightSwitch(Qnh.Value);
                }
            }

            Console.WriteLine($"Loaded IGC-File");


            bool isInsidePz = false;
            List<Coordinate> pointsInPz = new();
            foreach (Coordinate trackPoint in track.TrackPoints)
            {
                //if (Pz.IsInsidePz(GpsMode.Value, trackPoint, out double horizontalInfringement, out double verticalInfringement))
                if (Pz.IsInsidePz(GpsMode.Value, trackPoint, out double horizontalInfringement))
                {
                    isInsidePz = true;
                    pointsInPz.Add(trackPoint);
                }
            }

            if (!isInsidePz)
            {
                Console.WriteLine("No points inside pz.");
                return;
            }

            pointsInPz = pointsInPz.OrderBy(x => x.TimeStamp).ToList();
            Console.WriteLine(
                $"Found {pointsInPz.Count} points inside pz by track of pilot #{track.Pilot.PilotNumber}.");
            Coordinate entryPoint = pointsInPz.First();
            Console.WriteLine(
                $"Entry: {entryPoint.utmZone} {entryPoint.easting} {entryPoint.northing} at {entryPoint.TimeStamp}");
            Coordinate exitPoint = pointsInPz.Last();
            Console.WriteLine(
                $"Exit: {exitPoint.utmZone} {exitPoint.easting} {exitPoint.northing} at {exitPoint.TimeStamp}");
        }
         */


        private static void ShowTitle(string title)
        {
            Console.OutputEncoding = Encoding.UTF8;

            int width = Math.Max(60, Console.WindowWidth);
            string Pad(int n) => new(' ', n);

            string left = "┏";
            string right = "┓";
            string leftMid = "┗";
            string rightMid = "┛";
            string horiz = "━";
            string vert = "┃";

            string centeredTitle = $" {title} ";
            int inner = width - 2;
            int titlePad = Math.Max(0, inner - centeredTitle.Length);
            int leftPad = titlePad / 2;
            int rightPad = titlePad - leftPad;

            Console.WriteLine(left + new string(horiz[0], inner) + right);
            Console.WriteLine(vert + Pad(leftPad) + centeredTitle + Pad(rightPad) + vert);

            string info = $" {DateTime.Now:yyyy-MM-dd HH:mm} • Jan Meinl ";
            int infoPad = Math.Max(0, inner - info.Length);
            int infoLeft = infoPad / 2;
            int infoRight = infoPad - infoLeft;
            Console.WriteLine(vert + Pad(infoLeft) + info + Pad(infoRight) + vert);

            Console.WriteLine(leftMid + new string(horiz[0], inner) + rightMid);
        }

        private static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
            Thread.Sleep(2000);
            ClearConsole();
        }

        private static void ClearConsole()
        {
            Console.Clear();
            Console.WriteLine();
            ShowTitle("PZ-Testing");
            Console.WriteLine();
        }
    }
}