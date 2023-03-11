using Coordinates;
using Coordinates.Parsers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoordinateConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<string> UTMZones
        {
            get; set;
        } = new ObservableCollection<string>();

        private double Latitude
        {
            get; set;
        }

        private string LatitudeDegreeMinutes
        {
            get; set;
        }
        private double Longitude
        {
            get; set;
        }

        private string LongitudeDegreeMinutes
        {
            get; set;
        }

        private string UTMZone
        {
            get; set;
        }

        private int Easting
        {
            get; set;
        }

        private int Northing
        {
            get; set;
        }

        private string IGCFilePathAndName
        {
            get; set;
        }

        private List<string> IGCFileLines
        {
            get; set;
        } = new List<string>();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateUTMZoneList();
        }

        private void CreateUTMZoneList()
        {
            char[] zoneCharacters = new char[] { 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X' };
            UTMZones.Clear();
            for (int characterIndex = 0; characterIndex < zoneCharacters.Length; characterIndex++)
            {

                for (int index = 1; index <= 60; index++)
                {
                    UTMZones.Add($"{index}{zoneCharacters[characterIndex]}");
                }
            }
        }

        private void BtConvertToLatLong_Click(object sender, RoutedEventArgs e)
        {
            if (!UTMZones.Contains(CbUTMZone.Text))
            {
                MessageBox.Show("Please select a valid UTM Zone");
                return;
            }
            string utmZone = CbUTMZone.Text;
            int easting;
            if (!int.TryParse(TbEastingInput.Text, out easting))
            {
                MessageBox.Show("Please enter a valid easting value");
                return;
            }
            int northing;
            if (!int.TryParse(TbNorthingInput.Text, out northing))
            {
                MessageBox.Show("Please enter a valid northing value");
                return;
            }
            (double latitude, double longitude) = Coordinates.CoordinateHelpers.ConvertUTMToLatitudeLongitude(utmZone, easting, northing);
            Latitude = latitude;
            Longitude = longitude;

            (int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds) = Coordinates.CoordinateHelpers.ConvertToDegreeMinutes(Latitude);
            LatitudeDegreeMinutes = Coordinates.CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds);
            TbLatitudeOutput.Text = $"{Latitude:0.######} ({LatitudeDegreeMinutes})";
            (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = Coordinates.CoordinateHelpers.ConvertToDegreeMinutes(Longitude);
            LongitudeDegreeMinutes = Coordinates.CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds);
            TbLongitudeOutput.Text = $"{Longitude:0.######} ({LongitudeDegreeMinutes})";
            BtCopyDecimalDegrees.IsEnabled = true;
            BtCopyDgreeeMinutes.IsEnabled = true;
        }

        private void BtCopyDecimalDegrees_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText($"{Latitude}N {Longitude}E");
        }

        private void BtCopyDgreeeMinutes_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText($"{LatitudeDegreeMinutes}N {LongitudeDegreeMinutes}E");
        }

        private void BtConvertToUTM_Click(object sender, RoutedEventArgs e)
        {
            double latitude;
            if (!double.TryParse(TbLatitudeInput.Text, out latitude))
            {
                if (!ParseAndConvertDegreeMinuteInput(TbLatitudeInput.Text, out latitude))
                {
                    MessageBox.Show("Please provide a valid latitude");
                    return;
                }
            }
            double longitude;
            if (!double.TryParse(TbLongitudeInput.Text, out longitude))
            {
                if (!ParseAndConvertDegreeMinuteInput(TbLongitudeInput.Text, out longitude))
                {
                    MessageBox.Show("Please provide a valid longitude");
                    return;
                }
            }
            (string utmZone, int easting, int norhting) = Coordinates.CoordinateHelpers.ConvertLatitudeLongitudeToUTM(latitude, longitude);
            UTMZone = utmZone;
            Easting = easting;
            Northing = norhting;
            TbUTMZoneOutput.Text = UTMZone;
            TbEastingOutput.Text = Easting.ToString();
            TbNorthingOutput.Text = Northing.ToString();
            BtCopyUTM.IsEnabled = true;
        }

        private void BtCopyUTM_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText($"{UTMZone} {Easting} {Northing}");
        }

        private bool ParseAndConvertDegreeMinuteInput(string text, out double decimalDegrees)
        {
            decimalDegrees = double.NaN;
            string digits = "";
            int convertedNumber;
            int degrees = 0;
            int degreeMinutes = 0;
            int degreeSeconds = 0;
            int degreeTenthSeconds = 0;
            int partIndex = 0;
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                {
                    digits += c;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(digits))
                    {

                        convertedNumber = Convert.ToInt32(digits);
                        if (partIndex == 0)
                        {
                            degrees = convertedNumber;
                            partIndex++;
                        }
                        else if (partIndex == 1)
                        {
                            degreeMinutes = convertedNumber;
                            partIndex++;
                        }
                        else if (partIndex == 2)
                        {
                            degreeSeconds = convertedNumber;
                            partIndex++;
                        }
                        else if (partIndex == 3)
                        {
                            degreeTenthSeconds = convertedNumber;
                            partIndex++;
                        }
                        else
                        {
                            return false;
                        }
                        digits = "";
                    }
                }
            }
            decimalDegrees = Coordinates.CoordinateHelpers.ConvertToDecimalDegree(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds, true);
            return true;
        }

        private void TbPasteCompleteUTM_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TbPasteCompleteUTM.Text))
            {
                string[] parts = TbPasteCompleteUTM.Text.Split(" ", StringSplitOptions.TrimEntries);
                CbUTMZone.Text = parts[0];
                TbEastingInput.Text = parts[1];
                TbNorthingInput.Text = parts[2];
            }
        }

        private void TbPastCompleteLatLong_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TbPastCompleteLatLong.Text))
            {
                string input = TbPastCompleteLatLong.Text;
                input = input.Replace("N", "").Replace("E", "");
                string[] parts = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 2)
                {
                    int half = (parts.Length / 2);
                    TbLatitudeInput.Text = string.Join("", parts[0..half]);
                    TbLongitudeInput.Text = string.Join("", parts[half..^0]);
                }
                else
                {

                    TbLatitudeInput.Text = parts[0];
                    TbLongitudeInput.Text = parts[1];
                }
            }
        }

        private void BtSelectIGCFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "igc File (*.igc)|*.igc";
            if (openFileDialog.ShowDialog() ?? false)
            {
                IGCFileLines.Clear();
                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    while (!reader.EndOfStream)
                    {
                        IGCFileLines.Add(reader.ReadLine());
                    }
                }
            }
            else
                BtParseLine.IsEnabled = false;

        }

        private void BtParseLine_Click(object sender, RoutedEventArgs e)
        {
            if (RbBallonLiveParser.IsChecked ?? false)
            {

            }
            else//FAI parser
            {

            }
        }

        private void TbLineNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IGCFileLines.Count > 0)
            {
                int lineNumber;
                if (!string.IsNullOrEmpty(TbLineNumber.Text))
                {
                    if (!int.TryParse(TbLineNumber.Text, out lineNumber))
                    {
                        TbLineContent.Text = string.Empty;
                        MessageBox.Show("Please enter a valid line number");
                        BtParseLine.IsEnabled = false;
                        return;
                    }

                    if (lineNumber >= 0 && lineNumber <= IGCFileLines.Count - 1)
                    {
                        TbLineContent.Text = IGCFileLines[lineNumber];
                        BtParseLine.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show($"IGC file doesn't have a line number {lineNumber}");
                        BtParseLine.IsEnabled = false;
                    }
                }
                else
                {
                    TbLineContent.Text = string.Empty;
                    BtParseLine.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("Please select and .igc file first");
            }
        }
    }
}