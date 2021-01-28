using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EI.RP.UiFlows.Mvc.FlowTagHelpers.Action
{
	public interface IFlowButtonTagHelperStrategy
	{
		IEnumerable<FlowActionTagHelper.FlowActionType> StrategyFor { get; }

		Task Process(FlowActionTagHelper target, IHtmlHelper htmlHelper, TagHelperContext context,
			TagHelperOutput output);
	}
}