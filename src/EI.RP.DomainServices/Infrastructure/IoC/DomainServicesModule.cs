using Autofac;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.DomainServices.Commands.BusinessPartner.Deregister;

namespace EI.RP.DomainServices.Infrastructure.IoC
{
    public class DomainServicesModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces().WithInterfaceProfiling();

			builder.RegisterType<DeRegisterBusinessPartnerCommandHandler>().AsImplementedInterfaces()
				.InstancePerLifetimeScope().WithInterfaceProfiling();
		}
	}
}