using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Facade.Metadata;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.DefaultModels;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.UiFlows.Core.Flows.Initialization
{
	public abstract class UiFlowInitializationStep<TFlowType, TFlowInputData> : IUiFlowInitializationStep<TFlowType>
		where TFlowInputData : InitialFlowScreenModel, new()
	{
		public async Task<ScreenEvent> InitializeContext(UiFlowContextData newContext,
			IDictionary<string, object> flowInputData,
			ScreenEvent defaultEventToTrigger)
		{
			var data = await BuildStartData(newContext, flowInputData);
			data.Metadata.FlowHandler = newContext.FlowHandler;
			data.Metadata.FlowType = newContext.FlowType;
			data.Metadata.FlowScreenName = typeof(EmptyScreenModel).FullName;
			newContext.SetCurrentStepData(data);

			return await ResolveInitializationEventToTrigger(defaultEventToTrigger, data);
		}

		public virtual IScreenFlowConfigurator
			OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
				UiFlowContextData contextData)
		{
			return preStartCfg;
		}

		public abstract bool Authorize();
	

		public abstract TFlowType InitializerOfFlowType { get; }

		private Task< ScreenEvent> ResolveInitializationEventToTrigger(ScreenEvent defaultEventToTriggerAfter,
			UiFlowScreenModel screenModel)
		{
			return OnResolveInitializationEventToTrigger(defaultEventToTriggerAfter,screenModel);
		}
		protected virtual Task< ScreenEvent> OnResolveInitializationEventToTrigger(ScreenEvent defaultEventToTriggerAfter,
			UiFlowScreenModel screenModel)
		{
			return Task.FromResult(defaultEventToTriggerAfter);
		}
		private async Task<TFlowInputData> BuildStartData(UiFlowContextData newContext,
			IDictionary<string, object> flowInputData)
		{
			var expandoObject = flowInputData.ToExpandoObject();

			var result = await OnBuildStartData(newContext, expandoObject.ToObjectOfType<TFlowInputData>(false));

			return result;
		}

		protected virtual async Task<TFlowInputData> OnBuildStartData(UiFlowContextData newContext,
			TFlowInputData preloadedInputData)
		{
			return preloadedInputData;
		}
	}
}