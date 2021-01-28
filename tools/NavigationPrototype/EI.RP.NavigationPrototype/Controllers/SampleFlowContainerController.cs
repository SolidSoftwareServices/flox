using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows;
using EI.RP.NavigationPrototype.Models;
using EI.RP.UiFlows.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.NavigationPrototype.Controllers
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