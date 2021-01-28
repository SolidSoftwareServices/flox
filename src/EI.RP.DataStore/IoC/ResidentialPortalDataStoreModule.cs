using Autofac;
using EI.RP.CoreServices.IoC.Autofac;

namespace EI.RP.DataStore.IoC
{
	public class ResidentialPortalDataStoreModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces().WithInterfaceProfiling();
		}
	}
}