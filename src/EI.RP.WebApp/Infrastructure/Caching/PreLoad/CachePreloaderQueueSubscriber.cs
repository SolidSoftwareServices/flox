using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.System.DependencyInjection;
using EI.RP.WebApp.Infrastructure.Caching.PreLoad.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;

namespace EI.RP.WebApp.Infrastructure.Caching.PreLoad
{
	internal class CachePreloaderQueueSubscriber : BackgroundService
	{
		private static readonly ILogger Logger = LogManager.GetLogger("CacheLogger");
		private readonly CacheAccountPreLoaderQueue _queue;
		private readonly IServiceProvider _serviceProvider;
		private readonly ICacheSettings _settings;


		public CachePreloaderQueueSubscriber(CacheAccountPreLoaderQueue queue,
			IServiceProvider serviceProvider, ICacheSettings settings)
		{
			_queue = queue;
			_serviceProvider = serviceProvider;
			_settings = settings;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var tasks = new List<Task>();
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					tasks.RemoveAll(x => x.IsCompleted);
					tasks.AddRange(PreloadAccountDomainModels());
					await Task.Delay(TimeSpan.FromMilliseconds(50), stoppingToken);

				}
				catch (Exception ex)
				{
					Logger.Error(() => ex.ToString());
				}
			}

			if (tasks.Any())
			{
				await Task.WhenAll(tasks);
			}
		}

		private IEnumerable<Task> PreloadAccountDomainModels()
		{
			var tasks=new List<Task>();
			if (_settings.IsCachePreLoaderEnabled)
				while (!_queue.IsEmpty)
					if (_queue.TryDequeue(out var message))
					{
						tasks.Add(Task.Run(async () =>
						{
							try
							{
								await ProcessMessage(message);
							}
							catch (Exception ex)
							{
								Logger.Error(() => ex.ToString());
							}
						}));
					}

			return tasks;
			async Task ProcessMessage(CacheAccountPreLoaderQueue.CacheAccountPreLoadRequest message)
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var cachePreloadTasks = scope.ServiceProvider
						.Resolve<IEnumerable<ICachePreloadTask>>().OrderBy(x => x.PriorityOrder).ToArray();
					foreach (var preloadTask in cachePreloadTasks) await preloadTask.ProcessRequest(message);
				}
			}
		}
	}
}