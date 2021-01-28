using System;
using System.Collections.Generic;
using System.Text;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System.Identity;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Infrastructure
{
	public static class UserSessionExtensions
	{
		public static bool IsCustomer(this IUserSessionProvider source)
		{
			return !source.IsAnonymous() &&
			       source.CurrentUserClaimsPrincipal.IsInAnyRole(ResidentialPortalUserRole.OnlineUser,ResidentialPortalUserRole.OnlineUserRoi);
		}

	
		public static bool IsAgentOrAdmin(this IUserSessionProvider source)
		{
			return !source.IsAnonymous() &&
			       source.CurrentUserClaimsPrincipal.IsInAnyRole(ResidentialPortalUserRole.AdminUser,
				       ResidentialPortalUserRole.AgentUser);
		}
	}
}
