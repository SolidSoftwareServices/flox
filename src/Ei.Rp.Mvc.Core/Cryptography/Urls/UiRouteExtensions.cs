using System;
using System.Collections;
using System.Collections.Generic;

using System.Dynamic;
using System.Linq;
using System.Text;
using EI.RP.CoreServices.Encryption;
using Ei.Rp.Mvc.Core.Controllers;
using Ei.Rp.Mvc.Core.System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
#if !FrameworkDeveloper
using System.Diagnostics;
#endif

namespace Ei.Rp.Mvc.Core.Cryptography.Urls
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public static class UrlRouteExtensions
	{
		
		

		public static async Task<string> GetActionUrlAsync(this IHtmlHelper htmlHelper, 
			string actionName, string controllerName, object routeValues = null, object htmlAttributes = null)
		{
			var httpContext = htmlHelper.ViewContext.HttpContext;
			var url = new StringBuilder(httpContext.Request.PathBase.ToString());
			
			if (controllerName != string.Empty)
			{
				url.Append($"/{controllerName}");
			}

			url.Append($"/{actionName}");

			var queryString = await ResolveEncryptedQueryString(httpContext.Resolve<IEncryptionService>());
			if (queryString != string.Empty)
			{

				url.Append(queryString);
			}

			return url.ToString();

			async  Task<string> ResolveEncryptedQueryString(IEncryptionService encryptionService)
			{
				var result = await httpContext._ResolveEncryptedQueryStringParameterValueAsync(routeValues, encryptionService);

				if (!string.IsNullOrWhiteSpace(result))
				{
					result = ResolveEncryptionSettings(httpContext)?.EncryptUrls??false
						? $"?{EncryptionConstants.QueryStringArgumentName}={result}" 
						: $"?{result}";
				}

				return result;
			}
		}

		
        public static async Task<object> ToValidRouteValueAsync(this object routeValues, HttpContext httpContext)
		{
			var val = await httpContext._ResolveEncryptedQueryStringParameterValueAsync(routeValues, httpContext.Resolve<IEncryptionService>());
			var result = (IDictionary<string,object>)new ExpandoObject();
			if (!string.IsNullOrWhiteSpace(val))
			{
				if (ResolveEncryptionSettings(httpContext)?.EncryptUrls??false)
				{
					result.Add(EncryptionConstants.QueryStringArgumentName, val);
				}
				else
				{
					result = val.Split('&').Select(x => x.Split('=')).ToDictionary(x => x[0], x => (object) x[1]);
				}
			}


			return (dynamic)result;
		}
		

		public static async Task<IHtmlContent> GetActionLinkAsync(this IHtmlHelper htmlHelper, string linkText,
	        string actionName, Type controllerType, object routeValues = null, object htmlAttributes = null)
        {

	        if (controllerType != null && !typeof(Controller).IsAssignableFrom(controllerType)) throw new ArgumentException("Not a valid controller");

	        return await htmlHelper.GetActionLinkAsync(linkText, actionName, controllerType.GetNameWithoutSuffix(),
		        routeValues, htmlAttributes);
        }
		public static async Task<IHtmlContent> GetActionLinkAsync(this IHtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null)
        {
            var a = new TagBuilder("a");
            ResolveHtmlAttributesString(a);

            a.MergeAttribute("href", await htmlHelper.GetActionUrlAsync(actionName, controllerName, routeValues: routeValues, htmlAttributes: htmlAttributes));
            a.InnerHtml.AppendHtml(linkText);

            return a;

            void ResolveHtmlAttributesString(TagBuilder tagBuilder)
            {
                if (htmlAttributes == null)
                {
                    return;
                }

                foreach (var (attributeKey, attributeValue) in new RouteValueDictionary(htmlAttributes))
                {
					if (attributeKey == "role" && (attributeValue == null || attributeValue is string value && string.IsNullOrWhiteSpace(value))) continue;

                    tagBuilder.MergeAttribute(
                        attributeKey.ToLowerInvariant().StartsWith("data_") ? attributeKey.Replace('_', '-') : attributeKey,
                        attributeValue?.ToString());
                }
            }
        }
        private static IUrlEncryptionSettings ResolveEncryptionSettings(HttpContext httpContext)
        {
	        return httpContext.Resolve<IUrlEncryptionSettings>(false);
        }


        private static async Task<string> _ResolveEncryptedQueryStringParameterValueAsync(this HttpContext context, object routeValues,
	        IEncryptionService encryptionService)
        {
	        var sb = new StringBuilder();
	        if (routeValues != null)
	        {
		        var d = new RouteValueDictionary(routeValues);
		        for (var i = 0; i < d.Keys.Count; i++)
		        {
			        var value = d.Values.ElementAt(i);
			        if (value != null && (value is string || !(value is IEnumerable)) )
			        {
				        if (sb.Length!=0)
				        {
					        sb.Append("&");
				        }


				        sb.Append($"{d.Keys.ElementAt(i)}={value}");
			        }
		        }
	        }

	        var result = string.Empty;
	        if (sb.Length > 0 && (ResolveEncryptionSettings(context)?.EncryptUrls ?? false))
	        {
		        result = Uri.EscapeDataString(await encryptionService.EncryptAsync(sb.ToString(), true));
	        }
	        else
	        {
		        result = sb.ToString();
	        }

	        return result;
        }


	}
}
