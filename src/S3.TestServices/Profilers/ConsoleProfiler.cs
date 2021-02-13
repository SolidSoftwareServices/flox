using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using S3.CoreServices.Profiling;
using S3.CoreServices.System;
using StackExchange.Profiling;
using StackExchange.Profiling.Internal;
using StackExchange.Profiling.Storage;

namespace S3.TestServices.Profilers
{
	public class ConsoleProfiler : IDisposable,IProfiler
	{
		private readonly int _logOnlyResultsTakingLongerThanMilliseconds;
		private  MiniProfiler _profiler;
		private  MiniProfilerBaseOptions _options;


		public ConsoleProfiler(int logOnlyResultsTakingLongerThanMilliseconds=0)
		{
			_logOnlyResultsTakingLongerThanMilliseconds = logOnlyResultsTakingLongerThanMilliseconds;
			_options = new MiniProfilerBaseOptions() {Storage = new ConsoleStorage(_logOnlyResultsTakingLongerThanMilliseconds) };
			
			_profiler = new DefaultProfilerProvider().Start(null,_options);
		}
		public void Dispose()
		{
			_profiler.Stop();
		}

		public void Flush()
		{
			_options.Storage.Save(_profiler);

			_options = new MiniProfilerBaseOptions() { Storage = new ConsoleStorage(_logOnlyResultsTakingLongerThanMilliseconds) };

			_profiler = new DefaultProfilerProvider().Start(null, _options);
		}
		
		public IDisposable RecordStep(string stepId)
		{
			var recordStep = _profiler.Step(stepId);
			if (recordStep == null)
			{
				throw new ApplicationException("RecordStep: Could not obtain a valid profiler");
			}
			return recordStep;
		}
		public IDisposable ProfileTest(string commandId)
		{
			return Profile("Test", commandId);
		}
		public IDisposable Profile(string categoryId, string commandId)
		{
			var profile = _profiler.Step($"{categoryId}-{commandId}");
			if (profile == null)
			{
				throw new ApplicationException("Profile:Could not obtain a valid profiler");
			}
			return profile;
		}

		public class ConsoleStorage : IAsyncStorage
		{
			private int LogOnlyResultsTakingLongerThanMilliseconds{ get; }

			public ConsoleStorage(int logOnlyResultsTakingLongerThanMilliseconds)
			{
				LogOnlyResultsTakingLongerThanMilliseconds = logOnlyResultsTakingLongerThanMilliseconds;
			}


			public IEnumerable<Guid> List(int maxResults, DateTime? start = null, DateTime? finish = null,
				ListResultsOrder orderBy = ListResultsOrder.Descending)
			{
				throw new NotImplementedException();
			}

			public void Save(MiniProfiler profiler)
			{
				var logger = LogManager.GetLogger("Test");

				var stringBuilder = new StringBuilder(Environment.NewLine+$"Total Tree:".PadLeftExact(50,'>').PadRight(50,'<')+ Environment.NewLine);
				var totalMilliseconds = profiler.Root.Children.Sum(x=>x.DurationMilliseconds);
				RenderGroupedResults(stringBuilder
					, new GroupedTiming{
						Name = "Root",
						MaxMilliseconds = totalMilliseconds,
						MinMilliseconds = totalMilliseconds,
						TimesCalled = 1,
						TotalMilliseconds = totalMilliseconds,
						Children= profiler.Root.Children.ToArray()
					},1);



				logger.Info(() => stringBuilder.ToString());



				void RenderGroupedResults(StringBuilder sb, GroupedTiming item,int level)
				{
					if (item.TotalMilliseconds <= LogOnlyResultsTakingLongerThanMilliseconds) return;

					sb.AppendLine($"{string.Empty.PadLeft(level, '*')}[{level}]{item.ToString(level)}");
					var children = item.Children.GroupBy(x => x.Name)
						.Select(x =>
						{
							
							var timing = new GroupedTiming();
							timing.Children = x.Where(y=>y.HasChildren).SelectMany(y=>y.Children).ToArray();
							timing.Name = x.Key;
							timing.TotalMilliseconds = x.Select(y => y.DurationMilliseconds).Sum();
							timing.TimesCalled = x.Count();
							timing.MinMilliseconds = x.Min(y=>y.DurationMilliseconds);
							timing.MaxMilliseconds = x.Max(y => y.DurationMilliseconds);
							return timing;
						});
					foreach (var child in children)
					{
						RenderGroupedResults(sb,child,level+1);
					}
					
				}
				
			}

			private class GroupedTiming
			{
				public string Name { get; set; }
				public decimal? TotalMilliseconds { get; set; }
				public int TimesCalled { get; set; }
				public decimal? MinMilliseconds { get; set; }
				public decimal? MaxMilliseconds { get; set; }
				public Timing[] Children { get; set; }

				public string ToString(int charsPrefix)
				{
					return
						$"{Name.PadRightExact(90-charsPrefix, '-')}{TotalMilliseconds.ToString().PadRightExact(9, '-')} Times:{TimesCalled.ToString().PadLeftExact(2, '0').PadRightExact(4)}[{MinMilliseconds},{MaxMilliseconds}]";
				}
			}
			public Task SaveAsync(MiniProfiler profiler)
			{
				Save(profiler);
				return Task.CompletedTask;
			}

			public MiniProfiler Load(Guid id)
			{
				throw new NotImplementedException();
			}

			public void SetUnviewed(string user, Guid id)
			{
			}

			public void SetViewed(string user, Guid id)
			{
			}



			public List<Guid> GetUnviewedIds(string user)
			{
				throw new NotImplementedException();
			}

			public Task<IEnumerable<Guid>> ListAsync(int maxResults, DateTime? start = null, DateTime? finish = null,
				ListResultsOrder orderBy = ListResultsOrder.Descending)
			{
				throw new NotImplementedException();
			}



			public Task<MiniProfiler> LoadAsync(Guid id)
			{
				throw new NotImplementedException();
			}

			public Task SetUnviewedAsync(string user, Guid id)
			{
				throw new NotImplementedException();
			}

			public Task SetViewedAsync(string user, Guid id)
			{
				throw new NotImplementedException();
			}

			public Task<List<Guid>> GetUnviewedIdsAsync(string user)
			{
				throw new NotImplementedException();
			}
		}


		
	}
}