
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Autofac;
using S3.CoreServices.IoC.Autofac;
using S3.CoreServices.Profiling;
using S3.CoreServices.System;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Infrastructure.IoC;
using S3.UiFlows.Mvc.Components;
using S3.UiFlows.Mvc.Controllers;
using S3.UiFlows.Mvc.Views;
using Module = Autofac.Module;

namespace S3.UiFlows.Mvc.Infrastructure.IoC
{
	public class UiFlowsMvcModule: Module 
	{
		private readonly Assembly _flowsAssembly;

		public UiFlowsMvcModule(Assembly flowsAssembly)
		{
			_flowsAssembly = flowsAssembly;
		}

		internal static FlowsRegistry Registry { get; set; }


		protected override void Load(ContainerBuilder builder)
		{
			if (Registry == null) throw new InvalidOperationException($"{nameof(Registry)} is expected");
			builder.RegisterAssemblyTypes(GetType().Assembly)
				.Where(
					x => !x.IsOneOf(typeof(UiFlow), typeof(FlowsRegistry))
					     && !x.IsAssignableTo<IProfiler>()
				)
				.AsImplementedInterfaces()
				.WithInterfaceProfiling();

			builder.RegisterModule(new UiFlowsCoreModule(RuntimeHelpers.GetUninitializedObject,Registry));
			builder.RegisterType<UiFlowController>().AsSelf().WithClassProfiling();
			
			RegisterEngineTypes(builder);

			RegisterDeferredComponentHandlers(builder);

			builder.RegisterInstance(Registry).SingleInstance().AsImplementedInterfaces();

		}

		private void RegisterDeferredComponentHandlers(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(_flowsAssembly).Where(x =>
				!x.IsAbstract && x.BaseType.ImplementsOpenGeneric(typeof(FlowStepComponent<,>)))
				.AsSelf()
				.InstancePerLifetimeScope();
		}


		private void RegisterEngineTypes(ContainerBuilder builder)
		{
			builder.RegisterType<FlowsMvcViewOptionsSetup>().AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterType<FlowsViewEngine>().AsSelf().SingleInstance().WithClassProfiling();
		}


		
	}
}