using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens.Models;
using Microsoft.AspNetCore.Mvc;

namespace S3.UiFlows.Mvc.Controllers
{
	public interface IUiFlowController
	{
		Task<IActionResult> Init(UiFlowController.InitializeUiFlowRequest request);
		Task<IActionResult> Current(UiFlowController.CurrentViewRequest request);
		Task<IActionResult> ContainedView(UiFlowController.ContainedViewRequest request);
		Task<IActionResult> NewContainedView( UiFlowController.GetNewContainedViewRequest request);
		
		Task<IActionResult> OnEvent([FromForm] string trigger, [FromForm] UiFlowScreenModel model);
	}
}