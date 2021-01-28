using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Billing;

namespace EI.RP.DomainServices.Queries.Billing.LatestBill
{
	/// <summary>
	///     simplify Domain queries here
	/// </summary>
	public static class SugarSyntaxExtensions
	{
		public static async Task<LatestBillInfo> GetLatestBillByAccountNumber(
			this IDomainQueryResolver provider, string accountNumber, bool byPassPipeline = false)
		{
			var query = new LatestBillQuery
			{
				AccountNumber = accountNumber
			};
			var result = await provider
				.FetchAsync<LatestBillQuery, LatestBillInfo>(query, byPassPipeline);
			return result.SingleOrDefault();
		}
	}
}