using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Interop;
using EI.RP.CoreServices.Resiliency;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.DesignPatterns.Observer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace EI.RP.CoreServices.Caching.Redis
{
	partial class RedisRemoteCacheFacade
	{
		private AsyncPublisherSubscriber<CacheCommand> _cacheCommandsQueue;

		private async Task InitiatePublisher()
		{
			_cacheCommandsQueue?.Dispose();
			_cacheCommandsQueue = new AsyncPublisherSubscriber<CacheCommand>();
			_disposables.Add(
				_cacheCommandsQueue.Subscribe(new CacheCommandPublisher(await this._redisCache.Value, this)));
		}

		private class CacheCommandPublisher : IAsyncObserver<IEnumerable<CacheCommand>>
		{
			private readonly RedisCache _redisCache;
			private readonly RedisRemoteCacheFacade _redisRemoteCacheFacade;

			public CacheCommandPublisher(RedisCache redisCache, RedisRemoteCacheFacade redisRemoteCacheFacade)
			{
				_redisCache = redisCache;
				_redisRemoteCacheFacade = redisRemoteCacheFacade;
			}


			public Task OnCompletedAsync()
			{
				throw new NotImplementedException();
			}

			public Task OnErrorAsync(Exception error)
			{
				throw new NotImplementedException();
			}

			public async Task OnNextAsync(IEnumerable<CacheCommand> commands)
			{
				
				await Task.WhenAll(commands.Select(ExecuteCommand));
				async Task ExecuteCommand(CacheCommand command)
				{
					Func<Task> payload;
					switch (command.Operation)
					{
						case CacheCommand.CacheOperation.Set:
							payload = async () =>
							{
								await ResilientOperations.Default.RetryIfNeeded(async()=>
								{
									await _redisCache.SetAsync(command.Key, command.Bytes, command.Options);
								});
								Logger.Debug(() => $"Redis SET {command.Key}");

								await (_redisRemoteCacheFacade.SetRemoteKeyValueWasSentToRedis?.Invoke(this,
									command.Key.ToOneItemArray())??Task.CompletedTask);
							};
							break;
						case CacheCommand.CacheOperation.Remove:
							payload =async () =>
							{
								await ResilientOperations.Default.RetryIfNeeded(async()=>
								{
									await _redisCache.RemoveAsync(command.Key);
								});
								Logger.Debug(() => $"Redis REMOVE {command.Key}");

								await (_redisRemoteCacheFacade.DeleteRemoteKeyWasSentToRedis?.Invoke(this,
									command.Key.ToOneItemArray())??Task.CompletedTask);
							};
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}

					try
					{
						await payload();
					}
					catch (Exception ex)
					{
						Logger.Warn(() => $"{command} - {ex}");
					}
				}
			}
		}
		public Task RemoveAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
		{
			_cacheCommandsQueue.Publish(new CacheCommand(CacheCommand.CacheOperation.Remove,key));
			return Task.CompletedTask;
		}

		public Task SetAsync(string key, byte[] bytes, DistributedCacheEntryOptions options)
		{
			_cacheCommandsQueue.Publish(new CacheCommand(CacheCommand.CacheOperation.Set,key,bytes,options));
			return Task.CompletedTask;
		}

		private class CacheCommand:IObservableMessage
		{
			public string Key { get; }
			public byte[] Bytes { get; }
			public DistributedCacheEntryOptions Options { get; }
			public  CacheOperation Operation { get; }

			public CacheCommand(CacheOperation cacheOperation, string key, byte[] bytes = null,
				DistributedCacheEntryOptions distributedCacheEntryOptions=null)
			{
				if (cacheOperation==CacheOperation.Set && bytes == null)
				{
					throw new ArgumentNullException(nameof(bytes));
				}

				Key = key;
				Bytes = bytes;
				Options = distributedCacheEntryOptions;

				Operation = cacheOperation;
			}

			public enum CacheOperation
			{
				Set,Remove
			}

			public Guid ReceptionBatchId
			{
				get;
				set;
			}
		}

		
	}
}