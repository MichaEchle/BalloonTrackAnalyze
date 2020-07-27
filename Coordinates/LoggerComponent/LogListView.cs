using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

namespace LoggerComponent
{
    public partial class LogListView : UserControl
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////
        // Enums
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #region Enums
        private enum LogIconType
        {
            LogInfo = 0,
            LogWarning = 1,
            LogError=2
            //LogCritical = 2,
            //LogFatal = 3
            //LogException = 4,
        }
        #endregion Enums

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // Properties
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #region Properties
        /// <summary>
        /// Path + file name to logfile (if available)
        /// </summary>
        public string LogFilePath
        {
            get; private set;
        }

        private bool LoggingHasBeenStarted
        {
            get; set;
        }
        private bool LogFileWriterHasBeenStarted
        {
            get; set;
        }
        private bool ColumnAutoSizingEnabled
        {
            get
            {
                return m_columnAutoSizingEnabled;
            }
            set
            {
                m_columnAutoSizingEnabled = value;
            }
        }
        private bool m_columnAutoSizingEnabled = true;

        /// <summary>
        /// Callback function used to filter specific logs
        /// <para>returns true if item should be logged; returns false if item should not be logged</para>
        /// </summary>
        private Func<object, LogItem, bool> LogFilterCallBack
        {
            get; set;
        }
        #endregion Properties

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor(s)
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #region Constructor(s)
        public LogListView()
        {
            InitializeComponent();

            // prepare control's clean up
            this.Disposed += new EventHandler(BFLogListView_Disposed);
        }
        #endregion Constructor(s)

        private void Log(LogSeverityType severity, string text, params object[] list)
        {
            Logger.Log(this, severity, text, list);
        }

        private void Log(object logSource, LogSeverityType severity, string text, params object[] list)
        {
            Logger.Log(logSource, severity, text, list);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // API functions
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #region API functions
        /// <summary>
        /// Scroll down log list view to latest log entry
        /// </summary>
        public void EnsureVisible()
        {
            LogListViewer.EnsureVisible(LogListViewer.Items.Count - 1);
        }

        /// <summary>
        /// Start logging and write logs into logfile
        /// </summary>
        /// <param name="logFilePath">logfile path + filename</param>
        /// <param name="backupFileCount">number of backup files to be created circularly</param>
        public void StartLogging(string logFilePath = "", int backupFileCount = 1)
        {
            // enable re-entrance
            if (!LoggingHasBeenStarted)
            {
                if (logFilePath != "")
                {
                    try
                    {
                        FileInfo fi = new FileInfo(logFilePath);
                        logFilePath = fi.FullName;      // resolve logfile path to full name to be independent of "System.Environment.CurrentDirectory" which might change within the current thread (or process)
                    }
                    catch (Exception ex)
                    {
                        Log(LogSeverityType.Error, "Failed to start logging: Logfile path '{0}' is not valid: '{1}'", logFilePath, ex.Message);
                        return;     // error
                    }

                    // start log writer
                    Logger.StartLogFileWriter(logFilePath, false, backupFileCount, LogFilterCallBack);
                    LogFileWriterHasBeenStarted = true;
                }

                LogFilePath = logFilePath;

                // subscribe log call event handler to log call event
                Logger.LogCall += new Logger.LogEventHandler(OnLogIntoListView);
                LoggingHasBeenStarted = true;

                Log((object)(ParentForm.ToString().StartsWith(ParentForm.GetType().ToString()) ? ParentForm.Text : ParentForm.ToString()), LogSeverityType.Info, "Start Logging...");
            }
        }

        /// <summary>
        /// Stop logging
        /// <para>This function is called automatically after the log list view control has been disposed</para>
        /// </summary>
        public void StopLogging()
        {
            // enable re-entrance
            if (LoggingHasBeenStarted)
            {
                Log(LogSeverityType.Info, "Stop Logging...");

                // remove log call event handler from log call event
                Logger.LogCall -= new Logger.LogEventHandler(OnLogIntoListView);
                LoggingHasBeenStarted = false;

                // (optionally) stop log writer
                if (LogFileWriterHasBeenStarted)
                {
                    Logger.StopLogFileWriter();
                    LogFileWriterHasBeenStarted = false;
                }
            }
        }

        /// <summary>
        /// Attach a (non-self-managed) existing logfile (+path) to the log list view's context menu, to enable "Open ..." context menu item
        /// <para>If logging has been started using a logfile (+path), attaching to it is done automatically</para>
        /// </summary>
        /// <param name="logFilePath">path + filename of logfile</param>
        public void AttachContextMenuToExistingLogFile(string logFilePath)
        {
            LogFilePath = logFilePath;
        }

        /// <summary>
        /// Attach log filter callback function to perform user-specific log filtering
        /// </summary>
        /// <param name="logFilterCallBack">callback function used to filter specific logs</param>
        public void AttachLogFilterCallBack(Func<object, LogItem, bool> filterCallback)
        {
            LogFilterCallBack = filterCallback;
        }
        #endregion API functions

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // Context menu
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #region Context menu functions
        private void LogListViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            OpenLogfileToolStripMenuItem.Visible = (LogFilePath != "");
            OpenLogfileLocationToolStripMenuItem.Visible = (LogFilePath != "");

            ClearLogsToolStripMenuItem.Enabled = (LogListViewer.Items.Count > 0);
            CopyLogLineToolStripMenuItem.Enabled = (LogListViewer.Items.Count > 0) && (LogListViewer.SelectedItems.Count > 0);
        }

        private void OpenLogfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogFileViewer logFileViewer = new LogFileViewer(LogFilePath);
            logFileViewer.Show(this);
            logFileViewer.ReadLog();
            string searchItem = "";
            if (LogListViewer.SelectedItems.Count > 0)
                searchItem = ((LogItem)LogListViewer.SelectedItems[0].Tag).ToString();
            logFileViewer.SearchItem = searchItem;
        }

        private void OpenLogfileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer", $"/select, \"{LogFilePath}\"");
            }
            catch (Exception ex)
            {
                Log(LogSeverityType.Error, "Failed to open explorer to logfile location '{0}': {1}", LogFilePath, ex.Message);
            }
        }

        private void ClearLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogListViewer.Items.Clear();
        }

        private void CopyLogLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LogListViewer.SelectedItems.Count == 0)
                return;

            AutoResetEvent copyToClipboardAutoResetEvent = new AutoResetEvent(false);
            try
            {
                LogItem logItem = (LogItem)LogListViewer.SelectedItems[0].Tag;

                Thread copyToClipboardThread = new Thread(delegate (object data)
                {
                    Clipboard.SetDataObject(data as string, true);
                    copyToClipboardAutoResetEvent.Set();
                });
                copyToClipboardThread.SetApartmentState(ApartmentState.STA);
                copyToClipboardThread.Name = "CopyLogLineToClipboardThread";
                //copyToClipboardThread.Start(logItem.Text);
                copyToClipboardThread.Start(logItem.ToString());
                copyToClipboardAutoResetEvent.WaitOne();

                copyToClipboardAutoResetEvent.Reset();
            }
            catch (Exception ex)
            {
                Log(LogSeverityType.Error, "Failed to copy selected log line to clipboard: {0}", ex);
            }
        }
        #endregion Context menu functions

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // Helper functions
        ////////////////////////////////////////////////////////////////////////////////////////////////
        #region Helper functions
        private void BFLogListView_Load(object sender, EventArgs e)
        {
            // prepare log icons for list view
            ImageList logIcons = new ImageList();
            //foreach (LogIconType iconType in Enum.GetValues(typeof(LogIconType)))
            //    logIcons.Images.Add(new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("BFBase3.icons.{0}.ico", iconType))));
            logIcons.Images.Add(Properties.Resources.StatusInformation_exp_16x);
            logIcons.Images.Add(Properties.Resources.StatusWarning_exp_16x);
            logIcons.Images.Add(Properties.Resources.StatusCriticalError_exp_16x);
            LogListViewer.SmallImageList = logIcons;

            AutoSizeColumns();

            // additional event handlers
            this.LogListViewer.SizeChanged += new System.EventHandler(this.LogListView_SizeChanged);
            this.LogListViewer.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.LogListView_ColumnWidthChanged);
        }

        private void BFLogListView_Disposed(object sender, EventArgs e)
        {
            StopLogging();
        }

        private void OnLogIntoListView(LogItem logItem)
        {
            bool useLogItem = true;

            // (optionally) filter log item
            if (LogFilterCallBack != null)
                useLogItem = LogFilterCallBack(this, logItem);

            if (useLogItem)
            {
                // get fore and back color
                Color foreColor = LogListViewer.ForeColor;
                Color backColor = LogListViewer.BackColor;
                if (logItem.Severity == LogSeverityType.Error)
                    backColor = Color.Red;
                //else if (logItem.Severity == LogSeverityType.Critical)
                //    backColor = Color.Yellow;
                else if (logItem.Severity == LogSeverityType.Warning)
                    backColor = Color.Yellow;

                // create and fill a new list view item
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = logItem.TimeStamp.ToString();
                listViewItem.Tag = logItem;
                listViewItem.ForeColor = foreColor;
                listViewItem.BackColor = backColor;
                listViewItem.SubItems.Add(logItem.Text);
                listViewItem.SubItems.Add(logItem.Source.ToString());
                if (logItem.Severity == LogSeverityType.Info)
                    listViewItem.ImageIndex = (int)LogIconType.LogInfo;
                else if (logItem.Severity == LogSeverityType.Warning)
                    listViewItem.ImageIndex = (int)LogIconType.LogWarning;
                //else if (logItem.Severity == LogSeverityType.Critical)
                //    listViewItem.ImageIndex = (int)LogIconType.LogCritical;
                else if (logItem.Severity == LogSeverityType.Error)
                    listViewItem.ImageIndex = (int)LogIconType.LogError;
                listViewItem.ToolTipText = logItem.ToString();

                // add created list view item to log list view
                ListViewLogItemsAdd(listViewItem);
            }
        }

        /// <summary>
        /// Add log list view item
        /// </summary>
        private void ListViewLogItemsAdd(ListViewItem listViewItem)
        {
            // ensure visibility
            if (LogListViewer.InvokeRequired)
            {
                try
                {
                    LogListViewer.Invoke(new Action<ListViewItem>(ListViewLogItemsAdd), new object[] { listViewItem });
                }
                catch (Exception)
                {
                    Log(LogSeverityType.Warning, "Failed to log '{0}', because GUI thread already terminated", listViewItem.Text);
                }
            }
            else
            {
                LogListViewer.Items.Add(listViewItem);
                LogListViewer.EnsureVisible(LogListViewer.Items.Count - 1);
            }
        }

        private void LogListView_SizeChanged(object sender, EventArgs e)
        {
            if (ColumnAutoSizingEnabled)
                AutoSizeColumns();
        }

        private void LogListView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            ColumnAutoSizingEnabled = false;
        }

        private void AutoSizeColumns()
        {
            ColumnHeaderText.Width = ClientSize.Width - ColumnHeaderTimeStamp.Width - ColumnHeaderSource.Width - 20;
            ColumnAutoSizingEnabled = true;
        }

        private void LogListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (LogFileWriterHasBeenStarted)
                OpenLogfileToolStripMenuItem_Click(sender, e);
        }
        #endregion Helper functions
    }
}
