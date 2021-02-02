using Autofac;
using S3.CoreServices.Infrastructure;
using S3.Mvc.Core.Profiler.IoC;
using S3.App.AspNetCore3_1.Flows.AppFlows;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Mvc.Infrastructure.IoC;

namespace S3.App.AspNetCore3_1.Infrastructure.IoC
{
	public class IoCRegistrationModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule(CoreModule.Configure().WithEncryptionFeature());
			builder.RegisterModule< UiFlowsMvcModule<SampleAppFlowType>>();
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