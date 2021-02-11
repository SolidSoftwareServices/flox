using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace S3.UiFlows.Mvc.FlowTagHelpers.Action
{

	internal class FlowFormEvent : IFlowButtonTagHelperStrategy
	{
		public IEnumerable<FlowActionTagHelper.FlowActionType> StrategyFor { get; } =
			new[] {FlowActionTagHelper.FlowActionType.TriggerEvent};

		public async Task Process(FlowActionTagHelper target, IHtmlHelper htmlHelper, TagHelperContext context,
			TagHelperOutput output)
		{
			foreach (var attribute in context.AllAttributes)
			{
				output.Attributes.Add(attribute);
			}

		
			switch (target.SerializeAsTagType)
			{
				case FlowActionTagHelper.TagType.NotRelevantToThis:
				case FlowActionTagHelper.TagType.Button:
					output.TagName = "button";
					output.Attributes.SetAttribute("name", SharedSymbol.FlowEventFormFieldName);
					output.Attributes.SetAttribute("value", (string)target.TriggerEvent);
					break;
				case FlowActionTagHelper.TagType.Anchor:
					output.PreElement.AppendHtml($"<input type=\"hidden\" name=\"{SharedSymbol.FlowEventFormFieldName}\" value=\"\" />");
					output.TagName = "a";
					output.Attributes.SetAttribute("data-event-field-name", SharedSymbol.FlowEventFormFieldName);
					output.Attributes.SetAttribute("data-trigger-event", target.TriggerEvent);
					output.Attributes.SetAttribute("href", "#");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
		}
	}
}

