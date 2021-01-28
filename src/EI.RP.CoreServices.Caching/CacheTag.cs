using System;

namespace EI.RP.CoreServices.Caching
{
	[Obsolete("remove")]
	internal static class CacheTag
	{
		public const string CachePrefix = "_|_";
		public const string GlobalKeyContext = "_g_";
		public const string Separator = "|";
	}
}