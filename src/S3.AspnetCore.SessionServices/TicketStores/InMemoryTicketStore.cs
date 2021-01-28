using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using S3.AspnetCore.SessionServices.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NLog;

namespace S3.AspnetCore.SessionServices.TicketStores
{
	/// <summary>
	/// Valid only when using sticky sessions
	/// </summary>
	class InMemoryTicketStore : ITicketStore
	{
		private readonly IWebSessionSettings _settings;
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private readonly ConcurrentDictionary<string, AuthenticationTicket> _data =
			new ConcurrentDictionary<string, AuthenticationTicket>();

		public InMemoryTicketStore(IWebSessionSettings settings)
		{
			_settings = settings;
		}

		public Task<string> StoreAsync(AuthenticationTicket ticket)
		{
			ThrowIfInvalidStorage();
			var key = Guid.NewGuid().ToString();
			if (!_data.TryAdd(key, ticket))
			{
				//this can  only happen if 2 guids are created in the same time window. impossible:https://blog.stephencleary.com/2010/11/few-words-on-guids.html#:~:text=Likelihood%20of%20Collision&text=Assuming%20a%20perfect%20source%20of,That's%20a%20lot.
				throw new InvalidProgramException("Could not store session");
			}

			Logger.Info(() => $"{ticket.Principal.Identity.Name} started new session {key}");
			return Task.FromResult(key);
		}

		private void ThrowIfInvalidStorage()
		{
			if (_settings.SessionStorage != SessionStorageType.InMemory)
				throw new InvalidOperationException(
					$"Only {nameof(SessionStorageType)}.{nameof(SessionStorageType.InMemory)} supported.");
		}

		public Task RenewAsync(string key, AuthenticationTicket ticket)
		{
			ThrowIfInvalidStorage();
			_data.AddOrUpdate(key, ticket, (k, t) => ticket);
			Logger.Info(() => $"Renewed session {key} of {ticket.Principal.Identity.Name}");
			return Task.CompletedTask;
		}

		public Task<AuthenticationTicket> RetrieveAsync(string key)
		{
			ThrowIfInvalidStorage();
			if (!_data.TryGetValue(key, out var result))
			{
				result = null;
			}

			return Task.FromResult(result);
		}

		public Task RemoveAsync(string key)
		{
			ThrowIfInvalidStorage();
			if (_data.TryRemove(key, value: out var ticket))
			{
				Logger.Info(() => $"Closed session {key} of {ticket.Principal.Identity.Name}");
			}

			return Task.CompletedTask;
		}
	}
}
