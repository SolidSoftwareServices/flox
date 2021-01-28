using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators;
using NLog;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation
{
	internal class MovingHouseValidationQueryHandler : QueryHandler<MovingHouseValidationQuery>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IUserSessionProvider _sessionProvider;
		private readonly ICacheProvider _cacheProvider;
		private readonly IEnumerable<IMovingHouseValidator> _movingHouseValidators;

		public MovingHouseValidationQueryHandler(IUserSessionProvider sessionProvider, ICacheProvider cacheProvider, IEnumerable<IMovingHouseValidator> movingHouseValidators)
		{
			_sessionProvider = sessionProvider;
			_cacheProvider = cacheProvider;
			_movingHouseValidators = movingHouseValidators;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(MovingHouseRulesValidationResult)};

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			MovingHouseValidationQuery query)
		{
			IEnumerable<MovingHouseRulesValidationResult> results;
			if (!query.IsValidQuery(out var nothing))
			{
				results =new MovingHouseRulesValidationResult()
				{
					MovingHouseValidationType = MovingHouseValidationType.QueryValidation,
					Output = OutputType.Failed
				}.ToOneItemArray();

			}
			else
			{
				results = await Task.WhenAll(_movingHouseValidators.Select(async x =>
				{
					var ruleKey = x.ResolveCacheKey(query);
					MovingHouseRulesValidationResult result;

					if (ruleKey == null)
					{
						//null == is not cacheable
						result = await x.Resolve(query);
					}
					else
					{
						result = await _cacheProvider.GetOrAddAsync($"{x.GetType().Name}-{ruleKey}",
							() => x.Resolve(query),
							_sessionProvider.ResolveUserBasedCacheKeyContext());
					}

					return result;

				}).ToArray());
			}

			Logger.Info(() => string.Join(Environment.NewLine, results.Select(x => $"{nameof(MovingHouseValidationQueryHandler)}. Rule={x.MovingHouseValidationType} Result={x.Output}")));

            return results.Cast<TQueryResult>();
        }
	}
}