using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Autofac;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Facade.CurrentView;
using EI.RP.UiFlows.Core.Facade.InitialView;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Infrastructure.IoC;
using EI.RP.UiFlows.Mvc.Controllers;
using EI.RP.UiFlows.Mvc.Views;
using Ei.Rp.Web.DebugTools.Infrastructure.IoC;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.UiFlows.Mvc.Infrastructure.IoC
{
	/// <summary>
	/// </summary>
	/// <typeparam name="TFlowTypesEnum">enumeration containing the various flow types</typeparam>
	public class UiFlowsMvcModule<TFlowTypesEnum> : BaseModule where TFlowTypesEnum : struct
	{
		private readonly ContextStoreStrategy _contextStoreStrategy;


		public UiFlowsMvcModule(ContextStoreStrategy contextStoreStrategy)
		{
			_contextStoreStrategy = contextStoreStrategy;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule<UiFlowsDebugToolsModule>();
			builder.RegisterAssemblyTypes(GetType().Assembly)
				.Where(x => x != typeof(UiFlow) && !x.IsAssignableTo<IProfiler>() &&
				            !x.IsAssignableTo<ICacheProvider>()).AsImplementedInterfaces().WithInterfaceProfiling();
			


			builder.RegisterModule(new UiFlowsCoreModule<TFlowTypesEnum>(_contextStoreStrategy,RuntimeHelpers.GetUninitializedObject));
			builder.RegisterType<UiFlowController>().AsSelf().WithClassProfiling();
			
			RegisterEngineTypes(builder);

		}

	


		private void RegisterEngineTypes(ContainerBuilder builder)
		{
			builder.RegisterType<FlowsMvcViewOptionsSetup>().AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterType<FlowsViewEngine>().AsSelf().SingleInstance().WithClassProfiling();
		}


		
	}
}