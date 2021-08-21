using Competition;
using Coordinates;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLC2021
{
    public partial class BLC2021TaskSheet2 : Form
    {

        #region Properties
        private bool BatchMode
        {
            get; set;
        }

        private DirectoryInfo OutputDirectory
        {
            get; set;
        }

        private Track FiddleTrack
        {
            get; set;
        }

        private int Task4_GoalNumber
        {
            get; set;
        } = 1;

        private int Task4_FirstMarkerNumber
        {
            get; set;
        } = 1;

        private int Task4_SecondMarkerNumber
        {
            get; set;
        } = 2;

        private int Task4_ThridMarkerNumber
        {
            get; set;
        } = 3;


        private readonly string ResultsFileNameInternal = "BLC2021TaskSheet2_Results_Internal.xlsx";
        private readonly string ResultFileNameProvisional = "BLC2021TaskSheet2_Results_Provisional.xslx";
        private double additionalAltitude;
        #endregion

        #region Constructor
        public BLC2021TaskSheet2(bool batchMode)
        {
            BatchMode = batchMode;
            InitializeComponent();

            Text = ToString();

        }
        #endregion

        #region Methods
        #region General
        public override string ToString()
        {
            if (BatchMode)
                return $"BLC 2021 Task Sheet 2 (Batch Mode) v{typeof(BLC2021TaskSheet1).Assembly.GetName().Version}";
            else
                return $"BLC 2021 Task Sheet 2 (Fiddle Mode) v{typeof(BLC2021TaskSheet1).Assembly.GetName().Version}";
        }

        private void BLC2021TaskSheet2_Load(object sender, EventArgs e)
        {
            logListView1.StartLogging();
            if (BatchMode)
                ConfigureForBatchMode();
            else
                ConfigureForFiddleMode();

            bool showDialog = false;
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.DefaultOutputDirectory))
            {
                OutputDirectory = new DirectoryInfo(Properties.Settings.Default.DefaultOutputDirectory);
                if (!OutputDirectory.Exists)
                    showDialog = true;
            }
            else
            {
                showDialog = true;
            }
            if (showDialog)
            {
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                {
                    folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                    folderBrowserDialog.UseDescriptionForTitle = true;
                    folderBrowserDialog.Description = "Select an output directory";
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        OutputDirectory = new DirectoryInfo(folderBrowserDialog.SelectedPath);
                        Properties.Settings.Default.DefaultOutputDirectory = folderBrowserDialog.SelectedPath;
                        Properties.Settings.Default.Save();
                        if (!btSelectIGCFiles.Enabled)
                            btSelectIGCFiles.Enabled = true;
                    }
                    else
                    {
                        Log(LogSeverityType.Error, "No output directory has been defined, an output directory must be configured to continue");
                        btSelectIGCFiles.Enabled = false;
                    }
                }
            }
            else
            {
                if (!btSelectIGCFiles.Enabled)
                    btSelectIGCFiles.Enabled = true;
            }
        }
        private void ConfigureForBatchMode()
        {
            btSelectIGCFiles.Text = "Select IGC File";

            //////////////////
            //Task 4
            //////////////////
            Task4_GoalNumber = 99;
            Task4_FirstMarkerNumber = -1;
            Task4_SecondMarkerNumber = -1;
            Task4_ThridMarkerNumber = -1;
            cbGoalTask4.Visible = false;
            lbZoneTask4.Visible = false;
            tbZoneTask4.Visible = false;
            lbEastingTask4.Visible = false;
            tbEastingTask4.Visible = false;
            lbNorthingTask4.Visible = false;
            tbNorthingTask4.Visible = false;
            lbAltitudeTask4.Visible = false;
            tbAltitudeTask4.Visible = false;
            btCalculateTask4.Visible = false;
            cbFirstMarkerTask4.Visible = false;
            cbSecondMarkerTask4.Visible = false;
            cbThirdMarkerTask4.Visible = false;
            lbGoalNumberTask4.Visible = true;
            lbFirstMarkerNumberTask4.Visible = true;
            lbSecondMarkerNumberTask4.Visible = true;
            lbThirdMarkerNumberTask4.Visible = true;
        }

        private void ConfigureForFiddleMode()
        {

            //////////////////
            //Task 4
            //////////////////
            Task4_GoalNumber = 1;
            Task4_FirstMarkerNumber = 1;
            Task4_SecondMarkerNumber = 2;
            Task4_ThridMarkerNumber = 3;
            cbGoalTask4.Visible = true;
            lbZoneTask4.Visible = true;
            tbZoneTask4.Visible = true;
            lbEastingTask4.Visible = true;
            tbEastingTask4.Visible = true;
            lbNorthingTask4.Visible = true;
            tbNorthingTask4.Visible = true;
            lbAltitudeTask4.Visible = true;
            tbAltitudeTask4.Visible = true;
            btCalculateTask4.Visible = true;
            cbFirstMarkerTask4.Visible = true;
            cbSecondMarkerTask4.Visible = true;
            cbThirdMarkerTask4.Visible = true;
            lbGoalNumberTask4.Visible = false;
            lbFirstMarkerNumberTask4.Visible = false;
            lbSecondMarkerNumberTask4.Visible = false;
            lbThirdMarkerNumberTask4.Visible = false;
        }


        private void Log(LogSeverityType logSeverity, string logMessage)
        {
            Logger.Log(this, logSeverity, logMessage);
        }
        private async void btSelectIGCFiles_Click(object sender, EventArgs e)
        {
            if (BatchMode)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Select .igc files";
                openFileDialog.Filter = "igc files (*.igc)|*.igc";
                openFileDialog.CheckPathExists = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.Multiselect = true;
                UseWaitCursor = true;

                HesitationWaltzTask task4_1 = new HesitationWaltzTask();
                DeclarationToGoalDistanceRule declarationToGoalDistanceRule = new DeclarationToGoalDistanceRule();
                declarationToGoalDistanceRule.SetupRule(5000.0, double.NaN);
                List<IDeclarationValidationRules> declarationValidationRules = new List<IDeclarationValidationRules>();

                task4_1.SetupHWZ(4,CalculateGoalsTask4_1, 1, true, null, declarationValidationRules);

            }
            else//fiddle mode
            {
            }
        }
        #endregion

        #region BatchMode

        private List<Coordinate> CalculateGoalsTask4_1(Track track)
        {
            List<Coordinate> goals = new List<Coordinate>();
            Declaration declaration = track.GetLatestDeclaration(Task4_GoalNumber);
            if (declaration is null || declaration == default)
                throw new Exception($"No goal with goal number '{Task4_GoalNumber}' has been found");
            goals.Add(declaration.DeclaredGoal);
            return goals;
        }
        private List<Coordinate> CalculateGoalsTask4_2(Track track)
        {
            List<Coordinate> goals = new List<Coordinate>();
            double distanceFromDeclaredGoal = 1000.0;
            double additionalAltitude = CoordinateHelpers.ConvertToMeter(300.0);
            Declaration declaration = track.GetLatestDeclaration(Task4_GoalNumber);
            if(declaration is null || declaration == default)
                throw new Exception($"No goal with goal number '{Task4_GoalNumber}' has been found");

            CoordinateSharp.Coordinate tempDeclaredGoal = new CoordinateSharp.Coordinate(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude);

            double easting = Math.Round(tempDeclaredGoal.UTM.Easting, 0, MidpointRounding.AwayFromZero);
            double northing = Math.Round(tempDeclaredGoal.UTM.Northing, 0, MidpointRounding.AwayFromZero);
            string latZone = tempDeclaredGoal.UTM.LatZone;
            int longZone = tempDeclaredGoal.UTM.LongZone;

            for (int northingFactor = -1; northingFactor <= 1; northingFactor++)
            {
                for (int eastingFactor = -1; eastingFactor <= 1; eastingFactor++)
                {
                    if (northingFactor == 0 && eastingFactor == 0)
                        continue;
                    CoordinateSharp.UniversalTransverseMercator utmTempGoal = new CoordinateSharp.UniversalTransverseMercator($"{latZone}{longZone}", easting+(distanceFromDeclaredGoal*eastingFactor), northing+(distanceFromDeclaredGoal*northingFactor));
                    CoordinateSharp.Coordinate tempGoal = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utmTempGoal);
                    goals.Add(new Coordinate(tempGoal.Latitude.DecimalDegree, tempGoal.Longitude.DecimalDegree, declaration.DeclaredGoal.AltitudeGPS+ additionalAltitude, declaration.DeclaredGoal.AltitudeBarometric+ additionalAltitude, DateTime.Now));
                }
            }
            return goals;
        }

        private List<Coordinate> CalculateGoalsTask4_3(Track track)
        {
            List<Coordinate> goals = new List<Coordinate>();
            double distanceFromDeclaredGoal = 2000.0;
            double additionalAltitude = CoordinateHelpers.ConvertToMeter(600.0);
            Declaration declaration = track.GetLatestDeclaration(Task4_GoalNumber);
            if (declaration is null || declaration == default)
                throw new Exception($"No goal with goal number '{Task4_GoalNumber}' has been found");

            CoordinateSharp.Coordinate tempDeclaredGoal = new CoordinateSharp.Coordinate(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude);

            double easting = Math.Round(tempDeclaredGoal.UTM.Easting, 0, MidpointRounding.AwayFromZero);
            double northing = Math.Round(tempDeclaredGoal.UTM.Northing, 0, MidpointRounding.AwayFromZero);
            string latZone = tempDeclaredGoal.UTM.LatZone;
            int longZone = tempDeclaredGoal.UTM.LongZone;

            for (int northingFactor = -1; northingFactor <= 1; northingFactor++)
            {
                for (int eastingFactor = -1; eastingFactor <= 1; eastingFactor++)
                {
                    if (northingFactor == 0 && eastingFactor == 0)
                        continue;
                    CoordinateSharp.UniversalTransverseMercator utmTempGoal = new CoordinateSharp.UniversalTransverseMercator($"{latZone}{longZone}", easting + (distanceFromDeclaredGoal * eastingFactor), northing + (distanceFromDeclaredGoal * northingFactor));
                    CoordinateSharp.Coordinate tempGoal = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utmTempGoal);
                    goals.Add(new Coordinate(tempGoal.Latitude.DecimalDegree, tempGoal.Longitude.DecimalDegree, declaration.DeclaredGoal.AltitudeGPS + additionalAltitude, declaration.DeclaredGoal.AltitudeBarometric + additionalAltitude, DateTime.Now));
                }
            }
            return goals;
        }
        #endregion

        #region Fiddle Task4
        private void btCalculateTask4_Click(object sender, EventArgs e)
        {

        }



        #endregion

        #region Fiddle Task5
        #endregion

        #endregion

    }
}
