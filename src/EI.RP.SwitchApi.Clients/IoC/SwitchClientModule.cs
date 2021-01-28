using Autofac;
using EI.RP.CoreServices.IoC.Autofac;

namespace EI.RP.SwitchApi.Clients.IoC
{
	public class SwitchClientModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces().WithInterfaceProfiling();
        }
    }
}