using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Facade.Metadata;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.UiFlows.Core.Flows.Initialization
{
	public interface IUiFlowInitializationStep
	{
		Task<ScreenEvent> InitializeContext(UiFlowContextData newContext, IDictionary<string, object> flowInputData,
			ScreenEvent defaultEventToTrigger);

		IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
			IScreenFlowConfigurator preStartCfg, UiFlowContextData contextData);

		bool Authorize();
		
	}

	public interface IUiFlowInitializationStep<out TFlowType> : IUiFlowInitializationStep
	{
		TFlowType InitializerOfFlowType { get; }
	}
}