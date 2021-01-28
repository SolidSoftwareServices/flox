using EI.RP.CoreServices.Cqrs.Events;

namespace EI.RP.DataServices.EventsApi.Clients.Config
{
    interface IEventRelativeUrlResolver
    {
        string ResolveFor<TMessage>(TMessage message) where TMessage : IEventApiMessage;
    }
}