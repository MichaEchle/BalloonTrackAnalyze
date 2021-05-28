using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;
using System.Windows.Forms;
using LoggerComponent;
using System.Threading;

namespace BLC2021
{
    public sealed class PilotMapping
    {
        private static readonly Lazy<PilotMapping> lazy =
                new Lazy<PilotMapping>(() => new PilotMapping());

        public static PilotMapping Instance
        {
            get
            {
                return lazy.Value;
            }
        }


        private List<(int pilotNumber, string lastName, string firstName)> PilotMappings
        {
            get; set;
        }

        private PilotMapping()
        {
        }

        public bool GetPilotName(int pilotNumber, out string lastName, out string firstName)
        {
            lastName = string.Empty;
            firstName = string.Empty;
            if (PilotMappings is null || PilotMappings.Count == 0)
            {
                if (!LoadPilotMapping())
                {
                    Log(LogSeverityType.Error, $"Failed to get pilot name for pilot number '{pilotNumber}'");
                    return false;
                }
            }
            (int pilotNumber, string lastName, string firstName) pilotMapping = PilotMappings.Find(x => x.pilotNumber == pilotNumber);

            if (pilotMapping == default)
            {
                Log(LogSeverityType.Warning, $"Failed to get pilot name for pilot number '{pilotNumber}' : Pilot number not found within mappings");
                return false;

            }
            lastName = pilotMapping.lastName;
            firstName = pilotMapping.firstName;
            return true;

        }

        private bool LoadPilotMapping()
        {
            FileInfo pilotMappingFile = null;
            bool showDialog = false;
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.PathToPilotMapping))
            {
                pilotMappingFile = new FileInfo(Properties.Settings.Default.PathToPilotMapping);
                if (!pilotMappingFile.Exists)
                    showDialog = true;
            }
            else
                showDialog = true;
            if (showDialog)
            {

                Thread t = new Thread((ThreadStart)(() => {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.AddExtension = true;
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.Multiselect = false;
                openFileDialog.Title = "Select pilot mapping";
                openFileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pilotMappingFile = new FileInfo(openFileDialog.FileName);
                    Properties.Settings.Default.PathToPilotMapping = pilotMappingFile.FullName;
                    Properties.Settings.Default.Save();
                }
                }));

                
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();
            }
            try
            {
                List<(int pilotNumber, string lastName, string firstName)> pilotMappings = new List<(int pilotNumber, string lastName, string firstName)>();
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(pilotMappingFile))
                {
                    ExcelWorksheet wsPilots = package.Workbook.Worksheets.First();

                    int rowIndex = 2;
                    bool continueWithNextRow = true;
                    while (continueWithNextRow)
                    {
                        int? pilotNumber = wsPilots.Cells[rowIndex, 1].GetValue<int?>();
                        string lastName = wsPilots.Cells[rowIndex, 3].GetValue<string>();
                        string firstName = wsPilots.Cells[rowIndex, 4].GetValue<string>();

                        if (pilotNumber == null || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName))
                        {
                            continueWithNextRow = false;
                        }
                        else
                        {
                            rowIndex++;
                            pilotMappings.Add(((int)pilotNumber, lastName, firstName));
                        }
                    }
                }
                PilotMappings = pilotMappings;
                Log(LogSeverityType.Info, "Pilot mappings successfully loaded");

            }
            catch (Exception)
            {
                Log(LogSeverityType.Error, "Failed to load pilot mapping");
                return false;
            }
            return true;

        }

        public override string ToString()
        {
            return "PilotMapping";
        }

        private void Log(LogSeverityType logSeverity, string logMessage)
        {
            Logger.Log(this, logSeverity, logMessage);
        }
    }
}
