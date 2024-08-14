using LoggingConnector;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BLC2021
{
    public sealed class PilotMapping
    {
        private readonly ILogger<PilotMapping> Logger = LogConnector.LoggerFactory.CreateLogger<PilotMapping>();

        private List<(int pilotNumber, string lastName, string firstName)> PilotMappings
        {
            get; set;
        }


        public bool GetPilotName(int pilotNumber, out string lastName, out string firstName)
        {
            lastName = string.Empty;
            firstName = string.Empty;
            if (PilotMappings is null || PilotMappings.Count == 0)
            {
                if (!LoadPilotMapping())
                {
                    Logger?.LogError("Failed to get pilot name for pilot number '{pilotNumber}'", pilotNumber);
                    return false;
                }
            }
            (int pilotNumber, string lastName, string firstName) pilotMapping = PilotMappings.Find(x => x.pilotNumber == pilotNumber);

            if (pilotMapping == default)
            {
                Logger?.LogError("Failed to get pilot name for pilot number '{pilotNumber}' : Pilot number not found within mappings", pilotNumber);
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

                OpenFileDialog openFileDialog = new()
                {
                    AddExtension = true,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Multiselect = false,
                    Title = "Select pilot mapping",
                    Filter = "xlsx files (*.xlsx)|*.xlsx"
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pilotMappingFile = new FileInfo(openFileDialog.FileName);
                    Properties.Settings.Default.PathToPilotMapping = pilotMappingFile.FullName;
                    Properties.Settings.Default.Save();
                }

            }
            try
            {
                List<(int pilotNumber, string lastName, string firstName)> pilotMappings = [];
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage package = new(pilotMappingFile))
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
                Logger?.LogInformation("Pilot mappings successfully loaded");
            }
            catch (Exception)
            {
                Logger?.LogError("Failed to load pilot mapping");
                return false;
            }
            return true;

        }

        public override string ToString()
        {
            return "PilotMapping";
        }
    }
}
