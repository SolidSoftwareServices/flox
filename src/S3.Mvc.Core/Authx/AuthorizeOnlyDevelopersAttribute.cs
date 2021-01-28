using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace S3.Mvc.Core.Authx
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class AuthorizeOnlyDevelopersAttribute : AuthorizeAttribute, IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			if (!Debugger.IsAttached) context.Result = new UnauthorizedResult();
		}
	}
}