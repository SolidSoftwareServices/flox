using Autofac;
using AutoFac.Extras.NLog.DotNetCore;
using EI.RP.CoreServices.Azure.Infrastructure.IoC;
using EI.RP.CoreServices.Infrastructure;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.OData.Client.Infrastructure.IoC;
using EI.RP.CoreServices.Profiling;
using EI.RP.DataServices.SAP.Clients.Infrastructure.IoC;
using Ei.Rp.Mvc.Core.Profiler;
using EI.RP.Stubs.CoreServices.Http.Session;
using EI.RP.Stubs.IoC;

namespace EI.RP.DataServices.SAP.Clients.IntegrationTests.Infrastructure
{
	internal class TestsModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule<NLogModule>();

			builder.RegisterModule(CoreModule.Configure().WithEncryptionFeature(false));
			builder.RegisterModule<ODataClientModule>();

			builder.RegisterModule<SapRepositoriesModule>();

			
			builder.RegisterModule<StubsModule>();

			builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces();


            builder.RegisterType<NoProfiler>().AsImplementedInterfaces();

            builder.RegisterInstance(new TestSettings()).AsImplementedInterfaces().SingleInstance();

            builder.RegisterInstance(SessionDataHolder.Instance).SingleInstance().AsImplementedInterfaces();

            builder.RegisterModule<AzureModule>();
		}

		

    }
}
