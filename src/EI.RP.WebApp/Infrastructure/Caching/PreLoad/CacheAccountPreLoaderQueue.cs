using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;

namespace EI.RP.WebApp.Infrastructure.Caching.PreLoad
{
	public class CacheAccountPreLoaderQueue : ICacheAccountPreLoaderQueue<CacheAccountPreLoaderQueue.CacheAccountPreLoadRequest>
	{
		private static readonly ConcurrentQueue<CacheAccountPreLoadRequest> _queue=new ConcurrentQueue<CacheAccountPreLoadRequest>();
		private readonly ICacheSettings _settings;

		public CacheAccountPreLoaderQueue(ICacheSettings settings)
		{
			_settings = settings;
		}

		public bool IsEmpty => _queue.IsEmpty;

		public async Task SubmitRequestAsync(string forUserName)
		{
			if (_settings.IsCachePreLoaderEnabled  && _settings.CacheProviderType != CacheProviderType.NoCache)
			{
				var request = new CacheAccountPreLoadRequest();
				
				request.ForUserName = forUserName;
				
				_queue.Enqueue(request);
			}

			
		}
		public class CacheAccountPreLoadRequest
		{
			public DateTime CreatedUtc { get; } = DateTime.UtcNow;
			public string ForUserName { get; set; }

		}
		

		public bool TryDequeue(out CacheAccountPreLoadRequest request)
		{
			return _queue.TryDequeue(out request);
		}
	}
}