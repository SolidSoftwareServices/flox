using Autofac;
using EI.RP.CoreServices.Caching.Infrastructure.IoC;
using EI.RP.CoreServices.Infrastructure;
using EI.RP.CoreServices.IoC.Autofac;
using Ei.Rp.Mvc.Core.Profiler.IoC;
using EI.RP.NavigationPrototype.Flows.AppFlows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Mvc.Infrastructure.IoC;

namespace EI.RP.NavigationPrototype.Infrastructure.IoC
{
	public class IoCRegistrationModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule(CoreModule.Configure().WithEncryptionFeature());
			builder.RegisterModule(
				new UiFlowsMvcModule<SampleAppFlowType>(ContextStoreStrategy.InMemoryOfSingleInstance));
			builder.RegisterModule<CacheModule>();
			builder.RegisterModule<ProfilerModule>();
			builder.RegisterAssemblyTypes(GetType().Assembly)
				.Where(x => !typeof(IUiFlowScreen).IsAssignableFrom(x))
				.AsImplementedInterfaces();

			builder.RegisterAssemblyTypes(GetType().Assembly)
				.Where(x => typeof(IUiFlowScreen).IsAssignableFrom(x))
				.AsSelf();
		}
	}
}