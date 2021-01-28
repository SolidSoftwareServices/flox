using System;
using System.Globalization;
using EI.RP.CoreServices.Profiling;
using NLog;
using StackExchange.Profiling;

namespace Ei.Rp.Mvc.Core.Profiler
{
	internal class MiniProfilerDecorator : IProfiler
	{
		private readonly MiniProfiler _profiler;

		public MiniProfilerDecorator()
		{
			_profiler = MiniProfiler.Current;
		}

		public IDisposable RecordStep(string stepId)
		{
			var step = _profiler.Step(stepId);
			var timing = new TimingDecorator(step);
			
			return timing;
		}

		public IDisposable Profile(string categoryId, string commandId)
		{
			return new TimingDecorator(_profiler.Step($"{categoryId}-{commandId}"));
		}

		private class TimingDecorator:IDisposable
		{
			private static readonly ILogger Logger = LogManager.GetLogger("ProfileLogger");
			private readonly Timing _timing;

			public TimingDecorator(Timing timing)
			{
				_timing = timing;
			}

			public void Dispose()
			{
				if (_timing != null)
				{
					((IDisposable) _timing).Dispose();
					var timingName = _timing.Name;
					var timingDurationMilliseconds = _timing.DurationMilliseconds ?? 0M;

					Logger.Trace(() => $"{timingDurationMilliseconds.ToString("{0.0}",CultureInfo.InvariantCulture).PadLeft(6)} (ms) - {timingName}");
					
				}
			}
		}
	}
}