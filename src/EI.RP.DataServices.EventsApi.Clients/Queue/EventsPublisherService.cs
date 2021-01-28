using System;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Profiling;
using EI.RP.DataServices.EventsApi.Clients.ApiAdapter;
using EI.RP.DataServices.EventsApi.Clients.Config;
using Microsoft.Extensions.Hosting;
using NLog;

namespace EI.RP.DataServices.EventsApi.Clients.Queue
{
	class EventsPublisherService : BackgroundService
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private readonly IEventsPublisherSettings _settings;
		private readonly IApiEventAdapter _apiAdapter;
		private readonly EventsQueue _queue;
		private readonly IProfiler _profiler;

		public EventsPublisherService(IEventsPublisherSettings settings, IApiEventAdapter apiAdapter, EventsQueue queue,IProfiler profiler)
		{
			_settings = settings;
			_apiAdapter = apiAdapter;
			_queue = queue;
			_profiler = profiler;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Logger.Info(()=>$"{nameof(EventsPublisherService)} is starting.");

			stoppingToken.Register(() =>
				Logger.Info(()=>$" {nameof(EventsPublisherService)} is stopping."));

			while (!stoppingToken.IsCancellationRequested)
			{
				
				await SendPendingMessages();

				await Task.Delay(_settings.CheckForPendingEventsInterval, stoppingToken);
			}
			await SendPendingMessages();
			Logger.Info(()=>$" {nameof(EventsPublisherService)} is stopping.");
		}

		private async Task SendPendingMessages()
		{
			while (!_queue.IsEmpty)
			{
				if (_queue.TryDequeue(out var message))
				{
					try
					{
						using (_profiler.Profile("Events Publishing", "SendPendingMessage"))
						{
							await _apiAdapter.Publish(message);
							Logger.Debug(()=>$"Published event: {message}");
						}
					}
					catch (Exception ex)
					{
						Logger.Error(()=>$"Error sending event message: {message}. {ex}");
					}
				}
			}
		}
	}
}