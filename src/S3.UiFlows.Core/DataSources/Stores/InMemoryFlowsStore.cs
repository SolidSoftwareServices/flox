using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3.UiFlows.Core.DataSources.Stores
{
	class InMemoryFlowsStore : IFlowsStore
	{
		private readonly ConcurrentDictionary<string, string> _store = new ConcurrentDictionary<string, string>();

		public Task<IEnumerable<string>> GetValuesAsync ()=> Task.FromResult((IEnumerable<string>)_store.Values);
		public Task<string> GetAsync(string key)
		{
			return Task.FromResult(_store.ContainsKey(key) ? _store[key] : null);
		}

		public Task SetAsync(string key, string value)
		{
			_store.AddOrUpdate(key, a => value, (a, b) => value);
			return Task.CompletedTask;
		}

		public Task RemoveAsync(string key)
		{
			_store.TryRemove(key, out var json);
			return Task.CompletedTask;
		}

		public Task ClearAsync()
		{
			_store.Clear();
			return Task.CompletedTask;
		}
	}
}