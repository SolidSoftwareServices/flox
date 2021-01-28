using System;
using System.Collections.Generic;
using System.Linq;
using S3.CoreServices.Encryption;
using S3.Mvc.Core.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using S3.CoreServices.Profiling;
using S3.CoreServices.System;
using S3.CoreServices.System.FastReflection;

namespace S3.Mvc.Core.Cryptography.Urls
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	[AttributeUsage(AttributeTargets.Class)]
	public class SupportEncryptedUrlsAttribute : ActionFilterAttribute
	{
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var httpContext = context.HttpContext;
			var profiler = httpContext.Resolve<IProfiler>();
			using (profiler.Profile("Attributes", nameof(SupportEncryptedUrlsAttribute)))
			{

				if (httpContext.Resolve<IUrlEncryptionSettings>(false)?.EncryptUrls??false)
				{
					if (httpContext.Request.Query.Any(x => x.Key == EncryptionConstants.QueryStringArgumentName))
					{
						var original = httpContext.Request.Query[EncryptionConstants.QueryStringArgumentName];
						var decrypted = await httpContext.Resolve<IEncryptionService>()
							.DecryptAsync(Uri.UnescapeDataString(original), onlyIfHasPrefix: true);
						var paramsArrs = decrypted.Split('&');

						var parameters = new Dictionary<string, object>();
						for (var i = 0; i < paramsArrs.Length; i++)
						{
							var parts = paramsArrs[i].Split('=');
							var value = parts[1];
							if (value != null)
							{
								parameters.Add(parts[0], value);
							}
						}
						if (parameters.Any())
						{
							var values = parameters.Select(x=>$"{x.Key}={x.Value}");

							httpContext.Request.QueryString= new QueryString($"?{string.Join('&',values)}");
						}
						SetValuesFromUrl(context, parameters);

					}
				}
				else
				{


					var paramsArrs =
						httpContext.Request.Query.ToDictionary(x => x.Key, x => (object) x.Value.FirstOrDefault());
					SetValuesFromUrl(context, paramsArrs);


				}
			}
			await base.OnActionExecutionAsync(context, next);
		}


		private static void SetValuesFromUrl(ActionExecutingContext filterContext, IDictionary<string,object> queryStringParameters)
		{
			

			filterContext.HttpContext.Request.Query = new QueryCollection(queryStringParameters.ToDictionary(x=>x.Key,x=>new StringValues((string)x.Value)));


			var allKeys = queryStringParameters.Keys;


			foreach (var actionParameter in filterContext.ActionDescriptor.Parameters)
			{
				var parameterName = actionParameter.Name;
				var parameterType = actionParameter.ParameterType;
				var key = allKeys.SingleOrDefault(x =>
					x.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase));
				if (key != null)
				{
					var decryptedParameter = queryStringParameters[key];
					var val = GetValue(parameterType, decryptedParameter);
					filterContext.ActionArguments[key] = val;
				}
				else
				{
					if (filterContext.ActionArguments.ContainsKey(parameterName))
					{

						//TODO: MODEL BINDER INSTEAD
						var target = filterContext.ActionArguments[parameterName];
						//TODO: refactor performance--> possibly creting model binder
						var propertyInfos=actionParameter.ParameterType.GetPropertiesFast(
							BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public |
							BindingFlags.GetProperty | BindingFlags.SetProperty, x => allKeys.Any(y => x.Name.Equals(y)),actionParameter.ParameterType.FullName);
						foreach (var propertyInfo in propertyInfos.Where(x=>queryStringParameters.Keys.Any(k=>x.Name.Equals(k,StringComparison.InvariantCultureIgnoreCase))))
						{
							var decryptedParameter = queryStringParameters.Single(x=>propertyInfo.Name.Equals(x.Key,StringComparison.InvariantCultureIgnoreCase)).Value;
							if (decryptedParameter != null)
							{
								var val = GetValue(propertyInfo.PropertyType, decryptedParameter);
								target.SetPropertyValueFast(propertyInfo, val);
							}
						}
					}
				}
			}
		}

		private static object GetValue(Type parameterType, object decryptedParameter)
		{
			if (decryptedParameter == null) return null;
			object val;
			if (parameterType.IsEnum)
			{
				val = Enum.Parse(parameterType, decryptedParameter.ToString(), true);
			}
			else if (parameterType.IsNullable() && parameterType.GenericTypeArguments[0].IsEnum)
			{
				val = Enum.Parse(parameterType.GenericTypeArguments[0], decryptedParameter.ToString(), true);
			}
			else if (parameterType.IsNullable() && parameterType.IsNumeric(true) && decryptedParameter is string)
			{
				val = string.IsNullOrWhiteSpace(decryptedParameter.ToString())
					? null
					: Convert.ChangeType(decryptedParameter, parameterType.GenericTypeArguments[0],CultureInfo.InvariantCulture);
			}
			else
			{
				val = Convert.ChangeType(decryptedParameter, parameterType, CultureInfo.InvariantCulture);
			}

			return val;
		}
	}
}