using System.Linq;
using System.Security.Claims;

namespace S3.CoreServices.System.Identity
{
	public static class ClaimsPrincipalExtensions
	{
		public static bool IsInAnyRole(this ClaimsPrincipal target, params string[] roles)
		{
			return roles.Any(target.IsInRole);
		}
	}
}
