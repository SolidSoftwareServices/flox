using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Billing;

namespace EI.RP.DomainServices.Queries.Billing.Info
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<GeneralBillingInfo> GetAccountBillingInfoByAccountNumber(
			this IDomainQueryResolver provider, string accountNumber, bool byPassPipeline = false)
		{
			var query = new GeneralBillingInfoQuery
			{
				AccountNumber = accountNumber
			};
			var result = await provider
				.FetchAsync<GeneralBillingInfoQuery, GeneralBillingInfo>(query, byPassPipeline);
			return result.SingleOrDefault();
		}
	}
}