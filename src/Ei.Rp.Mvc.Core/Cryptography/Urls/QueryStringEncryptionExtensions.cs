using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Autofac.Core;
using EI.RP.CoreServices.Encryption;
using Ei.Rp.Mvc.Core.Controllers;
using Ei.Rp.Mvc.Core.System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Ei.Rp.Mvc.Core.Cryptography.Urls
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public static class QueryStringEncryptionExtensions
	{
		
        public static async Task<string> EncryptedActionUrl(this HttpContext httpContext,
            string actionName, string controllerName, object routeValues = null, object htmlAttributes = null)
        {
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

            async Task<string > ResolveEncryptedQueryString(IEncryptionService encryptionService)
            {
                var result =await  httpContext._ResolveEncryptedQueryStringParameterValueAsync(routeValues, encryptionService);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    result = ResolveEncryptionSettings(httpContext)?.EncryptUrls ?? false
                        ? $"?{EncryptionConstants.QueryStringArgumentName}={result}"
                        : $"?{result}";
                }

                return result;
            }
        }

        public static async Task<string> EncryptedActionUrl(this IHtmlHelper htmlHelper, 
			string actionName, string controllerName, object routeValues = null, object htmlAttributes = null)
		{
			return await htmlHelper.ViewContext.HttpContext.EncryptedActionUrl(actionName,controllerName,routeValues,htmlAttributes);
			
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
					if (i > 0)
					{
						sb.Append("&");
					}

					sb.Append($"{d.Keys.ElementAt(i)}={d.Values.ElementAt(i)}");
				}
			}

			var result = string.Empty;
			if (sb.Length > 0 && (ResolveEncryptionSettings(context)?.EncryptUrls??false))
			{
				result = Uri.EscapeDataString(await encryptionService.EncryptAsync(sb.ToString(), true));
			}
			else
			{
				result = sb.ToString();
			}

			return result;
		}
      
	

        public static async Task<object> ToEncryptedRouteValueAsync(this object routeValues, HttpContext httpContext)
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
	}
}
