using Autofac;
using EI.HybridAdapter.WebApi.Infrastructure.Settings;
using EI.RP.CoreServices.IoC.Autofac;
using Ei.Rp.Mvc.Core.Profiler.IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace EI.HybridAdapter.WebApi.Infrastructure.IoC
{
	class IoCRegistrationModule : BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule<ProfilerModule>();

			RegisterAppSettings(builder);
			RegisterUrlHelper(builder);
		}
		private void RegisterAppSettings(ContainerBuilder builder)
		{
			builder.RegisterType<AppSettings>().AsImplementedInterfaces().SingleInstance();
		}
		private void RegisterUrlHelper(ContainerBuilder builder)
		{
			builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().SingleInstance();
			builder.Register(c => new UrlHelper(c.Resolve<IActionContextAccessor>().ActionContext)).As<IUrlHelper>()
				.InstancePerLifetimeScope();
		}
	}
}