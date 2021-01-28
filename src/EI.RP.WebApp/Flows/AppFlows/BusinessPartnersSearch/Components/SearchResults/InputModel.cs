using System.Collections.Generic;
using Ei.Rp.DomainModels.Contracts;

namespace EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.Components.SearchResults
{
	
	public class InputModel
	{
		public IEnumerable<string> BusinessPartnersIdToShow { get; set; }
		public int NumberPagingLinks { get; set; } = 5;
		public bool IsPagingEnabled { get; set; } = true;
		public int PageSize { get; set; } = int.MaxValue;
		public int PageIndex { get; set; } = 0;
		public string QueryUserName { get; set; }
	}
}