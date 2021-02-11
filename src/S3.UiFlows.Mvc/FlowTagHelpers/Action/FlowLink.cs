using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using S3.CoreServices.System;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Mvc.Components;
using S3.UiFlows.Mvc.Views;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using S3.UiFlows.Core.DataSources;


namespace S3.UiFlows.Mvc.FlowTagHelpers.Action
{

	class FlowLink : IFlowButtonTagHelperStrategy
	{
		private readonly IUiFlowContextRepository _contextRepository;

		public FlowLink(IUiFlowContextRepository contextRepository)
		{
			_contextRepository = contextRepository;
		}

		public IEnumerable<FlowActionTagHelper.FlowActionType> StrategyFor { get; } = new[] { FlowActionTagHelper.FlowActionType.StartFlow,FlowActionTagHelper.FlowActionType.ReloadStepWithChanges };
		public async Task Process(FlowActionTagHelper target, IHtmlHelper htmlHelper, TagHelperContext context,
			TagHelperOutput output)
		{
		
			var content = (await output.GetChildContentAsync()).GetContent();
			
			IHtmlContent htmlContent;

            var attrsObject = (object)context.AllAttributes
                .Where(x => !FlowActionTagHelper.DesignerOnlySymbols.Any(att => att.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase)))
                .ToDictionary(x => x.Name, x => x.Value).ToExpandoObject();

			if (target.Type == FlowActionTagHelper.FlowActionType.StartFlow)
			{
				var containerViewModel = await LocateContainer();
				
				htmlContent = await htmlHelper.DisplayFlowLinkAsync2(containerViewModel, content, target.FlowName,
					target.FlowParameters, attrsObject);
			}
			else if (target.Type == FlowActionTagHelper.FlowActionType.ReloadStepWithChanges)
			{
				var uiFlowScreenModel = htmlHelper.ViewData.Model is FlowComponentViewModel model
					? (UiFlowScreenModel) model.ScreenModel
					: (UiFlowScreenModel) htmlHelper.ViewData.Model;
				htmlContent = await htmlHelper.UiFlowActionLinkToCurrentStepAsync(uiFlowScreenModel, 
                    target.FlowParameters, content, attrsObject);
			}
			else
			{
				throw new ArgumentOutOfRangeException();
			}
			output.TagName = null;
			output.Content.SetHtmlContent(htmlContent);
			async Task<UiFlowScreenModel> LocateContainer()
			{
				UiFlowScreenModel result = null;
				var currentHandler = (htmlHelper.ViewData.Model as UiFlowScreenModel)?.FlowHandler ??
				                     (htmlHelper.ViewData.Model as FlowComponentViewModel)?.ScreenModel?.FlowHandler;

				switch (target.FlowLocation)
				{

					case FlowActionTagHelper.StartFlowLocation.NotRelevantToThis:
					case  FlowActionTagHelper.StartFlowLocation.NotContained:
						break;
					case FlowActionTagHelper.StartFlowLocation.SameContainerAsMe:
						if (currentHandler != null)
						{
							var ctx = await _contextRepository.Get(currentHandler);
							if (ctx.ContainerFlowHandler != null)
							{
								ctx = await _contextRepository.Get(ctx.ContainerFlowHandler);
								result = ctx.GetCurrentStepData<UiFlowScreenModel>();
							}
						}

						break;
					case FlowActionTagHelper.StartFlowLocation.ContainedInMe:
						if (currentHandler != null)
						{
							var ctx = await _contextRepository.Get(currentHandler);
							result = ctx.GetCurrentStepData<UiFlowScreenModel>();
						}

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				return result;
			
			}
		}

	}


}