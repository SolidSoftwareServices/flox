using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using S3.App.Flows.AppFlows;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Mvc.Views;

namespace S3.App.Infrastructure
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
