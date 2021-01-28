using System.Threading.Tasks;
using Ei.Rp.Web.DebugTools.Flows.AppFlows.Graphs.Models;

namespace Ei.Rp.Web.DebugTools.Flows.AppFlows.Components.FlowViewer.Resolvers
{
	internal interface IFlowGraphResolver
	{
		Task<FGraph> GetGraph(string flowHandler);
		Task<object> ResolveDebuggingData(string flowHandler);


	}
}