
using System.Runtime.CompilerServices;

using Autofac;
using S3.CoreServices.IoC.Autofac;
using S3.CoreServices.Profiling;
using S3.CoreServices.System;
using S3.UiFlows.Core.Flows;

using S3.UiFlows.Core.Infrastructure.DataSources;
using S3.UiFlows.Core.Infrastructure.IoC;
using S3.UiFlows.Mvc.Components;
using S3.UiFlows.Mvc.Components.Deferred;
using S3.UiFlows.Mvc.Controllers;
using S3.UiFlows.Mvc.Views;


namespace S3.UiFlows.Mvc.Infrastructure.IoC
{
	/// <summary>
	/// </summary>
	/// <typeparam name="TFlowTypesEnum">enumeration containing the various flow types</typeparam>
	public class UiFlowsMvcModule<TFlowTypesEnum> : Module where TFlowTypesEnum : struct
	{
		private readonly ContextStoreStrategy _contextStoreStrategy;


		public UiFlowsMvcModule(ContextStoreStrategy contextStoreStrategy)
		{
			_contextStoreStrategy = contextStoreStrategy;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly)
				.Where(x => x != typeof(UiFlow) && !x.IsAssignableTo<IProfiler>() )
				.AsImplementedInterfaces()
				.WithInterfaceProfiling();
			


			builder.RegisterModule(new UiFlowsCoreModule<TFlowTypesEnum>(_contextStoreStrategy,RuntimeHelpers.GetUninitializedObject));
			builder.RegisterType<UiFlowController>().AsSelf().WithClassProfiling();
			
			RegisterEngineTypes(builder);

			RegisterDeferredComponentHandlers(builder);

		}

		private void RegisterDeferredComponentHandlers(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(typeof(TFlowTypesEnum).Assembly).Where(x =>
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