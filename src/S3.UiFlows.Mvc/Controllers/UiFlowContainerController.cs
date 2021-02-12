using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S3.Mvc.Core.Cryptography.Urls;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Registry;
using ControllerBase = S3.Mvc.Core.Controllers.ControllerBase;

namespace S3.UiFlows.Mvc.Controllers
{

	public abstract class UiFlowContainerController<TViewModel>: ControllerBase
		where TViewModel: UiFlowScreenModel

	{
		private readonly UiFlowController _flowController;
		private readonly IFlowsRegistry _flowsRegistry;

		protected UiFlowContainerController(UiFlowController flowController, IFlowsRegistry flowsRegistry)
		{
			_flowController = flowController;
			_flowsRegistry = flowsRegistry;
		}
		public async Task<IActionResult> Index(TViewModel viewModel)
		{
			return await GetIndexView(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Process([FromForm]TViewModel viewModel,[FromForm] string trigger, UiFlowScreenModel viewData, string flowType = null)
		{
			_flowController.ControllerContext = this.ControllerContext;

			await _flowController.OnEvent(trigger,viewData);
			if (flowType != null)
			{
				viewModel.SetContainedFlow(_flowsRegistry.GetByName(flowType,true).Name);
			}
			return RedirectToAction(nameof(Index), await viewModel.ToValidRouteValueAsync(ControllerContext.HttpContext));
		}

		protected abstract Task<IActionResult> GetIndexView(TViewModel viewModel);
		public async Task<IActionResult> DisplayFlow(TViewModel viewModel, string flow)
		{

			if (!Equals(viewModel.Metadata.ContainedFlowType, flow))
			{
				viewModel.Metadata.ContainedFlowHandler = null;
				viewModel.SetContainedFlow(flow);
			}
			return RedirectToAction(nameof(Index), await viewModel.ToValidRouteValueAsync(ControllerContext.HttpContext));
		}

	}
}