using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;

namespace EI.RP.Stubs.CoreServices.Cqrs.Events
{
	public class FakeEventsPublisher : IEventApiEventPublisher
	{
		public Task Publish<TMessage>(TMessage eventToPublish) where TMessage : IEventApiMessage
		{
			return Task.CompletedTask;
		}
	}
}