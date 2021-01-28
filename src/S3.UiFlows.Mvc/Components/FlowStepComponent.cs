using System;
using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens.Models;
using Microsoft.AspNetCore.Mvc;

namespace S3.UiFlows.Mvc.Components
{

	/// <summary>
	/// Represents a flow component embedded in a flow step
	/// </summary>
	/// <typeparam name="TComponentInput">The input type required to generate the <see cref="TComponentModel"/>.</typeparam>
	/// <typeparam name="TComponentModel">The component model</typeparam>
	public abstract partial class FlowStepComponent< TComponentInput, TComponentModel> : ViewComponent
	where TComponentModel : FlowComponentViewModel where TComponentInput : new()
	{
		public string ViewName { get; }

		protected FlowStepComponent(string viewName)
		{
			ViewName = viewName;
		}
		public async Task<IViewComponentResult> InvokeAsync(TComponentInput input, UiFlowScreenModel screenModel)
		{
			return await OnInvokeAsync(input, screenModel);
		}

		protected virtual async Task<IViewComponentResult> OnInvokeAsync(TComponentInput input, UiFlowScreenModel screenModel)
		{
			IViewComponentResult result;

			if (input == null)
			{
				input = new TComponentInput();
			}
			if (!(PlatformSettings?.IsDeferredComponentLoadEnabled ?? false) || !DeferComponentLoad)
			{
				var data = await ResolveComponentDataAsync(input, screenModel);
				data.ScreenModel = screenModel;
				data.ComponentId = $"flow_component{Guid.NewGuid()}";
				result= await ResolveResultAsync(data);
			}
			else
			{
				result = await ResolveLoaderResultAsync(input, screenModel);
			}
			return result;
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