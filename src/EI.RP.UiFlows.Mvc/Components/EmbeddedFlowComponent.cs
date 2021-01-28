using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.Mvc.Core.System.Request;
using EI.RP.UiFlows.Core.Facade.CurrentView;
using EI.RP.UiFlows.Core.Flows.Screens.Metadata;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EI.RP.UiFlows.Mvc.Components
{
	[ViewComponent(Name="EmbeddedFlow")]
	public class EmbeddedFlowComponent : ViewComponent
	{
		private readonly IFlowCurrentViewRequestHandler<IViewComponentResult> _currentViewResolver;

		public EmbeddedFlowComponent(IFlowCurrentViewRequestHandler<IViewComponentResult> currentViewResolver)
		{
			_currentViewResolver = currentViewResolver;
		}

		public async Task<IViewComponentResult> InvokeAsync(ComponentParameters input)
		{
			
			return await ResolveEmbedded(input.Model.Metadata);
		}

		private async Task<IViewComponentResult> ResolveEmbedded(IScreenMetaData model,
			object routeValues = null)
		{
			var currentRequestQueryString = Request.Query.ToExpandoObject();

			return await WhenInsideStepData();

		
			async Task<IViewComponentResult> WhenInsideStepData()
			{
				
				IViewComponentResult result;
				if (string.IsNullOrEmpty(model.ContainedFlowType))
				{
					result = new HtmlContentViewComponentResult(new StringHtmlContent(string.Empty));
				}
				else if (string.IsNullOrEmpty(model.ContainedFlowHandler))
				{
					throw new ApplicationException($"property {nameof(IScreenMetaData.ContainedFlowHandler)} should contain a value when there is a contained flow");
				}
				else
				{
					var stepViewCustomizations = new
							{ContainerModelProperty_FlowHandler = model.ContainedFlowHandler}
						.MergeObjects(currentRequestQueryString).ToExpandoObject();

					result = await _currentViewResolver.Execute(new CurrentViewRequest<IViewComponentResult>
						{
							FlowHandler = model.ContainedFlowHandler,
							
							BuildResultOnFlowNotFound = () =>
								Task.FromResult<IViewComponentResult>(
									new HtmlContentViewComponentResult(new StringHtmlContent("Not authorized"))),
							ViewParameters = stepViewCustomizations,
							OnAddModelError = (memberName, errorMessage) => ModelState.AddModelError(memberName, errorMessage),
						OnBuildView = (bvi =>
							{
								var viewPath = $"{bvi.FlowType}/Views/{bvi.ViewName}";
								return Task.FromResult<IViewComponentResult>(View(viewPath, bvi.ScreenModel));
							}),
						ResolveAsComponentOnly = true
						});
				}

				return result;
			}
		}

		public class ComponentParameters
		{
			public UiFlowScreenModel Model { get; set; }
			public object RouteValues { get; set; }
		}
	}
}