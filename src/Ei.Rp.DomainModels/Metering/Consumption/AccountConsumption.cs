using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;

namespace Ei.Rp.DomainModels.Metering.Consumption
{
	public partial class AccountConsumption : IQueryResult
	{
		public IEnumerable<CostEntry> CostEntries { get; set; } = new CostEntry[0];
		public IEnumerable<UsageEntry> UsageEntries { get; set; } = new UsageEntry[0];
	}

}