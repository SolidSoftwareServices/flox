using System.Collections.Generic;

namespace EI.RP.CoreServices.System.Paging
{
	public class PagedData<T>
	{

		public int PageSize { get; set; }
		public T[] CurrentPageItems { get; set; }=new T[0];
		public int PageIndex { get; set; }

		public int TotalPages { get; set; }
		public IEnumerable<T> Original { get; set; }
	}
}