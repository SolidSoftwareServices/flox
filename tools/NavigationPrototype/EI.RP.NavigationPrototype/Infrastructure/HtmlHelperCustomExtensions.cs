using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Views;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EI.RP.NavigationPrototype.Infrastructure
{
	public static class HtmlHelperCustomExtensions
	{
		
		public static Task<MvcForm> BeginUiFlowFormAsync(this IHtmlHelper html,
			IUiFlowScreenModel model, string htmlId = "", object queryString = null, string className = "",
			ScreenEvent defaultEvent = null)
		{
			return html.BeginUiFlowFormAsync<SampleAppFlowType>(model, htmlId, queryString, className,
				defaultEvent);

		}
	}
}
