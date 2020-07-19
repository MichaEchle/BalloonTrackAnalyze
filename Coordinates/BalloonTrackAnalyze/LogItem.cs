using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BalloonTrackAnalyze
{
	public sealed class LogItem
	{
		public LogItem()
		{
		}   // needed for xml serialization

		/// <summary>
		/// Constructor
		/// </summary>
		public LogItem(object source, LogSeverityType severity, string text)
		{
			// set values
			Source = source;
			m_severity = severity;
			m_text = text;
			TimeStamp = DateTime.Now;
		}

		[XmlIgnore]
		public bool IsUserMessage
		{
			get
			{
				return (Source == LogItem.UserMessageLogSource);
			}
		}

		public LogSeverityType Severity
		{
			get
			{
				return m_severity;
			}
			set
			{
				m_severity = value;
			}
		}
		private LogSeverityType m_severity;

		[XmlIgnore]
		public object Source
		{
			get
			{
				return m_source;
			}
			set
			{
				m_source = value;
				if (m_source != null)
					SourceName = m_source.ToString();
			}
		}
		//		[NonSerialized]
		private object m_source;

		/// <summary>
		/// Get access for log source name
		/// (needed for de-/serialization from/into database)
		/// </summary>
		public string SourceName
		{
			get
			{
				return m_sourceName;
			}
			set
			{
				m_sourceName = value;
			}
		}
		private string m_sourceName = "";

		public string Text
		{
			get
			{
				return m_text;
			}
			set
			{
				m_text = value;
			}
		}
		private string m_text;

		public DateTime TimeStamp
		{
			get
			{
				return m_timeStamp;
			}
			set
			{
				m_timeStamp = value;
			}
		}
		private DateTime m_timeStamp;

		public override string ToString()
		{
			//string logSource = Source.ToString();

			//string logLine = string.Format("{0}: {1} (at {2}{3})",
			//    Severity,
			//    Text,
			//    TimeStamp,
			//    (logSource !=  NoLogSource.ToString()) ? " logged from " + logSource : "");

			string logLine = string.Format("{0}: {1}: {2}{3}",
				TimeStamp,
				Severity,
				Text,
				//(logSource != NoLogSource.ToString()) ? " (logged from " + logSource + ")" : "");
				(Source != LogItem.NoLogSource) ? string.Format(" (logged from '{0}')", Source) : "");

			return logLine;
		}

		[XmlIgnore]
		public static object NoLogSource
		{
			get
			{
				return m_noLogSource;
			}
		}
		//		[NonSerialized]
		private static string m_noLogSource = "unknown";

		[XmlIgnore]
		public static object UserMessageLogSource
		{
			get
			{
				return m_userMessageLogSource;
			}
		}
		//		[NonSerialized]
		private static string m_userMessageLogSource = "User Message";
	}
}
