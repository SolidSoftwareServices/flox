using Autofac;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.Ports.OData;

namespace EI.RP.DataServices.SAP.Clients.Infrastructure.IoC
{
	public class SapRepositoriesModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly).Where(x=>!x.IsAssignableTo<IODataRepository>())
				.AsSelf()
				.AsImplementedInterfaces();

			//instances are trackable per lifetimescope
			builder.RegisterAssemblyTypes(GetType().Assembly)
				.Where(x => x.IsAssignableTo<IODataRepository>()).AsImplementedInterfaces().InstancePerLifetimeScope().WithInterfaceProfiling();
		}
	}
}