using System.Collections.Generic;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCardsContainer
{
	public class InputModel
    {
        public ClientAccountType AccountType { get; set; } = ClientAccountType.Electricity;
		public bool IsOpen { get; set; } = true;
		public bool HasStartedFromMeterReading { get; set; }


        public int PageIndex { get; set; } = 0;
		public int PageSize { get; set; } = int.MaxValue;
		public int NumberPagingLinks { get; set; } = 4;
		public bool IsPagingEnabled { get; set; } = false;

        public string ContainerId { get; set; } = "accounts-container";
		public string PaginationId { get; set; } = "accounts-pagination";
		public string WhenChangingPageFocusOn { get; set; }
	}
}
