using Autofac;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.System;

namespace EI.RP.WebApp.Infrastructure.HealthChecks
{
	class HealthChecksModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly)
				.Where(x => x.Implements(typeof(IResidentialPortalHealthCheck))).AsImplementedInterfaces().WithInterfaceProfiling();
		}
	}
}