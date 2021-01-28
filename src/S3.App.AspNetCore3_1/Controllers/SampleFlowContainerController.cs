using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using S3.App.AspNetCore3_1.Flows.AppFlows;
using S3.App.AspNetCore3_1.Models;
using S3.UiFlows.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace S3.App.AspNetCore3_1.Controllers
{
	[AllowAnonymous]
    public class SampleFlowContainerController : UiFlowContainerController<SampleFlowContainerViewModel,SampleAppFlowType>
	{
		public SampleFlowContainerController(UiFlowController flowController) : base(flowController)
		{
		}
		protected override  async Task<IActionResult> GetIndexView(SampleFlowContainerViewModel viewModel)
        {
            return View("FlowContainer", viewModel);
		}
	}
}