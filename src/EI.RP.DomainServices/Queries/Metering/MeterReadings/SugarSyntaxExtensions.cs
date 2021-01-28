using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Metering;

namespace EI.RP.DomainServices.Queries.Metering.MeterReadings
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<IEnumerable<MeterReadingInfo>> GetMeterReadings(this IDomainQueryResolver provider,
			string accountNumber, bool byPassPipeline = false)
		{
			var query = new MeterReadingsQuery
			{
				AccountNumber = accountNumber
			};

			var meterReadingInfo = await provider
				.FetchAsync<MeterReadingsQuery, MeterReadingInfo>(query, byPassPipeline);

			return meterReadingInfo;
		}
	}
}