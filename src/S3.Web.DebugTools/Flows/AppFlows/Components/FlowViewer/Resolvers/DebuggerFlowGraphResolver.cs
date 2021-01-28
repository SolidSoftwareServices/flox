using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using S3.Web.DebugTools.Flows.AppFlows.Graphs.Models;
using S3.CoreServices.System;
using S3.CoreServices.System.FastReflection;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Initialization.Models;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Infrastructure.DataSources;


namespace S3.Web.DebugTools.Flows.AppFlows.Components.FlowViewer.Resolvers
{
	class DebuggerFlowGraphResolver : IFlowGraphResolver
	{
		private readonly IUiFlowContextRepository _repository;
		private readonly IIndex<string, IUiFlow> _flowHandlers;

		public DebuggerFlowGraphResolver(IUiFlowContextRepository repository, IIndex<string, IUiFlow> flowHandlers)
		{
			_repository = repository;
			_flowHandlers = flowHandlers;
		}

		
		public async Task<FGraph> GetGraph(string flowHandler)
		{
			var graph = new FGraph();

			var flow = await ResolveUiFlowFromHandler(flowHandler);

			var flowCluster = await BuildFlow(flow);
			flowCluster = await AddContainerStep(flow, flowCluster);
			graph.AddCluster(flowCluster);

			return graph;
		}



		private async Task<FCluster> AddContainerStep(UiFlow flow, FCluster flowCluster)
		{
			var result = flowCluster;
			if (flow.CurrentState.ContextData.IsInContainer())
			{
				var containerFlow = await ResolveUiFlowFromHandler(flow.CurrentState.ContextData.ContainerFlowHandler);
				result = new FCluster(
					$"{containerFlow.FlowTypeId.ToUpper()}.{containerFlow.CurrentState.ContextData.CurrentScreenStep}");
				result.AddNode(flowCluster);

			}

			return result;
		}

		private Task<FCluster> BuildFlow(UiFlow flow)
		{
			var cluster = new FCluster(flow.FlowTypeId.ToUpper());
			var contextData = flow.CurrentState.ContextData;



			var nodes = flow.ScreenConfigurations.Select(x =>
			{
				var screenName = (ScreenName) x.ScreenName;
				var isCurrentStep = contextData.CurrentScreenStep.Equals(screenName);
				var alreadyExecuted = contextData.EventsLog.Any(e => e.FromStep.Equals(screenName));
				FNode.FNodeColor color = isCurrentStep
					? FNode.FNodeColor.Blue
					: alreadyExecuted
						? FNode.FNodeColor.Green
						: FNode.FNodeColor.Gray;

				return cluster.AddNode(new FNode(x.ScreenName, color)
				{
					Thickness = isCurrentStep ? 3 : alreadyExecuted ? 2 : 1
				});
			}).ToDictionary(x => x.Label, x => x);
			foreach (var step in flow.ScreenConfigurations)
			{
				var screenName = (ScreenName) step.ScreenName;
				var node = nodes[step.ScreenName];
				foreach (var screenTransition in step.Transitions)
				{
					if (nodes.ContainsKey(screenTransition.DestinationScreen))
					{
						var dest = nodes[screenTransition.DestinationScreen];
						var edge = node.AddEdge(dest, screenTransition.DisplayName);
						var timesExecuted = contextData.EventsLog.Count(x =>
							x.FromStep == screenName && x.ToStep == dest.Label &&
							x.Event == (ScreenEvent) screenTransition.EventName);
						edge.Thickness = timesExecuted == 0 ? 1 : 3;
						const int maxDash = 7;
						edge.Dash = timesExecuted >= 1
							? (Math.Max(maxDash - timesExecuted, 0)).ToString()
							: maxDash.ToString();
						edge.ToolTip = $"Times= {timesExecuted}";
					}
				}


			}

			return Task.FromResult(cluster);

		}

		private async Task<UiFlow> ResolveUiFlowFromHandler(string flowHandler)
		{
			if (string.IsNullOrWhiteSpace(flowHandler))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(flowHandler));
			}

			var uiFlowContextData = await _repository.Get(flowHandler);
			var type = uiFlowContextData?.FlowType;
			var result = (UiFlow) (type == null ? null : _flowHandlers[type.ToLowerInvariant()]);
			return result?.RestoreStateMachine(uiFlowContextData);
		}



		public async Task<object> ResolveDebuggingData(string flowHandler)
		{
			var data = await _repository.Get(flowHandler);
			return data.ViewModels.ToDictionary(x => x.Key, x => ResolveStepDataView(x.Value.UserData));

		}

		private object ResolveStepDataView(UiFlowScreenModel src)
		{
			var result = (IDictionary<string, object>) (src.ToExpandoObject());
			var exclude = new[] {nameof(UiFlowScreenModel.DontValidateEvents), nameof(UiFlowScreenModel.Errors)};
			var propertyNames = typeof(UiFlowScreenModel).GetPropertiesFast().Union(typeof(InitialFlowScreenModel).GetPropertiesFast()).Select(x=>x.Name).Where(x=>!exclude.Contains(x));
			foreach (var propertyName in propertyNames)
			{
				result.RemoveIfExists(propertyName);
			}

			return result;
		}
	}
}
