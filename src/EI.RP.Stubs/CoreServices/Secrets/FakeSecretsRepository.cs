using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NotImplementedException = System.NotImplementedException;

namespace EI.RP.CoreServices.Secrets
{
	public class FakeSecretsRepository : ISecretsRepository
	{
		private readonly bool _failIfNotFound;
		private static readonly ConcurrentDictionary<string,string> Data=new ConcurrentDictionary<string, string>();

		public FakeSecretsRepository(bool failIfNotFound=true)
		{
			_failIfNotFound = failIfNotFound;
		}

		public bool Contains(string key)
		{
			return Data.ContainsKey(key);
		}

		public Task<bool> ContainsAsync(string key)
		{
			return Task.FromResult(Contains(key));
		}

		

		public Task<string> GetAsync(string key, int maxAttempts, bool cacheResults = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult(Get(key));
		}

		public string Get(string key)
		{
			string value;
			if(! Data.TryGetValue(key,out value))
			{
				if (_failIfNotFound)
				{
					throw new KeyNotFoundException();
				}
				value= string.Empty;
			}

			return value;
		}

		public Task SetAsync(string key, string value)
		{
			Set(key,value);
			return Task.CompletedTask;
		}

		public void Set(string key, string value)
		{
			
			Data.AddOrUpdate(key,value,(k,v)=>value);
		}

		public Task RemoveAsync(string key)
		{
			Remove(key);
			return Task.CompletedTask;
		}

		public void Remove(string key)
		{
			
			if(!Data.TryRemove(key, out string v))
			{
				if (_failIfNotFound)
				{
					throw new KeyNotFoundException();
				}
			}
		}
	}
}