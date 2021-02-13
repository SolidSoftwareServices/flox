using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace S3.CoreServices.System
{
	public class ReversedIndex
	{
		private readonly ConcurrentDictionary<string, ConcurrentHashSet<string>> _index =
			new ConcurrentDictionary<string, ConcurrentHashSet<string>>();

		public void EnsureEntryExists(string itemId, string key)
		{
			var keys= _index.GetOrAdd(itemId, (k) => new ConcurrentHashSet<string>());
			keys.AddIfNotExists(key);
		}

		public void RemoveKey(string key)
		{
			foreach (var hashSet in _index.Values)
			{
				hashSet.Remove(key);
			}

			var emptyEntries = _index.Where(x=>x.Value.Count==0).Select(x=>x.Key).ToArray();
			foreach (var itemId in emptyEntries)
			{
				_index.TryRemove(itemId, out var nil);
			}
		}

		public IEnumerable<string> GetKeys(string itemId)
		{
			return  _index.ContainsKey(itemId)
				? _index[itemId]
				:(IEnumerable<string>)new string[0];
		}
	}
}