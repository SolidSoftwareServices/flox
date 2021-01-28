using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels;
using Ei.Rp.DomainModels.Billing;

namespace EI.RP.DomainServices.Queries.Billing.NextBill
{
	/// <summary>
	///     simplify Domain queries here
	/// </summary>
	public static class SugarSyntaxExtensions
	{

		public static async Task<NextBillEstimation> GetNextBillEstimationByAccountNumber(
			this IDomainQueryResolver provider, string accountNumber, bool byPassPipeline = false)
		{
			var query = new EstimateNextBillQuery
			{
				AccountNumber = accountNumber
			};
			var result = await provider
				.FetchAsync<EstimateNextBillQuery, NextBillEstimation>(query,byPassPipeline);
			return result.SingleOrDefault();
		}
	
	}
}