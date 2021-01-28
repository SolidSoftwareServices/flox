using System.Collections.Concurrent;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;

namespace EI.RP.DataServices.EventsApi.Clients.Queue
{

	class EventsQueue : ConcurrentQueue<IEventApiMessage>, IEventApiEventPublisher
	{
		public Task Publish<TMessage>(TMessage eventToPublish) where TMessage : IEventApiMessage
		{
			this.Enqueue(eventToPublish);
			return Task.CompletedTask;
		}
	}
}

	 
