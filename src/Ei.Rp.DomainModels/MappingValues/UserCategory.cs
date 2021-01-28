using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class UserCategory:TypedStringValue<UserCategory>
    {
	    [JsonConstructor]
	    private UserCategory()
	    {
	    }
		private UserCategory(string roleName, string debuggerFriendlyDisplayValue) : base(roleName,debuggerFriendlyDisplayValue)
	    {
	    }

	  
		public static readonly UserCategory SignUpUser = new UserCategory("001", nameof(SignUpUser));
	  
    }
}
