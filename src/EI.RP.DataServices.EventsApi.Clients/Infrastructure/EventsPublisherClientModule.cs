using System;
using Autofac;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.DataServices.EventsApi.Clients.Config;
using EI.RP.DataServices.EventsApi.Clients.Mock;
using EI.RP.DataServices.EventsApi.Clients.Queue;
using Microsoft.Extensions.Hosting;

namespace EI.RP.DataServices.EventsApi.Clients.Infrastructure
{
	public class EventsPublisherClientModule : BaseModule
    {
      

        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces().WithInterfaceProfiling();

			

            builder.RegisterType<MockEventsPublisher>().AsSelf();
            builder.RegisterType<EventsQueue>().AsSelf().SingleInstance();
            builder.Register((ctx) =>
            {
                var settings = ctx.Resolve<IEventsPublisherSettings>();
                var sapRepository = settings.UseMockEventsPublisher
                    ? ctx.Resolve<MockEventsPublisher>()
                    : (IEventApiEventPublisher) ctx.Resolve<EventsQueue>();
                return sapRepository;
            }).As<IEventApiEventPublisher>().WithInterfaceProfiling();

            builder.RegisterType<EventsPublisherService>().As<IHostedService>().InstancePerDependency().WithClassProfiling();


        }
    }
}
