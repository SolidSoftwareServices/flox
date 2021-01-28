using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S3.CoreServices.System
{
	public static class DictionaryExtensions
	{
		public static bool AddSafe<TKey,TValue>(this IDictionary<TKey, TValue>src, TKey key, TValue value,bool overrideIfExists=false)
		{
			if (src.ContainsKey(key))
			{
				if (!overrideIfExists)
				{
					return false;
				}

				src[key] = value;
			}
			else
			{
				src.Add(key, value);
			}

			return true;
		}

		public static string GetOrDefault(this IDictionary<string, object> src, string key,string defaultValue=null)
		{
			return src.Keys.Any(x => x.Equals(key, StringComparison.InvariantCultureIgnoreCase))?src[key]?.ToString() : defaultValue;
		}
		public static TValue GetOrAdd<TKey,TValue>(this IDictionary<TKey, TValue> src, TKey key, Func<TValue> initialValueResolver)
		{
			if(!src.ContainsKey(key))src.Add(key,initialValueResolver());
			return src[key];
		}
		public static IDictionary<TKey, TValue> RemoveIfExists<TKey, TValue>(this IDictionary<TKey, TValue> src, TKey key)
		{
			if (src.ContainsKey(key)) src.Remove(key);
			return src;
		}

		public static bool IsEqualTo<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> otherDictionary)
		{
			return (otherDictionary ?? new Dictionary<TKey, TValue>())
				.OrderBy(kvp => kvp.Key)
				.SequenceEqual((dictionary ?? new Dictionary<TKey, TValue>())
					.OrderBy(kvp => kvp.Key));
		}

		private static readonly SemaphoreSlim GetOrAddAsyncSem=new SemaphoreSlim(1,1);
		public static async Task<TValue> GetOrAddAsync<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary,TKey key,
			Func<Task<TValue>> valueFactory,CancellationToken cancellationToken=default(CancellationToken))
		{
			TValue result = default(TValue);
			if (!dictionary.TryGetValue(key, out result))
			{
				await GetOrAddAsyncSem.WaitAsync(cancellationToken);
				try
				{
					if (!dictionary.TryGetValue(key, out result))
					{
						result=  await valueFactory();
						dictionary[key] = result;
					}
				}
				finally
				{
					GetOrAddAsyncSem.Release();
				}
			}

			return result;
		}
	}
}