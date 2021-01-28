using System;
using EI.RP.CoreServices.DeliveryPipeline.Environments;
using Ei.Rp.Mvc.Core.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ei.Rp.Mvc.Core.Authx
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class AuthorizedOnlyDuringDevelopmentAttribute : AuthorizeAttribute, IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			if (context.HttpContext.Resolve<IHostingEnvironment>().IsOneOfTheReleaseEnvironments()) context.Result = new UnauthorizedResult();
		}
	}
}