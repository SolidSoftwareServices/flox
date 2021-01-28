using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.DependencyInjection;
using Ei.Rp.Mvc.Core.Cryptography.Urls;
using Microsoft.AspNetCore.Http;

namespace Ei.Rp.Mvc.Core.System
{
	public static class HttpContextExtensions
	{
		public static TService Resolve<TService>(this HttpContext httpContext, bool failIfNotRegistered = true)
		{
			if (httpContext == null)
				throw new ArgumentNullException(nameof(httpContext));

			return httpContext.RequestServices.Resolve<TService>();

		}


		public static async Task<object> ToEncryptedRouteValuesAsync(this HttpContext httpContext, params object[] routeValues)
		{
			var merged = routeValues[0].MergeObjects(routeValues.Any() ? routeValues.Skip(1).ToArray() : new object[0]);
			return await merged.ToValidRouteValueAsync(httpContext);
		}

        public static string GetBaseUrl(this HttpContext src)
        {
            var request = src.Request;

            var host = request.Host.ToUriComponent();

            var pathBase = request.PathBase.ToUriComponent();

            return $"{request.Scheme}://{host}{pathBase}";
        }
    }
}