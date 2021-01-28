using System.Collections.Generic;

namespace EI.RP.DomainServices.Infrastructure.Util
{
	public static class NIHelper
	{
		public const string NIPortalUsers = "niportalusers";

		public static Dictionary<string, string> GetNIPortalUserValue()
		{
			return CustomConfigParser.LoadConfigEntriesforNIPortalUsers(NIPortalUsers);
		}
	}
}