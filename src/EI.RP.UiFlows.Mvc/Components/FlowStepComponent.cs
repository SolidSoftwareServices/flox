using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.UiFlows.Mvc.Components
{

	/// <summary>
	/// Represents a flow component embedded in a flow step
	/// </summary>
	/// <typeparam name="TComponentInput">The input type required to generate the <see cref="TComponentModel"/>.</typeparam>
	/// <typeparam name="TComponentModel">The component model</typeparam>
	public abstract class FlowStepComponent< TComponentInput, TComponentModel> : ViewComponent
	where TComponentModel : FlowComponentViewModel where TComponentInput : new()
	{
		public string ViewName { get; }

		protected FlowStepComponent(string viewName)
		{
			ViewName = viewName;
		}
		public async Task<IViewComponentResult> InvokeAsync(TComponentInput input, UiFlowScreenModel screenModel)
		{
			if (input == null)
			{
				input=new TComponentInput();
			}
			var data = await ResolveComponentDataAsync(input, screenModel);
			data.ScreenModel = screenModel;
			return await ResolveResultAsync(data);
		}

		/// <summary>
		/// Resolves the component model
		/// </summary>
		/// <param name="input"></param>
		/// <param name="screenModel"></param>
		/// <returns></returns>
		protected abstract Task<TComponentModel> ResolveComponentDataAsync(TComponentInput input,
			UiFlowScreenModel screenModel);

		/// <summary>
		/// It resolves the view to show...
		/// </summary>
		/// <param name="componentModel"></param>
		/// <returns></returns>
		protected virtual async Task<IViewComponentResult> ResolveResultAsync(TComponentModel componentModel)
		{
			return View(ViewName, componentModel);
		}

		

	}
}