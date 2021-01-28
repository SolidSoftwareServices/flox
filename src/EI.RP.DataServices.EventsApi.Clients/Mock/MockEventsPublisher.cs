using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Serialization;
using NLog;

namespace EI.RP.DataServices.EventsApi.Clients.Mock
{
    class MockEventsPublisher: IEventApiEventPublisher
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public async Task Publish<TMessage>(TMessage eventToPublish) where TMessage : IEventApiMessage
        {
            //do nothing
            Logger.Info(()=>$"Published Event:{Environment.NewLine}{eventToPublish.ToJson()}");
        }

    }
}
