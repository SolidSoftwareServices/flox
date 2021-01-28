using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
namespace Ei.Rp.Mvc.Core.Views
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public static class HtmlHelperRequestExtensions
	{
		
		public static string CurrentController(this IHtmlHelper htmlHelper)
		{
			return htmlHelper.ResolveRouteValue( "controller");
		}

		

		public static string CurrentAction(this IHtmlHelper htmlHelper)
		{
			return htmlHelper.ResolveRouteValue("action");
		}

		public static string CurrentArea(this IHtmlHelper htmlHelper)
		{
			return htmlHelper.ResolveDataToken("area");
		}

		public static string ResolveRouteValue(this IHtmlHelper htmlHelper, string key)
		{
			var routeValues = htmlHelper.ViewContext.RouteData.Values;

			if (routeValues.ContainsKey(key))
				return (string)routeValues[key];

			return string.Empty;
		}

		public static string ResolveDataToken(this IHtmlHelper htmlHelper, string key)
		{
			var dataTokens = htmlHelper.ViewContext.RouteData.DataTokens;

			if (dataTokens.ContainsKey(key))
				return (string)dataTokens[key];

			return string.Empty;
		}
	}
}