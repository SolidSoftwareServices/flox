using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.WebApp.Flows.AppFlows;
using Microsoft.AspNetCore.Mvc.Rendering;
using  EI.RP.UiFlows.Mvc.Views;
namespace EI.RP.WebApp.Infrastructure.Extensions
{
	public static class HtmlHelperCustomExtensions
	{
		
		public static Task<MvcForm> BeginUiFlowFormAsync(this IHtmlHelper html,
			IUiFlowScreenModel model, string htmlId = "", object queryString = null, string className = "",
			ScreenEvent defaultEvent = null)
		{
			return html.BeginUiFlowFormAsync<ResidentialPortalFlowType>(model, htmlId, queryString, className,
					defaultEvent);

		}
	}
}