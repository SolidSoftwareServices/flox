using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class ResidentialPortalUserRole : TypedStringValue<ResidentialPortalUserRole>
	{
		public static readonly ResidentialPortalUserRole OnlineUser = new ResidentialPortalUserRole("UCES_REF", nameof(OnlineUser));
		public static readonly ResidentialPortalUserRole OnlineUserRoi = new ResidentialPortalUserRole("ONLINE_ROI", nameof(OnlineUserRoi));
		public static readonly ResidentialPortalUserRole AgentUser = new ResidentialPortalUserRole("UMC_AGENT", nameof(AgentUser));
		public static readonly ResidentialPortalUserRole AdminUser = new ResidentialPortalUserRole("UCES_ADMIN", nameof(AdminUser));

		[JsonConstructor]
		private ResidentialPortalUserRole()
		{
		}

		private ResidentialPortalUserRole(string roleName, string debuggerFriendlyDisplayValue) : base(roleName,debuggerFriendlyDisplayValue)
		{
		}

	
	}
}