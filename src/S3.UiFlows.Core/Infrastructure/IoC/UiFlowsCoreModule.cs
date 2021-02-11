using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using S3.CoreServices.IoC.Autofac;
using S3.CoreServices.Profiling;
using S3.CoreServices.System;
using S3.CoreServices.System.FastReflection;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.DataSources.Repositories;
using S3.UiFlows.Core.DataSources.Repositories.Adapters;
using S3.UiFlows.Core.DataSources.Stores;
using S3.UiFlows.Core.Facade.CurrentView;
using S3.UiFlows.Core.Facade.FlowResultResolver;
using S3.UiFlows.Core.Facade.InitialView;
using S3.UiFlows.Core.Facade.SetContainedView;
using S3.UiFlows.Core.Facade.TriggerEventOnView;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Registry;

namespace S3.UiFlows.Core.Infrastructure.IoC
{
	public class UiFlowsCoreModule : Module 
	{
		private readonly Func<Type, object> _getUninitializedObjectFactory;
		private readonly IFlowsRegistry _flowsRegistry;

		internal UiFlowsCoreModule( Func<Type,object> getUninitializedObjectFactory,IFlowsRegistry flowsRegistry)
		{
			_getUninitializedObjectFactory = getUninitializedObjectFactory;
			_flowsRegistry = flowsRegistry;
		}

		protected override void Load(ContainerBuilder builder)
		{
			RegisterContextRepository(builder);
			RegisterUiFlows(builder);
			RegisterFlowRequestHandlers(builder);
		}

		private void RegisterFlowRequestHandlers(ContainerBuilder builder)
		{
			builder.RegisterGeneric(typeof( FlowInitialViewRequestHandler < >)).AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterGeneric(typeof(FlowCurrentViewRequestHandler<>)).AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterGeneric(typeof(FlowTriggerEventOnViewHandler<>)).AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterGeneric(typeof(FlowResultResolverRequestHandler<>)).AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterGeneric(typeof(SetContainedViewRequestHandler<>)).AsImplementedInterfaces().WithInterfaceProfiling();
		}
		private void RegisterUiFlows(ContainerBuilder builder)
		{
			builder.RegisterType<UiFlow>().AsSelf();
			RegisterAppFlowScreens(builder);
			RegisterAppFlowTypes(builder);
		}

		private void RegisterAppFlowScreens(ContainerBuilder builder)
		{
			RegisterScreens();

			RegisterFlowInitializers();

			void RegisterFlowInitializers()
			{
				var initializers = new Dictionary<string, Type>();
				foreach (var type in TypesFinder.Resolver.FindConcreteTypesOf<IUiFlowInitializationStep>(false))
				{
					var initStep =
						(IUiFlowInitializationStep)_getUninitializedObjectFactory(type);

					initStep.SetPropertyValueFast(nameof(UiFlowInitializationStep.Registry), _flowsRegistry);
					var flowType = initStep.InitializerOfFlowType;
					if (string.IsNullOrWhiteSpace(flowType))
						throw new Exception(
							$"Could not resolve flow type of {type.FullName}.{nameof(IUiFlowInitializationStep.InitializerOfFlowType)}.");
					initializers.Add(flowType, initStep.GetType());
				}

				foreach (var key in initializers.Keys)
					builder.RegisterType(initializers[key])
						.Keyed<IUiFlowInitializationStep>(key.ToLowerInvariant()).WithInterfaceProfiling();
			}

			void RegisterScreens()
			{
				var sb = new StringBuilder();
				var steps = new Dictionary<string, HashSet<ScreenName>>();
				var screens = new Dictionary<string, HashSet<Type>>();
				foreach (var type in TypesFinder.Resolver.FindConcreteTypesOf<IUiFlowScreen>(
					false))
				{
					var uiFlowScreen = (IUiFlowScreen)_getUninitializedObjectFactory(type);
					uiFlowScreen.SetPropertyValueFast(nameof(UiFlowScreen.Registry), _flowsRegistry);
					var screenStep = uiFlowScreen.ScreenStep;
					
					if (screenStep == null)
						sb.AppendLine($"Could not resolve {type.FullName}.{nameof(IUiFlowScreen.ScreenStep)}.");

					var flowType = uiFlowScreen.IncludedInFlowType;
					if (string.IsNullOrWhiteSpace(flowType ))
						sb.AppendLine(
							$"Could not resolve {type.FullName}.{nameof(IUiFlowScreen.IncludedInFlowType)}.");

					if (!screens.ContainsKey(flowType)) screens.Add(flowType, new HashSet<Type>());

					screens[flowType].Add(type);

					if (!steps.ContainsKey(flowType)) steps.Add(flowType, new HashSet<ScreenName>());

					steps[flowType].Add(screenStep);
				}

				if (sb.Length > 0)
					throw new InvalidOperationException(
						$"The following properties must be resolvable dynamically.{Environment.NewLine}{sb}");

				foreach (var key in steps.Keys)
					builder.Register(c => steps[key])
						.Keyed<IEnumerable<ScreenName>>(key.ToLowerInvariant()).SingleInstance().WithInterfaceProfiling();

				foreach (var key in screens.Keys)
					builder.RegisterTypes(screens[key].ToArray())
						.Keyed<IUiFlowScreen>(key.ToLowerInvariant()).WithInterfaceProfiling();
			}
		}

		private void RegisterAppFlowTypes(ContainerBuilder builder)
		{
			var flowTypes = _flowsRegistry.AllFlows.Select(x=>x.Name).ToArray();
			foreach (var eiFlowType in flowTypes)
				builder.Register(c =>
					{
						var result = c.Resolve<UiFlow>();
						result.FlowTypeId = eiFlowType;

						return result;
					})
					//TODO: as strict enum value
					.Keyed<IUiFlow>(eiFlowType.ToLowerInvariant()).InstancePerLifetimeScope().WithInterfaceProfiling();

			builder.Register(c=>new FlowNames(flowTypes.Select(x => x.ToString().ToLowerInvariant()))).AsSelf();
		}

		private void RegisterContextRepository(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly).Where(x =>
					!x.IsAssignableTo<IUiFlowContextRepository>()
					&& !x.IsAssignableTo<IFlowsStore>()
					&& x!=GetType()
					&& !x.IsAssignableTo<IFlowsRegistry>()
					)
				.AsImplementedInterfaces().WithInterfaceProfiling();

			builder.RegisterType<InMemoryFlowsStore>()
				.SingleInstance()
				.AsImplementedInterfaces()
				.WithInterfaceProfiling();

			builder.RegisterType<UiFlowContextDataRepositoryDecorator>()
				.InstancePerLifetimeScope()
				.AsImplementedInterfaces()
				.WithInterfaceProfiling();
		}
	}
}