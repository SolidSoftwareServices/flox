using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Ei.Rp.Web.DebugTools.Areas.FlowDebugger.Models;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ei.Rp.Web.DebugTools.Areas.FlowDebugger.Controllers
{
	[AllowAnonymous]
	[Area(Areas.FlowDebugger)]
    public class DebuggerController : Controller
	{
		private readonly IUiFlowContextRepository _repository;
		public DebuggerController(IUiFlowContextRepository repository)
		{
			_repository = repository;
		}

		public async Task<IActionResult> Index()
		{
			var debuggerModel = new DebuggerModel();
			try
			{
				var contextData = await _repository.GetLastSnapshot();
				if (contextData != null)
				{
					UiFlowScreenModel screenModel = contextData.GetCurrentStepData<UiFlowScreenModel>();
					string flowHandler = screenModel.FlowHandler;
					while (screenModel.Metadata.ContainedFlowHandler != null)
					{
						contextData = await _repository.Get(screenModel.Metadata.ContainedFlowHandler);
						screenModel = contextData.GetCurrentStepData<UiFlowScreenModel>();
						flowHandler = screenModel.FlowHandler;
					}


					debuggerModel.FlowHandler = flowHandler;
				}
			}
			catch (Exception ex)
			{
				debuggerModel.Error = $"Possible cause: You need to refresh the page once you have the application with a flow rendered {Environment.NewLine}{ex}";
				
			}



			return View(debuggerModel);
        }
    }
}