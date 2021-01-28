using System;
using Autofac;
using AutoFac.Extras.NLog.DotNetCore;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.Azure.Infrastructure.IoC;
using EI.RP.CoreServices.Caching.Infrastructure.IoC;
using EI.RP.CoreServices.Infrastructure;
using EI.RP.CoreServices.OData.Client.Infrastructure.IoC;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.Secrets;
using EI.RP.DataServices.SAP.Clients.Infrastructure.IoC;
using Ei.Rp.Mvc.Core.Profiler;
using EI.RP.Stubs.CoreServices.Http.Session;
using EI.RP.Stubs.IoC;
using Microsoft.Extensions.Configuration;

namespace ResidentialPortalCLI.OData.Infrastructure
{
	internal class MainModule : Module
	{
		private readonly IConfiguration _configuration;

		public MainModule(IConfigurationRoot configuration)
		{
			_configuration = configuration;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule<NLogModule>();

			var platformSettings = new AppSettings(_configuration, null);
			builder.RegisterModule(CoreModule.Configure(platformSettings).WithEncryptionFeature(false));
			builder.RegisterModule<ODataClientModule>();

			builder.RegisterModule<SapRepositoriesModule>();


			builder.RegisterModule<StubsModule>();


			builder.RegisterModule(new CacheModule(platformSettings));


			builder.RegisterType<NoProfiler>().AsImplementedInterfaces();

			builder.RegisterInstance(SessionDataHolder.Instance).AsImplementedInterfaces();

			builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces();
			builder.RegisterModule<AzureModule>();
			builder.Register(c => new AppSettings(_configuration, c.Resolve<IServiceProvider>()))
				.AsImplementedInterfaces().SingleInstance();
		}
	}
}