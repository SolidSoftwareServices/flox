using System;
using System.Linq;
using System.Net.Http;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Profiling;
using Ei.Rp.Mvc.Core.System;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ei.Rp.Mvc.Core.Cryptography.AntiTampering
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ValidateHiddenInputsAntiTamperingAttribute : ActionFilterAttribute
	{
		private static readonly HttpMethod[] HttpMethods = { HttpMethod.Post, HttpMethod.Put, HttpMethod.Patch };

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var httpContext = context.HttpContext;
			var profiler = httpContext.Resolve<IProfiler>();
			using (profiler.Profile("Attributes", nameof(ValidateHiddenInputsAntiTamperingAttribute)))
			{
				var httpMethod = new HttpMethod(httpContext.Request.Method);

				if (HttpMethods.Any(x => x.Equals(httpMethod)))
				{
					var encryptionService = context.HttpContext.Resolve<IEncryptionService>();
					var protectedProperties = context.HttpContext.Request.Form.Keys
						.Where(x => x.StartsWith(EncryptionConstants.TokenPrefixAntiTampering))
						.Select(x =>
							(originalName: x.Substring(EncryptionConstants.TokenPrefixAntiTampering.Length), protectedName: x));

					foreach (var protectedProperty in protectedProperties)
					{
						await ValidateAntiTamperingPropertyAsync(context, protectedProperty.originalName, protectedProperty.protectedName,
							encryptionService);
					}
				}
			}
			await base.OnActionExecutionAsync(context, next);
		}

		private async Task ValidateAntiTamperingPropertyAsync(ActionExecutingContext context, string originalName, string protectedName,
			IEncryptionService encryptionService)
		{
			var httpContext = context.HttpContext;
			var httpContextRequest = httpContext.Request;
			var decryptedValue = await encryptionService.DecryptAsync(httpContextRequest.Form[protectedName],true);
			if (decryptedValue == null)
			{
				throw new HiddenInputTamperedException("A required security token was not supplied or was invalid.");
			}

			var originalValue = httpContextRequest.Form[originalName];


			if (!decryptedValue.ToLower().Equals(originalValue))
			{
				throw new HiddenInputTamperedException("A required security token was not supplied or was invalid.");
			}
		}

	}
}