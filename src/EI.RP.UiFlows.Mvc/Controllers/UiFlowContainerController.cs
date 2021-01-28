using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.Mvc.Core.Cryptography.Urls;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Ei.Rp.Mvc.Core.Controllers.ControllerBase;

namespace EI.RP.UiFlows.Mvc.Controllers
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public abstract class UiFlowContainerController<TViewModel,TFlowType>: ControllerBase
		where TViewModel: UiFlowScreenModel
	where TFlowType:struct

	{
		private readonly UiFlowController _flowController;

		protected UiFlowContainerController(UiFlowController flowController)
		{
			_flowController = flowController;
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
				viewModel.SetContainedFlow(Enum.Parse<TFlowType>(flowType));
			}
			return RedirectToAction(nameof(Index), await viewModel.ToValidRouteValueAsync(ControllerContext.HttpContext));
		}

		protected abstract Task<IActionResult> GetIndexView(TViewModel viewModel);
		public async Task<IActionResult> DisplayFlow(TViewModel viewModel, TFlowType flow)
		{

			if (!Equals(viewModel.Metadata.ContainedFlowType.ToEnum<TFlowType>(), flow))
			{
				viewModel.Metadata.ContainedFlowHandler = null;
				viewModel.SetContainedFlow(flow);
			}
			return RedirectToAction(nameof(Index), await viewModel.ToValidRouteValueAsync(ControllerContext.HttpContext));
		}

	}
}