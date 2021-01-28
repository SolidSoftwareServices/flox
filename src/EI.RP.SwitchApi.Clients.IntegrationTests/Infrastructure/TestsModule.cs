using System;
using Autofac;
using AutoFac.Extras.NLog.DotNetCore;
using EI.RP.CoreServices.Azure.Infrastructure.IoC;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Infrastructure;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.Secrets;
using EI.RP.DataStore.IoC;
using Ei.Rp.Mvc.Core.Profiler;
using EI.RP.SwitchApi.Clients.IoC;

namespace EI.RP.SwitchApi.Clients.IntegrationTests.Infrastructure
{
	internal class TestsModule : BaseModule
	{
		private readonly string _environmentName;

		public TestsModule(string environmentName)
		{
			_environmentName = environmentName;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule<NLogModule>();
			builder.RegisterModule<AzureModule>();
			builder.RegisterModule<SwitchClientModule>();

			builder.RegisterModule(CoreModule.Configure().WithEncryptionFeature(false));
			builder.RegisterModule<ResidentialPortalDataStoreModule>();

			builder.RegisterAssemblyTypes(GetType().Assembly).Where( x=>x!=typeof(TestSettings)).AsImplementedInterfaces();
			
			builder.RegisterType<NoProfiler>().AsImplementedInterfaces();
			builder.RegisterType<NoCacheProvider>().AsImplementedInterfaces();
			
			builder.Register<Func<ISecretsRepository>>(c => {
				var context = c.Resolve<IComponentContext>();
				return ()=>context.Resolve<ISecretsRepository>(); 
			});
			builder.Register(c =>
					new TestSettings(_environmentName,c.Resolve<Func<ISecretsRepository>>()))
				.AsImplementedInterfaces();
		}

	}
}