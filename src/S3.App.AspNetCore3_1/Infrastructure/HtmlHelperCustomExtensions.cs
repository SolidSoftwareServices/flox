using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using S3.App.AspNetCore3_1.Flows.AppFlows;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Mvc.Views;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace S3.App.AspNetCore3_1.Infrastructure
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
