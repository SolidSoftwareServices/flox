using System;
using System.Threading.Tasks;
using S3.AspnetCore.SessionServices.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace S3.AspnetCore.SessionServices.TicketStores
{
	public class RedisTicketStore : ITicketStore
	{
		private readonly IWebSessionSettings _settings;
		private const string KeyPrefix = "AuthSessionStore-";
		private readonly IDistributedCache _cache;

		public RedisTicketStore(RedisCacheOptions options,IWebSessionSettings settings)
		{
			_settings = settings;
			_cache = new RedisCache(options);
		}

		public async Task<string> StoreAsync(AuthenticationTicket ticket)
		{
			ThrowIfInvalidStorage();
			var key = $"{KeyPrefix}{Guid.NewGuid()}";
			await RenewAsync(key, ticket);
			return key;
		}

		public async Task RenewAsync(string key, AuthenticationTicket ticket)
		{
			ThrowIfInvalidStorage();
			var expiresUtc = ticket.Properties.ExpiresUtc;
			var options = new DistributedCacheEntryOptions();
			if (expiresUtc.HasValue)
			{
				options.SetAbsoluteExpiration(expiresUtc.Value);
			}
			var val = TicketSerializer.Default.Serialize(ticket);
			await _cache.SetAsync(key, val, options);
		}

		public async Task<AuthenticationTicket> RetrieveAsync(string key)
		{
			AuthenticationTicket ticket=null;
			byte[] bytes = null;
			bytes = await _cache.GetAsync(key);
			if (bytes != null)
			{
				ticket = TicketSerializer.Default.Deserialize(bytes);
			}
			return ticket;
		}

		public async Task RemoveAsync(string key)
		{
			await _cache.RemoveAsync(key);
		}
		private void ThrowIfInvalidStorage()
		{
			if (_settings.SessionStorage != SessionStorageType.Redis)
				throw new InvalidOperationException(
					$"Only {nameof(SessionStorageType)}.{nameof(SessionStorageType.InMemory)} supported.");
		}
	}
}