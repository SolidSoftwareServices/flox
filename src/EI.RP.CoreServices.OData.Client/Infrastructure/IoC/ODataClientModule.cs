using Autofac;
using EI.RP.CoreServices.IoC.Autofac;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.IoC
{
	public class ODataClientModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces().WithInterfaceProfiling();
		}
	}
}