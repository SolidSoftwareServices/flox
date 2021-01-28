using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Authx;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.Async;
using EI.RP.DataServices.EventsApi.Clients.Config;

namespace EI.RP.DataServices.EventsApi.Clients.ApiAdapter
{
	class EventsPublisherApiAdapter : IApiEventAdapter, IDisposable
	{
		private readonly IEventRelativeUrlResolver _urlResolver;
		private readonly AsyncLazy<JsonApiClient> _client;

		public EventsPublisherApiAdapter(IEventsPublisherSettings settings, IEventRelativeUrlResolver urlResolver,IProfiler profiler,IBearerTokenProvider bearerTokenProvider,IHttpClientBuilder httpClientBuilder)
		{
			_urlResolver = urlResolver;
			_client = new AsyncLazy<JsonApiClient>(async () => new JsonApiClient(httpClientBuilder,settings.EvenLogApiUrlPrefix, profiler,
				bearerTokenProvider, await settings.EventsBearerTokenProviderUrlAsync()));
		}

		public async Task Publish<TMessage>(TMessage eventToPublish) where TMessage : IEventApiMessage
		{
			var clientValue = await _client.Value;
			await clientValue.PostJsonAsync(_urlResolver.ResolveFor(eventToPublish), eventToPublish);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _client.IsValueCreated)
			{
				_client?.Value.GetAwaiter().GetResult().Dispose();
			}
		}

		~EventsPublisherApiAdapter()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}