using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using Ei.Rp.Mvc.Core.Authx;
using EI.RP.WebApp.Models.Membership;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EI.RP.WebApp.Models;
using EI.RP.WebApp.Models.Static;

namespace EI.RP.WebApp.Controllers
{
	[AuthorizedOnlyDuringDevelopment]
    public class TestSettingsController : Ei.Rp.Mvc.Core.Controllers.ControllerBase
    {
	    private readonly IConfigurableTestingItems _configurableTestingItems;

	    public TestSettingsController(IConfigurableTestingItems configurableTestingItems)
	    {
		    _configurableTestingItems = configurableTestingItems;
	    }

	    /// <summary>
        ///  To be removed
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> TestSettings()
        {
	        var model = new TestSettingsViewModel
	        {
		        CanShowCostToDate = _configurableTestingItems.CanShowCostToDate,
		        CostToDateAmount = _configurableTestingItems.CostToDateAmount,
		        CostToDateSince = _configurableTestingItems.CostToDateSince,
		        SimulateAppWorkloadDelaySeconds = _configurableTestingItems.SimulateAppWorkloadDelaySeconds,
		        SimulateConsumptionDataFailure = _configurableTestingItems.SimulateConsumptionDataFailure
	        };

	        return View("TestSettingsView", model);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Submit(TestSettingsViewModel viewModel)
        {
            var loginModel = new LoginViewModel();

            _configurableTestingItems.CanShowCostToDate = viewModel.CanShowCostToDate;
            _configurableTestingItems.CostToDateAmount = viewModel.CostToDateAmount;
            _configurableTestingItems.CostToDateSince = viewModel.CostToDateSince;
            _configurableTestingItems.SimulateAppWorkloadDelaySeconds = viewModel.SimulateAppWorkloadDelaySeconds;
            _configurableTestingItems.SimulateConsumptionDataFailure = viewModel.SimulateConsumptionDataFailure;

			return RedirectToAction("Login", "Login", loginModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> UiFlows()
        {
	      
	        return View("FlowsView", new StaticModel());
        }
	}
}