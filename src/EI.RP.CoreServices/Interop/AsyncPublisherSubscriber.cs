using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.System.DesignPatterns.Observer;
using NLog;

namespace EI.RP.CoreServices.Interop
{
	public class AsyncPublisherSubscriber<TMessage>:IDisposable,IAsyncObservable<TMessage>,IAsyncObservable<IEnumerable<TMessage>>
	where TMessage:IObservableMessage
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly ConcurrentQueue<TMessage> _queue = new ConcurrentQueue<TMessage>();
		private readonly ManualResetEvent _signalEvent=new ManualResetEvent(false);
		private readonly Task _notificationWorker;
		private readonly CancellationTokenSource _cts=new CancellationTokenSource();
		public AsyncPublisherSubscriber()
		{
			_notificationWorker = Task.Factory.StartNew(async() => await NotificationWorkerPayload(_cts.Token),
				TaskCreationOptions.LongRunning);
		}
		public void Publish(IEnumerable<TMessage> messages)
		{
			
			foreach (var message in messages)
			{
				_PublishSingle(message);
			}
		}
		public void Publish(TMessage message)
		{

			_PublishSingle(message);
		}

		private void _PublishSingle(TMessage message)
		{
			_queue.Enqueue(message);

			_signalEvent.Set();
		}

		public int PendingMessages => _queue.Count;

		private async Task NotificationWorkerPayload(CancellationToken ctsToken)
		{
			
			while (!ctsToken.IsCancellationRequested)
			{
				_signalEvent.WaitOne(TimeSpan.FromSeconds(1));
				_signalEvent.Reset();
				var batchRun = Guid.NewGuid();
				try
				{
					var messages = new List<TMessage>();
					while (_queue.TryDequeue(out TMessage msg))
					{
						msg.ReceptionBatchId = batchRun;
						messages.Add(msg);
					}

					if (messages.Any())
					{
						var notificationTasks=new List<Task>();
						foreach (var batchObserver in _batchObservers)
						{
							notificationTasks.Add(batchObserver.OnNextAsync(messages));
						}

						foreach (var message in messages)
						{
							foreach (var observer in _singleObservers)
							{
								notificationTasks.Add(
									observer.OnNextAsync(message));
							}
						}

						await Task.WhenAll(notificationTasks);
					}
				}
				catch (Exception ex)
				{
					Logger.Error(() => ex.ToString());
				}
				
			}
		}


		public void Dispose()
		{
			_cts.Cancel(throwOnFirstException:false);
			_notificationWorker.Wait();
			
			_signalEvent?.Dispose();
		}
		private readonly List<IAsyncObserver<TMessage>> _singleObservers=new List<IAsyncObserver<TMessage>>();

		
		public IDisposable Subscribe(IAsyncObserver<TMessage> observer)
		{
			if(_singleObservers.Contains(observer)) throw  new InvalidOperationException("observer already subscribed");
			_singleObservers.Add(observer);
			return new Unsubscriber<IAsyncObserver<TMessage>>(_singleObservers, observer);
		}
		private readonly List<IAsyncObserver<IEnumerable<TMessage>>> _batchObservers=new List<IAsyncObserver<IEnumerable<TMessage>>>();
		public IDisposable Subscribe(IAsyncObserver<IEnumerable<TMessage>> batchObserver)
		{
			if(_batchObservers.Contains(batchObserver)) throw  new InvalidOperationException("observer already subscribed");
			_batchObservers.Add(batchObserver);
			return new Unsubscriber<IAsyncObserver<IEnumerable<TMessage>>>(_batchObservers, batchObserver);
		}
		private class Unsubscriber<TObserver> : IDisposable
		{
			private readonly List<TObserver>_observers;
			private readonly TObserver _observer;

			public Unsubscriber(List<TObserver> observers, TObserver observer)
			{
				this._observers = observers;
				this._observer = observer;
			}

			public void Dispose()
			{
				if (_observer != null && _observers.Contains(_observer))
					_observers.Remove(_observer);
			}
		}


		
	}
}
