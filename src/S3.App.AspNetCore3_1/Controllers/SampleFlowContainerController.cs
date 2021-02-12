using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S3.App.Models;
using S3.UiFlows.Core.Registry;
using S3.UiFlows.Mvc.Controllers;

namespace S3.App.Controllers
{
	[AllowAnonymous]
    public class SampleFlowContainerController : UiFlowContainerController<SampleFlowContainerViewModel>
	{
		public SampleFlowContainerController(UiFlowController flowController, IFlowsRegistry registry) : base(flowController,registry)
		{
		}
		protected override  async Task<IActionResult> GetIndexView(SampleFlowContainerViewModel viewModel)
        {
            return View("FlowContainer", viewModel);
		}
	}
}