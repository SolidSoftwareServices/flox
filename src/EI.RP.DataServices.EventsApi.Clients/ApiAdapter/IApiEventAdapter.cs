using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;

namespace EI.RP.DataServices.EventsApi.Clients.ApiAdapter
{
	interface IApiEventAdapter
	{
		Task Publish<TMessage>(TMessage eventToPublish) where TMessage : IEventApiMessage;
	}
}