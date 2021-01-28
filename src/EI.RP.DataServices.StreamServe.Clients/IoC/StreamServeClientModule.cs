using Autofac;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.IoC.Autofac;

namespace EI.RP.DataServices.StreamServe.Clients.IoC
{
	public class StreamServeClientModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly).AsSelf();

			builder.Register(c=>
			{
				var generalSettings = c.Resolve<IAzureGeneralSettings>();
				var result = generalSettings.IsAzureEnabled
					? (IStreamServeRepository) c.Resolve<AzureEnabledRepository>()
					: c.Resolve<LegacyRepository>();
				return result;
			}).AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterType<BillImagesRepository>().AsImplementedInterfaces().WithInterfaceProfiling();
		}
	}
}