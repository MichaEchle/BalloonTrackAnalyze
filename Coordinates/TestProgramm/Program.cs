using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accessibility;
using Competition;
using Coordinates;
using Coordinates.Parsers;
using CoordinateSharp;
using OfficeOpenXml.FormulaParsing.Excel.Functions;

namespace TestProgramm
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Montgolfiade_DM2022.CalculateFlight5();
        }



        private static string ToProperText(CoordinateSharp.CoordinatePart part)
        {
            string text = part.Degrees + "° " + part.Minutes + "ʹ " + Math.Round(part.Seconds, 2, MidpointRounding.AwayFromZero) + "ʺ";
            return text;
        }

    }
}
