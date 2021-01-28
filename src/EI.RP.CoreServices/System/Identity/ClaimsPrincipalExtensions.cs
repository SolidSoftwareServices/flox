using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace EI.RP.CoreServices.System.Identity
{
	public static class ClaimsPrincipalExtensions
	{
		public static bool IsInAnyRole(this ClaimsPrincipal target, params string[] roles)
		{
			return roles.Any(target.IsInRole);
		}
	}
}
