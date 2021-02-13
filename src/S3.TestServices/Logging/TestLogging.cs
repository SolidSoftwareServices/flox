using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;

namespace S3.TestServices.Logging
{
	public class TestLogging
	{
		public static readonly TestLogging Default = new TestLogging();

		private TestLogging()
		{
		}

		private bool _configured = false;
		private readonly object _syncLock=new object();
		public void ConfigureLogging(string layout = null, LogLevel minLogLevel = null)
		{
			if(!_configured)
			{
				lock (_syncLock)
					if (!_configured)
					{
						ConfigureLogging(layout, minLogLevel?.ToString());
						_configured = true;
					}
					else
					{
						ShowAlreadyConfiguredMessage();
					}
			}
			else
			{
				ShowAlreadyConfiguredMessage();
			}
			void ShowAlreadyConfiguredMessage()
			{
				LogManager.GetCurrentClassLogger().Warn(() => "Logging was already configured, swallowing this call...");
			}

		}

		private void ConfigureLogging(string layout = null, string minLogLevel = null)
		{
			var config = new LoggingConfiguration();
			// Step 2. Create targets
			var consoleTarget = new ColoredConsoleTarget("console")
			{
				Layout = layout ?? @"${date:format=HH\:mm\:ss.ffff} ${logger} -> ${level} ${message} ${exception}"
			};
			config.AddTarget(consoleTarget);
			config.AddRuleForAllLevels(consoleTarget);

			if (minLogLevel != null)
			{
				var logLevel = LogLevel.FromString(minLogLevel);
				if (logLevel.Ordinal > LogLevel.Trace.Ordinal)
				{
					var min = logLevel == LogLevel.Trace ? logLevel : LogLevel.FromOrdinal(logLevel.Ordinal - 1);
					foreach (var configLoggingRule in config.LoggingRules)
					{
						configLoggingRule.DisableLoggingForLevels(LogLevel.Trace, min);
					}
				}
			}

			LogManager.Configuration = config;

			Verify();

			void Verify()
			{
				
					var logger=LogManager.GetCurrentClassLogger();

					var logLevel =minLogLevel!=null? LogLevel.FromString(minLogLevel):LogLevel.Trace;
					foreach (var level in LogLevel.AllLevels.Where(x=>x.Ordinal<logLevel.Ordinal))
					{
						Assert.IsFalse(logger.IsEnabled(level),$"The log level {level.Name} was not DISABLED");
					}
					foreach (var level in LogLevel.AllLevels.Where(x=>x.Ordinal>=logLevel.Ordinal && x!=LogLevel.Off))
					{

						Assert.IsTrue(logger.IsEnabled(level),$"The log level {level.Name} was not ENABLED");
					}
				
			}
		}

		public void Flush()
		{
			LogManager.Flush();
		}
	}
}