using System.Threading.Tasks;
using S3.Web.DebugTools.Flows.AppFlows.Graphs.Models;

namespace S3.Web.DebugTools.Flows.AppFlows.Components.FlowViewer.Resolvers
{
	internal interface IFlowGraphResolver
	{
		Task<FGraph> GetGraph(string flowHandler);
		Task<object> ResolveDebuggingData(string flowHandler);


	}
}