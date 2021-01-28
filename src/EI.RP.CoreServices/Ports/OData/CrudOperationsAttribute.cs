using System;

namespace EI.RP.CoreServices.Ports.OData
{
	public class CrudOperationsAttribute : Attribute
	{
		public bool CanAdd { get; set; } = false;
		public bool CanUpdate { get; set; } = false;
		public bool CanDelete { get; set; } = false;
		public bool CanQuery { get; set; } = true;
	}

	public class SortableAttribute : Attribute { }
	public class FilterableAttribute : Attribute { }
}