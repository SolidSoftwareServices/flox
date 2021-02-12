using System;
using System.Collections.Generic;
using System.Linq;

namespace S3.CoreServices.System.Paging
{
	public static class PagerExtensions
	{
		public static PagedData<T> ToPagedData<T>(this IEnumerable<T> src, int pageSize, int pageIndex, bool isPagingEnabled = true)
		{
			var source = src.ToArray();
			var result = new PagedData<T>
			{
				Original= source,
				PageSize = pageSize,
				PageIndex = pageIndex,
				TotalPages = isPagingEnabled ? (int)Math.Ceiling(source.Length / (double)pageSize) : 1,
				CurrentPageItems = isPagingEnabled ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToArray() : source.ToArray()
			}.CorrectPageIndexBoundaries();
			return result;
		}

		private static PagedData<T> CorrectPageIndexBoundaries<T>(this PagedData<T> src)
		{
			if (src.PageIndex > src.TotalPages)
			{
				src.PageIndex = src.TotalPages;
			}
			else if (src.PageIndex < 1)
			{
				src.PageIndex = 1;
			}

			return src;
		}
	}
}
