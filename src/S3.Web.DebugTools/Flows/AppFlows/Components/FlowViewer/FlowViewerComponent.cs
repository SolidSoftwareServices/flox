using System.Linq;
using System.Threading.Tasks;
using S3.Web.DebugTools.Flows.AppFlows.Components.FlowViewer.Resolvers;
using S3.Web.DebugTools.Flows.AppFlows.Graphs.Models;
using S3.CoreServices.Serialization;
using S3.CoreServices.System.DependencyInjection;
using S3.Web.DebugTools.Infrastructure.FlowDiagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace S3.Web.DebugTools.Flows.AppFlows.Components.FlowViewer
{
	[ViewComponent(Name = "FlowViewer")]
	public class FlowViewerComponent : ViewComponent
	{
		private readonly IFlowChangesRecorder _changesRecorder;

		public FlowViewerComponent(IFlowChangesRecorder changesRecorder)
		{
			_changesRecorder = changesRecorder;
		}

		public async Task<IViewComponentResult> InvokeAsync(IComponentInput input)
		{
			Model model=new Model
			{
				Error=input.Error
			};
			if (input.Error == null)
			{
				//TODO INJECTABLE, visibility issue
				var flowResolver = HttpContext.RequestServices.Resolve<IFlowGraphResolver>();
				var data = flowResolver.ResolveDebuggingData(input.FlowHandler);
				var graph = flowResolver.GetGraph(input.FlowHandler);
				model.GraphAsJson = (await graph).ToJson(camelCase: true);
				model.StepsDatasAsJson = (await data).ToJson();

				GetDifferences();
			}

			return View("_Index", model);
		}

		private void GetDifferences()
		{
			var allRequests = _changesRecorder.Requests();
			var resultContainer = _changesRecorder.GetFlowStateDifferences(11.ToString());
			var result46 = _changesRecorder.GetFlowStateDifferences(46.ToString());
			var result47 = _changesRecorder.GetFlowStateDifferences(47.ToString());

		}


		public interface IComponentInput
		{

			string FlowHandler { get; }
			string Error { get; set; }

		}
		public class Model
		{

			public string GraphAsJson { get; set; }
			public string StepsDatasAsJson { get; set; }
			public string Error { get; set; }

			public int Count { get; set; }
		}
	}

	

	
}