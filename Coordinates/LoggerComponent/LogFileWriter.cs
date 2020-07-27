using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LoggerComponent
{
	internal class LogFileWriter
	{
		private string m_filePath;
		private bool m_append;
		private StreamWriter m_logFile;
		private List<LogItem> m_logItems = new List<LogItem>();
		private Thread m_logFileWriterThread;

		/// <summary>
		/// Callback function used to filter specific logs
		/// <para>returns true if item should be logged; returns false if item should not be logged</para>
		/// </summary>
		private Func<object, LogItem, bool> LogFilterCallBack
		{
			get; set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		internal LogFileWriter(string filePath, bool append)
		{
			// set filename
			m_filePath = filePath;

			// set append flag
			m_append = append;
		}

		/// <summary>
		/// Start
		/// </summary>
		/// <param name="logFilterCallBack">callback function used to filter specific logs</param>
		/// <returns>true: success; false: error</returns>
		internal bool Start(Func<object, LogItem, bool> logFilterCallBack = null)
		{
			LogFilterCallBack = logFilterCallBack;

			// create new sequencer thread
			m_logFileWriterThread = new Thread(new ThreadStart(Run));
			m_logFileWriterThread.IsBackground = true;      // if all foreground threads have finished, the main process will no longer wait for LogFileWriter's thread to terminate prior to exit itself

			// set name
			m_logFileWriterThread.Name = "Logfile Writer";

			// run thread
			m_logFileWriterThread.Start();

			// success
			return true;
		}

		/// <summary>
		/// The one and only run method
		/// </summary>
		private void Run()
		{
			// set culture to english-US
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

			try
			{
				// create chronological log file stream
				FileStream logFileStream = new FileStream(m_filePath, m_append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

				// create stream writer for log file stream
				m_logFile = new StreamWriter(logFileStream);
			}

			catch (Exception e)
			{
				MessageBox.Show(string.Format("Could not create file: {0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return; // error
			}

			// subscribe log call event handler to log call event
			Logger.LogCall += new Logger.LogEventHandler(OnLogCall);

			try
			{
				// infite loop, can only be broken e.g. by an abort thread exception
				while (true)
				{
					while (m_logItems.Count > 0)
					{
						// get current log item
						LogItem logItem = m_logItems[0];

						// write log entry
						if (logItem != null)
						{
							//m_logFile.WriteLine(logItem.ToString());
							string logLine = Logger.GetLogLineFromLogItem(logItem);
							m_logFile.WriteLine(logLine);
						}

						// remove saved log container
						m_logItems.RemoveAt(0);
					}

					// flush log file
					m_logFile.Flush();

					// give other threads a chance to do something
					Thread.Sleep(10);
				}
			}

			catch
			{
				// remove log call event handler from log call event
				Logger.LogCall -= new Logger.LogEventHandler(OnLogCall);

				try
				{
					// flush log file
					m_logFile.Flush();

					// close chronological log file
					m_logFile.Close();
				}

				catch (Exception e)
				{
					MessageBox.Show(string.Format("Could not close file: {0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return; // error
				}
			}
		}

		/// <summary>
		/// Log call event handler
		/// </summary>
		internal void OnLogCall(LogItem logItem)
		{
			bool useLogItem = true;

			// (optionally) filter log item
			if (LogFilterCallBack != null)
				useLogItem = LogFilterCallBack(this, logItem);

			// (optionally) add log item to log items
			if (useLogItem)
				m_logItems.Add(logItem);
		}

		/// <summary>
		/// Stop
		/// </summary>
		/// <returns>true: success; false: error</returns>
		internal bool Stop()
		{
			try
			{
				// give the thread the chance to write some more log items
				if (m_logItems.Count > 0)
					Thread.Sleep(100);

				// check if thread exists
				if (m_logFileWriterThread != null)
				{
					// check if thread is alive
					if (m_logFileWriterThread.IsAlive)
					{
						// abort thread
						m_logFileWriterThread.Abort();
					}
				}
			}

			catch (Exception e)
			{
				Logger.Log(e);
				Logger.Log(LogSeverityType.Error, "Stop of log file writer failed");
				return false;
			}

			// success
			return true;
		}
	}
}
