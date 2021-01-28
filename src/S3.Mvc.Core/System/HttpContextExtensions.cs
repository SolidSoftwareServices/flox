using System;
using System.Linq;
using System.Threading.Tasks;
using S3.CoreServices.System;
using S3.CoreServices.System.DependencyInjection;
using S3.Mvc.Core.Cryptography.Urls;
using Microsoft.AspNetCore.Http;

namespace S3.Mvc.Core.System
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

		private static string _baseUrl = null;
        public static string GetBaseUrl(this HttpContext src)
        {
	        if (_baseUrl == null)
	        {
		        var request = src.Request;

		        var host = request.Host.ToUriComponent();

		        var pathBase = request.PathBase.ToUriComponent();

		        _baseUrl = $"{request.Scheme}://{host}{pathBase}/";
	        }

	        return _baseUrl;
        }
    }
}