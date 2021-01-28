using System;
using Autofac;
using AutoFac.Extras.NLog.DotNetCore;
using EI.RP.CoreServices.Azure.Infrastructure.IoC;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Caching.InMemory;
using EI.RP.CoreServices.Infrastructure;
using EI.RP.CoreServices.InProc.Infrastructure;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.OData.Client.Infrastructure.IoC;
using EI.RP.CoreServices.Profiling;
using EI.RP.DataServices.SAP.Clients.Infrastructure.IoC;
using EI.RP.DataServices.StreamServe.Clients.IoC;
using EI.RP.DataStore.IoC;
using EI.RP.DomainServices.Infrastructure.InvoicePdf;
using EI.RP.DomainServices.Infrastructure.IoC;
using Ei.Rp.Mvc.Core.Profiler;
using EI.RP.Stubs.IoC;
using EI.RP.SwitchApi.Clients.IoC;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace EI.RP.DomainServices.IntegrationTests.Infrastructure
{
	internal class TestsModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule<NLogModule>();

			builder.RegisterModule(CoreModule.Configure().WithEncryptionFeature(false));
			builder.RegisterModule<InProcModule>();
			builder.RegisterModule<ODataClientModule>();

			builder.RegisterModule<SapRepositoriesModule>();
			builder.RegisterModule<SwitchClientModule>();
			builder.RegisterModule<StreamServeClientModule>();
			builder.RegisterModule<ResidentialPortalDataStoreModule>();

			builder.RegisterModule<DomainServicesModule>();

			builder.RegisterModule<StubsModule>();
			builder.RegisterModule<AzureModule>();
			builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces();
			builder.RegisterType<PdfInvoiceImageResolver>().AsImplementedInterfaces();


			builder.RegisterType<NoProfiler>().AsImplementedInterfaces();
			RegisterNoCache();
			builder.Register(c=>new TestSettings("Test",c.Resolve<IServiceProvider>())).AsImplementedInterfaces().SingleInstance();

			void RegisterMemoryCache()
			{
				builder.Register(c => new MemoryDistributedCache(new CacheOptions())).AsImplementedInterfaces();
				builder.RegisterType<InMemoryCacheProvider>().AsImplementedInterfaces().SingleInstance();
			}

			void RegisterNoCache()
			{
				builder.RegisterType<NoCacheProvider>().AsImplementedInterfaces();
			}
		}

		private class CacheOptions : IOptions<MemoryDistributedCacheOptions>
		{
			public MemoryDistributedCacheOptions Value => new MemoryDistributedCacheOptions();
		}
	}
}
