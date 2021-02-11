using Autofac;
using S3.App.Flows.AppFlows;
using S3.CoreServices.Infrastructure;
using S3.Mvc.Core.Profiler.IoC;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Mvc.Infrastructure.IoC;

namespace S3.App.Infrastructure.IoC
{
	public class IoCRegistrationModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule(CoreModule.Configure().WithEncryptionFeature());
			builder.RegisterModule(new  UiFlowsMvcModule(this.GetType().Assembly));
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