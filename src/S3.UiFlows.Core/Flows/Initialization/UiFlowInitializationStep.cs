using System.Collections.Generic;
using System.Threading.Tasks;
using S3.CoreServices.System;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.Facade.Metadata;
using S3.UiFlows.Core.Flows.Initialization.Models;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.DefaultModels;
using S3.UiFlows.Core.Infrastructure.DataSources;

namespace S3.UiFlows.Core.Flows.Initialization
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