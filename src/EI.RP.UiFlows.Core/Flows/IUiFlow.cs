using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Facade.Metadata;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.ErrorHandling;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.UiFlows.Core.Flows
{
	public interface IUiFlow
	{
		string FlowTypeId { get; }
		(string ViewName, IUiFlowContextData ContextData) CurrentState { get; }

		Task<UiFlowScreenModel> Execute(ScreenEvent transitionTrigger, UiFlowScreenModel screenModel = null,
			Action<IEnumerable<UiFlowUserInputError>, UiFlowScreenModel> onErrors = null);

		Task<UiFlowScreenModel> StartNew(string containerFlowHandler,
			IDictionary<string, object> flowInputData);

		/// <summary>
		///     Returns a dotgraph defintion of the flow that can be seen on http://www.webgraphviz.com/
		/// </summary>
		/// <returns></returns>
		/// <see cref="https://en.wikipedia.org/wiki/DOT_%28graph_description_language%29" />
		string ToDotGraph();

		Task<UiFlowScreenModel> GetCurrentStepData(string flowHandler,
			IDictionary<string, object> stepViewCustomizations = null);

		Task SetCurrentStepData(UiFlowScreenModel screenModel);

		Task<IUiFlowScreen> GetCurrentScreen(string flowHandler);
		Task<AppMetadata.FlowMetadata> GetMetadata();
	}
}