using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrackReportGenerator;

public partial class TrackReportGeneratorForm : Form
{
    #region Properties
    private readonly ILogger<TrackReportGeneratorForm> Logger=LogConnector.LoggerFactory.CreateLogger<TrackReportGeneratorForm>();
    private bool UseGPSAltitude = true;
    private double MaxAllowedAltitude = CoordinateHelpers.ConvertToMeter(10000);
    private bool SkipCoordinatesWithoutLocation = true;
    #endregion Properties

    #region Constructor
    public TrackReportGeneratorForm()
    {
        InitializeComponent();
        Text += typeof(TrackReportGeneratorForm).Assembly.GetName().Version;
        //logListView1.StartLogging(@".\Logfile.txt");
    }

    #endregion Constructor

    #region Methods

    private async void btSelectFiles_Click(object sender, EventArgs e)
    {
        progressBar1.Value = 0;
        OpenFileDialog openFileDialog = new();
        if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.InitalDirectory))
            openFileDialog.InitialDirectory = Properties.Settings.Default.InitalDirectory;
        openFileDialog.Title = "Select .igc files";
        openFileDialog.Filter = "igc files (*.igc)|*.igc";
        openFileDialog.CheckPathExists = true;
        openFileDialog.CheckPathExists = true;
        openFileDialog.Multiselect = true;
        UseWaitCursor = true;
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            string[] igcFiles = openFileDialog.FileNames;
            int successful = 0;
            int erroneous = 0;
            if (igcFiles.Length > 0)
            {
                Properties.Settings.Default.InitalDirectory = new FileInfo(igcFiles[0]).DirectoryName;
                Properties.Settings.Default.Save();
            }
            for (int index = 0; index < igcFiles.Length; index++)
            {
                lbStatus.Text = $"{index} of {igcFiles.Length} processed ({successful} successful / {erroneous} erroneous)";
                bool success = await ProcessFileAsync(igcFiles[index]);
                if (success)
                    successful++;
                else
                    erroneous++;
                progressBar1.Value = (int)Math.Round((double)(((double)(index + 1) / igcFiles.Length) * 100), 0, MidpointRounding.AwayFromZero);
            }
            UseWaitCursor = false;
            progressBar1.Value = 100;
            lbStatus.Text = $"{igcFiles.Length} of {igcFiles.Length} processed ({successful} successful / {erroneous} erroneous)";
        }
    }

    private async Task<bool> ProcessFileAsync(string igcFile)
    {
        return await Task<bool>.Run(() =>
        {
            Track track;
            if (rbBallonLiveParser.Checked)
            {
                if (!Coordinates.Parsers.BalloonLiveParser.ParseFile(igcFile, out track))
                    return false;
            }
            else
            {
                if (!Coordinates.Parsers.FAILoggerParser.ParseFile(igcFile, out track))
                    return false;
            }
            if (rbBallonLiveParser.Checked)
            {
                if (track.AdditionalPropertiesFromIGCFile.TryGetValue("Change of position source", out string changeOfPositionSource))
                {
                    if (changeOfPositionSource.Equals("yes", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Logger?.LogWarning("Caution: change of position source has been detected, refer the log for more details");
                        //   MessageBox.Show("Caution: change of position source has been detected, refer the log for more details");
                    }
                }
            }
            string reportFileName = Path.Combine(Path.GetDirectoryName(igcFile), Path.GetFileNameWithoutExtension(igcFile) + ".xlsx");
            if (!File.Exists(reportFileName) || !cbSkipExistingReports.Checked)
            {
                if (!ExcelTrackReportGenerator.GenerateTrackReport(reportFileName, track, SkipCoordinatesWithoutLocation, UseGPSAltitude, MaxAllowedAltitude))
                    return false;
            }
            else
            {
                Logger?.LogInformation("File '{fileName}' skipped", Path.GetFileName(igcFile));
            }

            return true;
        });
    }

    private void rbGPSAltitude_CheckedChanged(object sender, EventArgs e)
    {
        UseGPSAltitude = rbGPSAltitude.Checked;
    }



    private void rbMeter_CheckedChanged(object sender, EventArgs e)
    {
        if (rbMeter.Checked)
        {
            tbMaxAltitude.Text = $"{MaxAllowedAltitude:0.#}";
        }
        else
        {
            tbMaxAltitude.Text = $"{Math.Round(CoordinateHelpers.ConvertToFeet(MaxAllowedAltitude), 0, MidpointRounding.AwayFromZero)}";
        }
    }

    private void cbSkipCoordinates_CheckedChanged(object sender, EventArgs e)
    {
        SkipCoordinatesWithoutLocation = cbSkipCoordinates.Checked;
    }

    private void cbCheckMaxAltitude_CheckedChanged(object sender, EventArgs e)
    {
        if (cbCheckMaxAltitude.Checked)
        {
            tbMaxAltitude.Text = "10000";
            MaxAllowedAltitude = CoordinateHelpers.ConvertToMeter(10000);
            tbMaxAltitude.Enabled = true;
            rbFeet.Enabled = true;
            rbFeet.Checked = true;
            rbMeter.Enabled = true;
        }
        else
        {
            MaxAllowedAltitude = double.NaN;
            tbMaxAltitude.Text = "";
            tbMaxAltitude.Enabled = false;
            rbFeet.Enabled = false;
            rbMeter.Enabled = false;
        }
    }

    private void tbMaxAltitude_Leave(object sender, EventArgs e)
    {
        if (!double.TryParse(tbMaxAltitude.Text, out double tempMaxAltitude))
        {
            Logger?.LogError("Failed to parse '{maxAltitude}' as double. Please enter a number", tbMaxAltitude.Text);
            return;
        }
        if (rbMeter.Checked)
            MaxAllowedAltitude = tempMaxAltitude;
        else
            MaxAllowedAltitude = CoordinateHelpers.ConvertToMeter(tempMaxAltitude);
    }
    #endregion Methods
}
