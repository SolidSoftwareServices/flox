using System;
using EI.RP.CoreServices.Cqrs.Events;

namespace EI.RP.DataServices.EventsApi.Clients.Config
{
    class EventRelativeUrlResolver : IEventRelativeUrlResolver
    {
        public string ResolveFor<TMessage>(TMessage message) where TMessage : IEventApiMessage
        {
            if (message is EventApiEvent)
            {
                return "api/events/add";
            }
            throw new NotSupportedException($"The relative url resolution has not been implemented for {typeof(TMessage).FullName}");
        }
    }
}