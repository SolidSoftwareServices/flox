using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Queries.Contracts.Accounts.Resolvers
{
	interface IAccountInfoQueryResolver
	{
		Type StrategyFor { get; }

		Task<IEnumerable<TQueryResult>> ResolveAsAsync<TQueryResult>(AccountInfoQuery query);
	}
}