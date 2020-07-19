using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BalloonTrackAnalyze
{
	/// <summary>
	/// Severity types for log items
	/// </summary>
	public enum LogSeverityType
	{
		Info = 0,
		Warning = 1,
		Error=2
		//Critical = 2,
		//Fatal = 3
		//Exception = 4
	};

	/// <summary>
	/// The one and only log
	/// <newpara>Author: Joerg Staudigel</newpara>
	/// </summary>
	/// <remarks>
	/// No remarks.
	/// </remarks>
	public sealed class Logger
	{
		/// <summary>
		/// New log event
		/// </summary>
		public static event LogEventHandler LogCall;

		/// <summary>
		/// Delegate definition for log event handler
		/// </summary>
		public delegate void LogEventHandler(LogItem logItem);

		/// <summary>
		/// Get access for log items
		/// </summary>
		/// <remarks>Key: log source</remarks>
		public static Dictionary<object, List<LogItem>> LogItems
		{
			get
			{
				return m_logItems;
			}
		}
		private static Dictionary<object, List<LogItem>> m_logItems = new Dictionary<object, List<LogItem>>();

		/// <summary>
		/// Log
		/// </summary>
		private static void Log(LogItem logItem)
		{
			lock (logItem)      // to keep to order of the logItems
			{
				//#if DEBUG
				// write log entry to visual studio output window
				string logLine = GetLogLineFromLogItem(logItem);
				Trace.WriteLine(logLine);
				Trace.Flush();
				//#endif
				// add log item to log items, order by log source
				List<LogItem> subLogItems;
				if (LogItems.ContainsKey(logItem.Source))
					subLogItems = LogItems[logItem.Source];
				else
				{
					subLogItems = new List<LogItem>();
					LogItems[logItem.Source] = subLogItems;
				}
				subLogItems.Add(logItem);

				// fire log call event
				if (LogCall != null)
					LogCall(logItem);
			}

		}

		/// <summary>
		/// Log
		/// </summary>
		public static void Log(string text, params object[] list)
		{
			// log
			Log(LogSeverityType.Info, text, list);
		}

		/// <summary>
		/// Log
		/// </summary>
		public static void Log(object source, string text, params object[] list)
		{
			// log
			Log(source, LogSeverityType.Info, text, list);
		}

		/// <summary>
		/// Log
		/// </summary>
		public static void Log(LogSeverityType severity, string text, params object[] list)
		{
			// log
			Log(LogItem.NoLogSource, severity, text, list);
		}

		/// <summary>
		/// Log
		/// </summary>
		public static void Log(object source, LogSeverityType severity, string text, params object[] list)
		{
			// create log item
			char[] finalText;
			if ((list != null) && (list.Length > 0))
				finalText = string.Format(text, list).ToCharArray();
			else
				finalText = text.ToCharArray();
			for (int i = 0; i < finalText.Length; i++)
			{
				if (finalText[i] == '\n')
					finalText[i] = ' ';
			}
			LogItem logItem = new LogItem(source, severity, new String(finalText));

			// log item
			Log(logItem);
		}

		/// <summary>
		/// Log
		/// </summary>
		public static void Log(Exception exception)
		{
			// log
			Log(LogItem.NoLogSource, exception);
		}

		/// <summary>
		/// Log
		/// </summary>
		public static void Log(object source, Exception exception)
		{
			Log(new LogItem(source, LogSeverityType.Error, string.Format("Exception: {0}", exception.Message)));

			//WL 10.11.2008: check for availability of StackTrace
			if (exception.StackTrace != null)
			{
				string[] stackTraceLines = exception.StackTrace.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string stackTraceLine in stackTraceLines)
					Log(new LogItem(source, LogSeverityType.Error, stackTraceLine.Trim()));
			}
		}

		/// <summary>
		/// Start log file writer
		/// </summary>
		/// <param name="filePath">log file name (+optional path)</param>
		/// <param name="append">open log file for appending text</param>
		/// <param name="backupFileCount">max. number of backup files to be created before opening log file (only if appending is disabled)</param>
		/// <param name="logFilterCallBack">callback function used to filter specific logs</param>
		/// <returns>true: success; false: error</returns>
		public static bool StartLogFileWriter(string filePath, bool append, int backupFileCount, Func<object, LogItem, bool> logFilterCallBack = null)
		{
			// create new log file writer
			m_logFileWriter = new LogFileWriter(filePath, append);
			FileInfo fileInfo = null;

			try
			{
				fileInfo = new FileInfo(filePath);
			}
			catch (Exception ex)
			{
				Log(LogSeverityType.Error, "Failed to start logfile writer: Logfile path '{0}' is not valid: '{1}'", filePath, ex.Message);
				return false;
			}

			// check if directory exists
			if (!Directory.Exists(fileInfo.DirectoryName))
			{
				try
				{
					Directory.CreateDirectory(fileInfo.DirectoryName);
				}
				catch (Exception ex)
				{
					Log(LogSeverityType.Error, "Failed to create start logfile writer: Failed to create missing directory structure: {0}", ex.Message);
					return false;
				}
			}

			if (!append)
			{
				// (optionally) create backup file (e.g. "Logfile_20080322_141753.txt")
				try
				{
					if ((File.Exists(filePath)) && (backupFileCount > 0))
					{
						string fileNameWithoutExtension = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
						FileInfo[] fis = fileInfo.Directory.GetFiles(string.Format("{0}_*.txt", fileNameWithoutExtension), SearchOption.TopDirectoryOnly);
						List<string> fileNames = new List<string>();
						foreach (FileInfo fi in fis)
						{
							if (Regex.Match(fi.Name, "^" + fileNameWithoutExtension + @"_(\d{8})_(\d{6})\.txt").Success)
								fileNames.Add(fi.Name);
						}

						fileNames.Sort();
						File.Copy(filePath, string.Format("{0}\\{1}_{2}.txt", fileInfo.DirectoryName, fileNameWithoutExtension, fileInfo.LastWriteTime.ToString("s").Replace("-", "").Replace("T", "_").Replace(":", "")), true);

						// delete old log file backups
						for (int index = fileNames.Count - backupFileCount; index >= 0; index--)
							File.Delete(string.Format("{0}\\{1}", fileInfo.DirectoryName, fileNames[index]));
					}
				}
				catch (Exception e)
				{
					MessageBox.Show(string.Format("Failed to create log file backup:\r\n\r\n'{0}'", e.Message), "Log file error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			// start
			if (!m_logFileWriter.Start(logFilterCallBack))
				return false; // error

			// success
			return true;
		}
		private static LogFileWriter m_logFileWriter;

		/// <summary>
		/// Stop log file writer
		/// </summary>
		/// <returns>true: success; false: error</returns>
		public static bool StopLogFileWriter()
		{
			// stop
			if (!m_logFileWriter.Stop())
				return false; // error

			// success
			return true;
		}

		/// <summary>
		/// Convert log items into a multiline string
		/// </summary>
		public static string ToMultiLineString(List<List<LogItem>> logItems, string lineSeparator)
		{
			StringBuilder logText = new StringBuilder();
			if (logItems != null)
			{
				foreach (List<LogItem> logSection in logItems)
				{
					foreach (LogItem logItem in logSection)
						//logText.AppendFormat("{0}: {1}{2}", logItem.Severity.ToString(), logItem.Text, lineSeparator);
						logText.AppendFormat("{0}{1}", GetLogLineFromLogItem(logItem), lineSeparator);
				}
			}
			return logText.ToString();
		}

		internal static string GetLogLineFromLogItem(LogItem logItem)
		{
			return logItem.ToString();

			//string logLine = string.Format("{0}: {1}: {2}{3}",
			//    logItem.TimeStamp,
			//    logItem.Severity,
			//    logItem.Text,
			//    (logItem.Source != LogItem.NoLogSource) ? string.Format(" (logged from '{0}')", logItem.Source) : "");

			//return logLine;
		}
	}
}
