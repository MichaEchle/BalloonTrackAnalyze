using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace BalloonTrackAnalyze
{
    public partial class LogFileViewer : Form
    {
        ////////////////////////////////////////////////////////////////////////////////
        // Interop
        ////////////////////////////////////////////////////////////////////////////////
        #region Interop
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
        #endregion Interop

        ////////////////////////////////////////////////////////////////////////////////
        // Constants
        ////////////////////////////////////////////////////////////////////////////////
        #region Constants
        private const int m_linesBefor = 100;
        private const int m_linesAfter = 100;

        private const int WM_USER = 0x400;
        private const int EM_HIDESELECTION = WM_USER + 63;

        #endregion Constants

        ////////////////////////////////////////////////////////////////////////////////
        // Properties
        ////////////////////////////////////////////////////////////////////////////////
        #region Properties
        private String m_searchString = "";
        private delegate void HiLighterDelegate();
        private delegate void LogFileStringBuilderDelegate();

        private Dictionary<string, Color> m_lookupTable = new Dictionary<string, Color>();
        private Thread m_readerThread = null;
        private Thread m_hiLighterThread = null;

        /// <summary>
        /// returns the path to the logFile which will be viewed
        /// </summary>
        private String LogFilePath
        {
            get; set;
        }

        /// <summary>
        /// The viewer will look for this item and will highlight it.
        /// </summary>
        public String SearchItem
        {
            get; set;
        }

        /// <summary>
        /// get access to the masked search item
        /// </summary>
        private String MaskedSearchItem
        {
            get
            {
                if (m_maskedSearchItem == "")
                    m_maskedSearchItem = Regex.Escape(SearchItem);
                return m_maskedSearchItem;
            }
        }
        private String m_maskedSearchItem = "";

        /// <summary>
        /// provides the string which represents the content of the log file
        /// </summary>
        private String LogFileString
        {
            get; set;
        }
        #endregion Properties

        ////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ////////////////////////////////////////////////////////////////////////////////
        #region Constructors
        public LogFileViewer(String logFilePath)
        {
            LogFilePath = logFilePath;
            InitializeComponent();
            richTextBox1.WordWrap = false;

            // init lookup table
            m_lookupTable.Add("SearchItem", Color.Brown);
            m_lookupTable.Add("Info", Color.LightGreen);
            m_lookupTable.Add("Warning", Color.Yellow);
            //m_lookupTable.Add("Critical", Color.Yellow);
            m_lookupTable.Add("Error", Color.Red);
        }
        #endregion Constructors

        /// <summary>
		/// read the LogFile into the FileReader
		/// </summary>
		private void ReadLogFile()
        {
            try
            {
                IAsyncResult result = richTextBox1.BeginInvoke(new LogFileStringBuilderDelegate(LogFileStringBuilder));
                richTextBox1.EndInvoke(result);
            }
            catch (Exception)
            {

            }
            m_hiLighterThread.Start();
        }

        /// <summary>
        /// Builds a string from the content of the log file
        /// </summary>
        private void LogFileStringBuilder()
        {
            FileStream fileStream = new FileStream(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StringBuilder logFileBuilder = new StringBuilder();
            StreamReader reader = new StreamReader(fileStream);
            LogFileString = reader.ReadToEnd();

            // cuts the string from the logfile to the searchItem +- a specified count of lines
            Regex regEx = new Regex(MaskedSearchItem, RegexOptions.Compiled);
            Match firstMatch = regEx.Match(LogFileString, 0);
            regEx = new Regex("\n", RegexOptions.RightToLeft);
            MatchCollection lineBreaksBefor = regEx.Matches(LogFileString, firstMatch.Index);
            int startPosition = 0;
            if (lineBreaksBefor.Count > 100)
                startPosition = lineBreaksBefor[m_linesBefor].Index;
            System.Text.Encoding aCode = System.Text.Encoding.UTF8;
            byte[] content = aCode.GetBytes(LogFileString);
            MemoryStream logStream = new MemoryStream(content);

            StreamReader reader2 = new StreamReader(logStream);
            reader2.BaseStream.Seek(startPosition, SeekOrigin.Begin);
            m_searchString = reader2.ReadToEnd();

            richTextBox1.Text = m_searchString.Trim();
        }

        private void HiLighter()
        {
            IAsyncResult result = richTextBox1.BeginInvoke(new HiLighterDelegate(HighLight), null);
            richTextBox1.EndInvoke(result);
        }

        /// <summary>
        /// starts reading of the logFile
        /// </summary>
        /// <returns></returns>
        public void ReadLog()
        {
            m_readerThread = new Thread(new ThreadStart(ReadLogFile));
            m_readerThread.Name = "Logfile Viewer Reader";
            m_hiLighterThread = new Thread(new ThreadStart(HiLighter));
            m_hiLighterThread.Name = "Logfile Viewer HiLighter";
            m_readerThread.Start();
        }

        //Highlight the textbox
        private void HighLight()
        {
            // set focus to (hidden) label to prevent auto scrolling of richtext box
            labelToFocus.Focus();
            SendMessage(richTextBox1.Handle, EM_HIDESELECTION, 1, 0);

            //hiLight Info,Warning,Critical,Fatal
            MatchCollection matches = Regex.Matches(richTextBox1.Text, "Info:|Warning:|Error:", RegexOptions.Compiled);
            foreach (Match match in matches)
            {
                richTextBox1.SelectionStart = match.Index;
                richTextBox1.SelectionLength = match.Length;
                richTextBox1.SelectionBackColor = GetLogSeverityColor(match.Value);
            }

            int lineStartIndex = 0;
            for (int lineIndex = 0; lineIndex < richTextBox1.Lines.Length; lineIndex++)
            {
                string line = richTextBox1.Lines[lineIndex];
                if (line.Trim() == "")
                    continue;
                if (SearchItem.StartsWith(line))
                {
                    // highlight searchItem
                    richTextBox1.SelectionStart = lineStartIndex;
                    richTextBox1.SelectionLength = line.Length;
                    richTextBox1.SelectionBackColor = GetLogSeverityColor(line);
                    richTextBox1.SelectionFont = new Font("Times New Roman", 12);

                    // scroll richtext box to show searchItem somewhere in the middle
                    int finalLineOffset = (int)Math.Max(lineIndex - 20, 0);
                    richTextBox1.SelectionStart = richTextBox1.GetFirstCharIndexFromLine(finalLineOffset);
                    richTextBox1.SelectionLength = 0;
                    richTextBox1.ScrollToCaret();

                    // set cursor to start of search item's line to keep line visible if wrapping gets changed
                    richTextBox1.SelectionStart = lineStartIndex;
                    break;
                }

                lineStartIndex += line.Length + 1;
            }

            richTextBox1.DeselectAll();

            // set back focus to richtext box
            SendMessage(richTextBox1.Handle, EM_HIDESELECTION, 0, 0);
            richTextBox1.Focus();
        }

        private Color GetLogSeverityColor(string logLine)
        {
            Match match = Regex.Match(logLine, "Info:|Warning:|Error:", RegexOptions.Compiled);
            if (match.Success)
                return m_lookupTable[match.Value.Trim(':')];
            else
                return Color.Black;
        }


        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoResetEvent copyToClipboardAutoResetEvent = new AutoResetEvent(false);
            try
            {
                Thread copyToClipboardThread = new Thread(delegate (object data)
                {
                    Clipboard.SetDataObject(data as string, true);
                    copyToClipboardAutoResetEvent.Set();
                });
                copyToClipboardThread.SetApartmentState(ApartmentState.STA);
                copyToClipboardThread.Name = "CopyLogLinesToClipboardThread";
                string textToCopy = richTextBox1.SelectedText.Replace("\n", "\r\n").Replace("\r\n\n", "\r\n");  // for notepad only
                if (textToCopy.Trim() == "")
                    textToCopy = SearchItem;
                if (textToCopy.Trim() != "")
                {
                    copyToClipboardThread.Start(textToCopy);
                    copyToClipboardAutoResetEvent.WaitOne();
                    copyToClipboardAutoResetEvent.Reset();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverityType.Error, "Failed to copy selected log line(s) to clipboard: {0}", ex);
            }
        }

        private void OpenLogfileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer", $"/select, \"{LogFilePath}\"");
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverityType.Error, "Failed to open explorer to logfile location '{0}': {1}", LogFilePath, ex.Message);
            }
        }

        private void WrapTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.WordWrap = WrapTextToolStripMenuItem.Checked;
        }

        private void WrapTextToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (WrapTextToolStripMenuItem.Checked)
                WrapTextToolStripMenuItem.BackColor = Color.FromKnownColor(KnownColor.ActiveBorder);
            else
                WrapTextToolStripMenuItem.BackColor = Color.FromKnownColor(KnownColor.Control);
        }
    }
}
