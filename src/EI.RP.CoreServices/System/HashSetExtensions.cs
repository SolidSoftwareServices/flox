using System.Collections.Generic;

namespace EI.RP.CoreServices.System
{
	public static class HashSetExtensions
	{
		public static void AddRange<T>(this HashSet<T> src, IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				src.Add(item);
			}
		}
	}
}