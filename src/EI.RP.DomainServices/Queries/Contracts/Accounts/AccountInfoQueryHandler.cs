using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts.Resolvers;
using NLog;

namespace EI.RP.DomainServices.Queries.Contracts.Accounts
{

	
	internal class AccountInfoQueryHandler : QueryHandler<AccountInfoQuery>
	{

		private readonly IEnumerable<IAccountInfoQueryResolver> _resolvers;


		public AccountInfoQueryHandler(IEnumerable<IAccountInfoQueryResolver> resolvers)
		{
			_resolvers = resolvers;
		}

		protected override Type[] ValidQueryResultTypes { get; } = TypesFinder.Resolver
			.FindTypes(x => typeof(AccountBase).IsAssignableFrom(x),
				cacheQueryId: $"{nameof(AccountInfoQueryHandler)}_QueryTypes",
				includePublicOnly: true).ToArray();

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			AccountInfoQuery query)
		{
			
			var resolver = _resolvers.SingleOrDefault(x => x.StrategyFor == typeof(TQueryResult));
			if (resolver == null)
				throw new ArgumentException($"Could not find a resolver for {typeof(TQueryResult).FullName}");

			var results = await resolver.ResolveAsAsync<TQueryResult>(query);
			return results.ToArray();
		}
	}
}