using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching.Models;
using EI.RP.CoreServices.Interop;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.DesignPatterns.Observer;
using StackExchange.Redis;

namespace EI.RP.CoreServices.Caching.Redis
{
	partial class RedisRemoteCacheFacade
	{
		class RedisEventMessage:IObservableMessage		
		{

			public static RedisEventMessage Parse(ChannelMessage message, string instanceName)
			{
				var channel = message.Channel.ToString();
				var keyWithInstance = channel.Substring(channel.IndexOf(':') + 1);
				var key = keyWithInstance?.Replace(instanceName, string.Empty);
				var eventName = message.Message.ToString();

				bool isFromTheSameApp;
				isFromTheSameApp = keyWithInstance.Contains(instanceName);
				return new RedisEventMessage(key, eventName, isFromTheSameApp);
			}

			private RedisEventMessage(string key, string eventName, bool isFromTheSameApp)
			{
				Key = key;
				EventName = eventName;
				IsFromTheSameApp = isFromTheSameApp;
			}

			public string Key { get; }
			public string EventName { get; }
			public bool IsFromTheSameApp { get; }


			public override string ToString()
			{
				return
					$"[ {nameof(EventName)}: {EventName}, {nameof(IsFromTheSameApp)}: {IsFromTheSameApp}, {nameof(Key)}: {Key}]";
			}

			public bool IsEventSupported()
			{
				return EventName.IsOneOf("expired", "del", "evicted", "hset");
			}

			public Guid ReceptionBatchId
			{
				get;
				set;
			}
		}

		private class CacheEventsPublisher : IAsyncObserver<RedisEventMessage>
		{
			private readonly RedisRemoteCacheFacade _facade;

			public CacheEventsPublisher(RedisRemoteCacheFacade facade)
			{
				_facade = facade;
			}


			public Task OnCompletedAsync()
			{
				throw new NotImplementedException();
			}

			public Task OnErrorAsync(Exception error)
			{
				throw new NotImplementedException();
			}


			private readonly ConcurrentHashSet<RedisEventMessage> _processedBatchMessages=new ConcurrentHashSet<RedisEventMessage>();
			public async Task OnNextAsync(RedisEventMessage eventMessage)
			{

				try
				{
					if (!_processedBatchMessages.Any(x => x.ReceptionBatchId == eventMessage.ReceptionBatchId))
					{
						_processedBatchMessages.Clear();
					}
					
					if (eventMessage.IsFromTheSameApp)
					{
						bool processed = false;
						Logger.Trace(() => $"Redis:Handling remote redis event: {eventMessage} STARTED");
						switch (eventMessage.EventName)
						{
							case "expired":
							case "del":
							case "evicted":
								_processedBatchMessages.Clear();
								await (_facade.RemoteKeyDeletedReceivedFromRedis?.Invoke(this,
									eventMessage.Key.ToOneItemArray()) ?? Task.CompletedTask);
								processed = true;
								break;
							case "hset":
								if (_processedBatchMessages.AddIfNotExists(eventMessage))
								{
									await (_facade.RemoteKeyValueSetReceivedFromRedis?.Invoke(this,
										eventMessage.Key.ToOneItemArray()) ?? Task.CompletedTask);
									processed = true;
								}

								break;
							default:
								Logger.Trace(() => $"Redis: {eventMessage.EventName} has not handler.");
								break;
						}

						if (processed)
						{
							Logger.Debug(() => $"Redis:Handling remote redis event: {eventMessage} COMPLETED");
						}

					}

				}
				catch (Exception ex)
				{
					Logger.Warn(() => $"Could not handle {eventMessage} - {ex}");
				}

			}
		}

		private AsyncPublisherSubscriber<RedisEventMessage> _cacheEventsQueue;

		private async Task InitiateSubscriber()
		{
			_cacheEventsQueue?.Dispose();
			
			_cacheEventsQueue = new AsyncPublisherSubscriber<RedisEventMessage>();
			_disposables.Add(_cacheEventsQueue.Subscribe(new CacheEventsPublisher(this)));

			var getInstanceName = GetInstanceName();
			var connection = await _redisServerProvider.GetConnectionAsync();
			_redisEventsSubscriber = connection.GetSubscriber();
			await _redisEventsSubscriber.UnsubscribeAllAsync();
			var instanceName = await getInstanceName;
			var redisChannel = (RedisChannel)$"__keyspace@*__:*{CacheKeyScope.EnvironmentKeyPart()}*{CacheKeyScope.VersionKeyPart}*";
			var queue = await _redisEventsSubscriber.SubscribeAsync(redisChannel);
			queue.OnMessage(ReceivedEventsHandler);

			async Task ReceivedEventsHandler(ChannelMessage message)
			{
				try
				{
					var parsedMessage = RedisEventMessage.Parse(message, instanceName);
					if (parsedMessage.IsFromTheSameApp && parsedMessage.IsEventSupported())
					{
						_cacheEventsQueue.Publish(parsedMessage);
					}

				}
				catch (Exception ex)
				{
					Logger.Error(() => ex.ToString());
					throw;
				}
			}
		}
	}
}