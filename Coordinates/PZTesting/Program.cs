using ABI.System.Windows.Input;
using Coordinates;
using Coordinates.Parsers;
using JansScoring.pz_rework.type;
using OfficeOpenXml;
using System.Text;
using WinRT.Interop;

namespace PZTesting
{
    class Program
    {
        public static readonly string Version = "0.0.1";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.Title = "PZ-Testing";
            ClearConsole();

            RequestCompetitionDetails();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
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
                RequestQNH();
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

            Console.WriteLine("Please enter the backup coordinate: nothing.");
            Console.Write("> ");
            string? nothingString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nothingString))
            {
                ShowError("No nothing given.");
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
                ShowError($"Invalid nothing given: {e.Message}");
                RequestBackupCoordinate();
                return;
            }

            Console.WriteLine($"Backup coordinate: {zoneString} {easting} {northing}");
            BackupCoordinate = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate(zoneString, easting, northing);
            RequestPZ();
        }

        private static Coordinate? BackupCoordinate = null;

        private static void RequestQNH()
        {
            Console.WriteLine("Please enter the qnh (in hPa).");
            Console.Write("> ");
            string? qnhString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(qnhString))
            {
                ShowError("No QNH given.");
                RequestQNH();
                return;
            }

            try
            {
                int inputQNH = Convert.ToInt32(qnhString);
                Console.WriteLine($"QNH: {inputQNH}");
                Qnh = inputQNH;
            }
            catch (Exception e)
            {
                ShowError($"Invalid QNH given: {e.Message}");
                RequestQNH();
            }
        }

        private static bool? GpsMode = null;
        private static int? Qnh = null;


        private static void RequestPZ()
        {
            Console.WriteLine("Please enter the type of the pz. (circular/polygonal)");
            Console.Write("> ");
            string? pzTypeString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(pzTypeString))
            {
                ShowError("No pz type given.");
                RequestPZ();
                return;
            }

            bool inputCircularPz = pzTypeString.ToLower() == "circular";
            bool inputPolygonalPz = pzTypeString.ToLower() == "polygonal";
            if (!inputCircularPz && !inputPolygonalPz)
            {
                ShowError("Invalid pz type given.");
                RequestPZ();
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
                    RequestPZ();
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
                    RequestPZ();
                    return;
                }


                Console.WriteLine("Please enter the center of pz: zone.");
                Console.Write("> ");
                string? zoneString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(zoneString))
                {
                    ShowError("No zone given.");
                    RequestPZ();
                    return;
                }

                Console.WriteLine("Please enter the center of pz: easting.");
                Console.Write("> ");
                string? eastingString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(eastingString))
                {
                    ShowError("No easing given.");
                    RequestPZ();
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
                    RequestPZ();
                    return;
                }

                Console.WriteLine("Please enter the center of pz: nothing.");
                Console.Write("> ");
                string? nothingString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(nothingString))
                {
                    ShowError("No nothing given.");
                    RequestPZ();
                    return;
                }

                int northing;
                try
                {
                    northing = Convert.ToInt32(nothingString);
                }
                catch (Exception e)
                {
                    ShowError($"Invalid nothing given: {e.Message}");
                    RequestPZ();
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
                    RequestPZ();
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
                    RequestPZ();
                    return;
                }

                Console.WriteLine($"PZ: {centerCoordinate} with height {height}m and radius {radius}m");
                PZ = new CirclePZ(centerCoordinate, height, radius);
            }
            else if (inputPolygonalPz)
            {
                Console.WriteLine("Please enter the path to the polygonal plt file.");
                Console.Write("> ");
                string? polygonalPzPath = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(polygonalPzPath))
                {
                    ShowError("No path given.");
                    RequestPZ();
                    return;
                }

                bool isFile = File.Exists(polygonalPzPath);
                if (!isFile)
                {
                    ShowError("File does not exits.");
                    RequestPZ();
                    return;
                }

                if (!polygonalPzPath.ToLower().EndsWith(".plt"))
                {
                    ShowError("File is not a plt file.");
                    RequestPZ();
                    return;
                }

                Console.WriteLine("Please enter the min height of the pz in m.");
                Console.Write("> ");
                string? minHeightString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(minHeightString))
                {
                    ShowError("No min height given.");
                    RequestPZ();
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
                    RequestPZ();
                    return;
                }

                Console.WriteLine("Please enter the max height of the pz in m.");
                Console.Write("> ");
                string? maxHeightString = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(maxHeightString))
                {
                    ShowError("No height given.");
                    RequestPZ();
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
                    RequestPZ();
                    return;
                }

                PZ = new PolygonPZ(polygonalPzPath, minHeight, maxHeight);
            }
            else
            {
                ShowError("Invalid pz type given.");
                RequestPZ();
                return;
            }


            RequestFileOrFolder();
        }

        private static PZ? PZ = null;

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
                ParseFile(path);
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

            if (PZ == null)
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
                        trackPoint.CorrectBarometricHeight(Qnh.Value);
                    }

                    foreach (MarkerDrop markerDrop in track.MarkerDrops)
                    {
                        markerDrop.MarkerLocation.CorrectBarometricHeight(Qnh.Value);
                    }

                    foreach (Declaration decleration in track.Declarations)
                    {
                        decleration.PositionAtDeclaration.CorrectBarometricHeight(Qnh.Value);
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



            foreach (Track track in tracks)
            {
                bool isInsidePZ = false;
                List<Coordinate> pointsInPz = new();
                foreach (Coordinate trackPoint in track.TrackPoints)
                {
                    if (PZ.IsInsidePz(GpsMode.Value, trackPoint, out double infringement))
                    {
                        isInsidePZ = true;
                        pointsInPz.Add(trackPoint);
                    }
                }

                if (!isInsidePZ)
                {
                    Console.WriteLine($"No points inside pz by track of pilot #{track.Pilot.PilotNumber}.");
                    continue;
                }

                pointsInPz = pointsInPz.OrderBy(x => x.TimeStamp).ToList();
                Console.WriteLine($"Found {pointsInPz.Count} points inside pz by track of pilot #{track.Pilot.PilotNumber}.");
                Coordinate entryPoint = pointsInPz.First();
                Console.WriteLine($"Entry: {entryPoint.utmZone} {entryPoint.easting} {entryPoint.northing} at {entryPoint.TimeStamp}" );
                Coordinate exitPoint = pointsInPz.Last();
                Console.WriteLine($"Exit: {exitPoint.utmZone} {exitPoint.easting} {exitPoint.northing} at {exitPoint.TimeStamp}");

            }
        }

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

            if (PZ == null)
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
                    trackPoint.CorrectBarometricHeight(Qnh.Value);
                }

                foreach (MarkerDrop markerDrop in track.MarkerDrops)
                {
                    markerDrop.MarkerLocation.CorrectBarometricHeight(Qnh.Value);
                }

                foreach (Declaration decleration in track.Declarations)
                {
                    decleration.PositionAtDeclaration.CorrectBarometricHeight(Qnh.Value);
                }
            }

            Console.WriteLine($"Loaded IGC-File");


            bool isInsidePZ = false;
            List<Coordinate> pointsInPz = new();
            foreach (Coordinate trackPoint in track.TrackPoints)
            {
                if (PZ.IsInsidePz(GpsMode.Value, trackPoint, out double infringement))
                {
                    isInsidePZ = true;
                    pointsInPz.Add(trackPoint);
                }
            }

            if (!isInsidePZ)
            {
                Console.WriteLine("No points inside pz.");
                return;
            }

            pointsInPz = pointsInPz.OrderBy(x => x.TimeStamp).ToList();
            Console.WriteLine($"Found {pointsInPz.Count} points inside pz by track of pilot #{track.Pilot.PilotNumber}.");
            Coordinate entryPoint = pointsInPz.First();
            Console.WriteLine($"Entry: {entryPoint.utmZone} {entryPoint.easting} {entryPoint.northing} at {entryPoint.TimeStamp}" );
            Coordinate exitPoint = pointsInPz.Last();
            Console.WriteLine($"Exit: {exitPoint.utmZone} {exitPoint.easting} {exitPoint.northing} at {exitPoint.TimeStamp}");

        }


        private static void ShowTitle(string title)
        {
            Console.OutputEncoding = Encoding.UTF8;

            int width = Math.Max(60, Console.WindowWidth);
            string pad(int n) => new(' ', n);

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
            Console.WriteLine(vert + pad(leftPad) + centeredTitle + pad(rightPad) + vert);

            string info = $" {DateTime.Now:yyyy-MM-dd HH:mm} • Jan Meinl ";
            int infoPad = Math.Max(0, inner - info.Length);
            int infoLeft = infoPad / 2;
            int infoRight = infoPad - infoLeft;
            Console.WriteLine(vert + pad(infoLeft) + info + pad(infoRight) + vert);

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