using System;
using System.Linq;
using Autofac;
using EI.RP.CoreServices.Azure.Infrastructure.IoC;
using EI.RP.CoreServices.Caching.Infrastructure.IoC;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Infrastructure;
using EI.RP.CoreServices.InProc.Infrastructure;
using EI.RP.CoreServices.OData.Client.Infrastructure.IoC;
using EI.RP.DataServices.EventsApi.Clients.Infrastructure;
using EI.RP.DataServices.SAP.Clients.Infrastructure.IoC;
using EI.RP.DataServices.StreamServe.Clients.IoC;
using EI.RP.DataStore.IoC;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Infrastructure.InvoicePdf;
using EI.RP.DomainServices.Infrastructure.IoC;
using Ei.Rp.Mvc.Core.Profiler.IoC;
using EI.RP.SwitchApi.Clients.IoC;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Mvc.Infrastructure.IoC;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.SharedFlowComponents;
using EI.RP.WebApp.Infrastructure.Caching.PreLoad;
using EI.RP.WebApp.Infrastructure.Caching.PreLoad.Processors;
using EI.RP.WebApp.Infrastructure.HealthChecks;
using EI.RP.WebApp.Infrastructure.PresentationServices.EventsPublisher;
using EI.RP.WebApp.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace EI.RP.WebApp.Infrastructure.IoC
{
     class IoCRegistrationModule : BaseModule
    {
	    private readonly AppSettings _appSettings;
	    private readonly IConfigurationRoot _configuration;

	    public IoCRegistrationModule(IConfigurationRoot configuration, IServiceProvider services)
	    {
		    _configuration = configuration;
			_appSettings=new AppSettings(configuration,services);
	    }

	    

	    protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(CoreModule.Configure(_appSettings).WithEncryptionFeature(true));
            builder.RegisterModule<InProcModule>();
            builder.RegisterModule<AzureModule>();
			builder.RegisterModule<ODataClientModule>();
            builder.RegisterModule<SapRepositoriesModule>();
			builder.RegisterModule<SwitchClientModule>();
			builder.RegisterModule<StreamServeClientModule>();
			builder.RegisterModule<EventsPublisherClientModule>();
            builder.RegisterModule<DomainServicesModule>();
	        builder.RegisterModule<ResidentialPortalDataStoreModule>();
	        builder.RegisterModule(new UiFlowsMvcModule<ResidentialPortalFlowType>(ContextStoreStrategy.InSession));
	        builder.RegisterModule(new CacheModule(_appSettings));
	        builder.RegisterModule<ProfilerModule>();
	        builder.RegisterModule<HealthChecksModule>();
			

			RegisterUrlHelper(builder);

			RegisterFlowServices(builder);

			RegisterPresentationServices(builder);

			builder.RegisterType<PdfInvoiceImageResolver>().AsImplementedInterfaces().WithInterfaceProfiling();

            RegisterDomainCache(builder);
            RegisterAppSettings(builder);
		}

	    private void RegisterAppSettings(ContainerBuilder builder)
	    {

		    builder.Register(c => new AppSettings(_configuration, c.Resolve<IServiceProvider>()))
			    .AsSelf()
			    .AsImplementedInterfaces()

			    .InstancePerLifetimeScope().WithInterfaceProfiling();
	    }

	    private void RegisterPresentationServices(ContainerBuilder builder)
	    {
		    var parts = typeof(IUIEventPublisher).Namespace.Split('.');
		    var prefix = string.Join(".",parts.Take(parts.Length-1));

		    builder.RegisterAssemblyTypes(GetType().Assembly)
			    .Where(x => x.Namespace != null && x.Namespace.StartsWith(prefix))
			    .AsImplementedInterfaces().WithInterfaceProfiling();
	    }

	    private void RegisterDomainCache(ContainerBuilder builder)
        {
			builder.RegisterType<CachePreloaderQueueSubscriber>().As<IHostedService>().InstancePerDependency().WithClassProfiling();
	        builder.RegisterType<CacheAccountPreLoaderQueue>().AsImplementedInterfaces().AsSelf().WithClassProfiling();
	        builder.RegisterType<SessionProviderForCachePreloader>().As<IActingAsUserSessionProvider>().WithInterfaceProfiling();
	        builder.RegisterAssemblyTypes(GetType().Assembly).Where(x => x.Implements(typeof(ICachePreloadTask)))
		        .AsImplementedInterfaces().WithInterfaceProfiling();
        }

        private void RegisterUrlHelper(ContainerBuilder builder)
        {
	        builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().SingleInstance();
	        builder.Register(c => new UrlHelper(c.Resolve<IActionContextAccessor>().ActionContext)).As<IUrlHelper>()
		        .InstancePerLifetimeScope().WithInterfaceProfiling();
        }

        private void RegisterFlowServices(ContainerBuilder builder)
        {

			//TODO: EMBEDD IN LIBRARY
	        var nss = new[] {typeof(__IoC__Locator).Namespace, typeof(ResidentialPortalFlowType).Namespace};

	        builder.RegisterAssemblyTypes(GetType().Assembly)
		        .Where(x => x.Namespace!=null && nss.Any(y => x.Namespace.StartsWith(y)) 
		                                      && !typeof(IUiFlowScreen).IsAssignableFrom(x)
		                                      ).AsImplementedInterfaces().WithInterfaceProfiling();
	        builder.RegisterAssemblyTypes(GetType().Assembly)
		        .Where(x => x.Namespace != null && nss.Any(y => x.Namespace.StartsWith(y))
		                                        && typeof(IUiFlowScreen).IsAssignableFrom(x)
		        ).AsSelf().WithClassProfiling();

			
		}
    }
}