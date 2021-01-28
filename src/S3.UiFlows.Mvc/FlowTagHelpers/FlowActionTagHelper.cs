using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Mvc.FlowTagHelpers.Action;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
#if !FrameworkDeveloper
#endif

namespace S3.UiFlows.Mvc.FlowTagHelpers
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	[HtmlTargetElement(TagName)]
	public class FlowActionTagHelper : TagHelper
	{
		internal const string TagName = "flow-action";
		internal const string EventAttrName = "trigger-event";
		internal const string TypeAttrName = "action-type";
		internal const string FlowAttrName = "flow-name";
		internal const string LocationAttrName="flow-location";
		internal const string RouteValuesAttrName = "flow-parameters";

		internal const string TagTypeAttrName = "tag-type";

		internal static readonly string[] DesignerOnlySymbols =
		{
			TagName, EventAttrName,TypeAttrName,FlowAttrName,LocationAttrName,RouteValuesAttrName,TagTypeAttrName
		};

		private readonly IEnumerable<IFlowButtonTagHelperStrategy> _strategies;
		private readonly IHtmlHelper _htmlHelper;

		public FlowActionTagHelper(IHtmlHelper htmlHelper, IEnumerable<IFlowButtonTagHelperStrategy> strategies)
		{
			_strategies = strategies;
			_htmlHelper = htmlHelper;
		}


		[ViewContext]
		[HtmlAttributeNotBound]
		public ViewContext ViewContext { get; set; }
		[HtmlAttributeName(TypeAttrName)] public FlowActionType Type { get; set; } = FlowActionType.TriggerEvent;


		[HtmlAttributeName(EventAttrName)] public ScreenEvent TriggerEvent { get; set; }

		[HtmlAttributeName(FlowAttrName)] public string FlowName { get; set; }
		[HtmlAttributeName(RouteValuesAttrName)] public object FlowParameters { get; set; }
		[HtmlAttributeName(LocationAttrName)] public StartFlowLocation FlowLocation { get; set; }=StartFlowLocation.NotRelevantToThis;
		[HtmlAttributeName(TagTypeAttrName)] public TagType SerializeAsTagType { get; set; } = TagType.NotRelevantToThis;
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			if (output.TagName == TagName)
			{ 
				var attributes = context.AllAttributes.Where(x => DesignerOnlySymbols.Any(att => att.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))).ToArray();
				

				((IViewContextAware)_htmlHelper).Contextualize(ViewContext);

				await _strategies.Single(x => x.StrategyFor.Any(y => y == Type)).Process( this, _htmlHelper, context, output);
				foreach (var attribute in attributes)
				{
					output.Attributes.Remove(attribute);
				}
			}
		}

		public enum FlowActionType
		{
			 NoAction=0,

			/// <summary>
			/// Button that Triggers an event on the current flow
			/// </summary>
			TriggerEvent,

			/// <summary>
			/// anchor that starts a flow
			/// </summary>
			StartFlow,

			/// <summary>
			/// Reload the step with the parameters provided
			/// </summary>
			ReloadStepWithChanges
		}

		public enum StartFlowLocation
		{
			NotRelevantToThis = 0,
			NotContained ,
			SameContainerAsMe,
			ContainedInMe
		}

		public enum TagType
		{
			NotRelevantToThis=0,
			Button,
			Anchor
		}
	}
}
