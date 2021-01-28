using Autofac;
using EI.RP.CoreServices.Azure.Infrastructure.Authx.Credentials;
using EI.RP.CoreServices.Azure.Secrets;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.System;

namespace EI.RP.CoreServices.Azure.Infrastructure.IoC
{
	public class AzureModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly).Where(x=>!x.Implements(typeof(IAzureCredentialsResolverStrategy))
			                                                           )
				.AsImplementedInterfaces();
			
			builder.RegisterAssemblyTypes(GetType().Assembly)
				.Where(x => x.Implements(typeof(IAzureCredentialsResolverStrategy)))
				.AsImplementedInterfaces()
				.WithInterfaceProfiling()
				.InstancePerLifetimeScope();

		}
	}
}