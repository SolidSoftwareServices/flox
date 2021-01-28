using System.Collections.Generic;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering.Consumption;

namespace EI.RP.DomainServices.Queries.Metering.Consumption.Services
{
	internal interface IConsumptionResolver
	{
		Task<IEnumerable<(AccountConsumption.CostEntry cost,AccountConsumption.UsageEntry usage)>> ResolveCostsAndUsage(AccountConsumptionQuery queryModel, AccountInfo accountInfo);
		bool ResolverFor(ConsumptionDataRetrievalType aggregationType);
	}
}