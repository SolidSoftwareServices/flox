using EI.RP.CoreServices.Http.Session;

namespace EI.RP.CoreServices.Caching
{
	public static class CacheProviderSessionExtensions
	{


		public static string ResolveUserBasedCacheKeyContext(this IUserSessionProvider userSessionProvider)
		{
			return !userSessionProvider.IsAnonymous()
				? userSessionProvider.ActingAsUserName ?? userSessionProvider.UserName
				: null;
		}
	}
}