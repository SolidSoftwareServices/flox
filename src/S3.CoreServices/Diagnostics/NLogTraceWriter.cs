using System;
using System.Diagnostics;
using Newtonsoft.Json.Serialization;
using NLog;

namespace S3.CoreServices.Diagnostics
{
	class NLogTraceWriter : ITraceWriter
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public TraceLevel LevelFilter => TraceLevel.Verbose;

		public void Trace(TraceLevel level, string message, Exception ex)
		{
			var logEvent = new LogEventInfo
			{
				Message = message,
				Level = GetLogLevel(level),
				Exception = ex
			};

			Logger.Log(logEvent);
		}

		private LogLevel GetLogLevel(TraceLevel level)
		{
			switch (level)
			{
				
				case TraceLevel.Error:
					return LogLevel.Error;
				case TraceLevel.Warning:
					return LogLevel.Warn;
				case TraceLevel.Info:
					return LogLevel.Info;
				case TraceLevel.Off:
					return LogLevel.Off;
				default:
					return LogLevel.Trace;
			}
		}
	}
}
