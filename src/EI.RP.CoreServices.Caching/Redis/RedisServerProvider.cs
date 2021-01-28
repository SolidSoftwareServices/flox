using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.Async;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;

namespace EI.RP.CoreServices.Caching.Redis
{
	internal  class RedisServerProvider:IRedisServerProvider
	{
		private  AsyncLazy<RedisCacheOptions> Options { get; }
		private IServer _server;
		private ConnectionMultiplexer _connection;
		private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
		public RedisServerProvider(RedisCacheOptions options):this(new AsyncLazy<RedisCacheOptions>(options)){}
		
		public RedisServerProvider(AsyncLazy<RedisCacheOptions> options)
		{
			Options = options;
		}

		public  async Task<IServer> GetServerAsync(CancellationToken token = default(CancellationToken))
		{
			token.ThrowIfCancellationRequested();
			await GetConnectionAsync(token);
			if (_server == null)
			{
				_server = await _connectionLock.AsyncCriticalSection(async () =>
				{
					var server = _server;
					if (server == null)
					{
						var endPoint = _connection.GetEndPoints().FirstOrDefault();
						if (endPoint == null)
						{
							throw new ArgumentException("Could not get an end point from Redis.");
						}

						server = _connection.GetServer(endPoint);
					}

					return server;

				});
			}

			return _server;
		}
		public  async Task<ConnectionMultiplexer> GetConnectionAsync(CancellationToken token = default(CancellationToken))
		{
			token.ThrowIfCancellationRequested();

			if (_connection == null)
			{
				_connection = await _connectionLock.AsyncCriticalSection(async () =>
				{
					var connection = _connection;
					if (connection == null)
					{
						var redisCacheOptions = await Options.Value;
						connection = await ConnectionMultiplexer.ConnectAsync(redisCacheOptions.Configuration);
					}

					return connection;

				});
			}

			return _connection;
		}

		public ConnectionMultiplexer GetConnection()
		{
			return GetConnectionAsync().GetAwaiter().GetResult();
		}

		public void Dispose()
		{
			_connectionLock?.Dispose();
			_connection?.Dispose();
			
		}
	}
}