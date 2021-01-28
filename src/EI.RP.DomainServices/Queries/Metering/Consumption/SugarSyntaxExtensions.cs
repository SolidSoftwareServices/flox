using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering.Consumption;

namespace EI.RP.DomainServices.Queries.Metering.Consumption
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<AccountConsumption> GetAccountConsumption(this IDomainQueryResolver provider,
			string accountNumber, TimePeriodAggregationType aggregationType, DateTimeRange range,
			ConsumptionDataRetrievalType retrievalType,
			bool fillResultWithZeroes = false, bool byPassPipeline = false)
		{
            var query = new AccountConsumptionQuery
			{
				AccountNumber = accountNumber,
				AggregationType = aggregationType,
				Range = range,
				FillResultWithZeroes = fillResultWithZeroes,
				RetrievalType = retrievalType
			};

			return (await provider
				.FetchAsync<AccountConsumptionQuery, AccountConsumption>(query, byPassPipeline)).Single();
		}
	}
}