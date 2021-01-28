using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Billing;

namespace EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<EqualizerPaymentSetupInfo> GetEqualiserSetUpInfo(
			this IDomainQueryResolver provider, string accountNumber, DateTime? firstPaymentDateTime = null,
			bool byPassPipeline = false)
		{
			var query = new EqualizerPaymentSetupInfoQuery()
			{
				AccountNumber = accountNumber
			};
			if (firstPaymentDateTime.HasValue)
			{
				query.FirstPaymentDateTime = firstPaymentDateTime.Value;
			}

			var result = await provider
				.FetchAsync<EqualizerPaymentSetupInfoQuery, EqualizerPaymentSetupInfo>(query, byPassPipeline);
			return result.SingleOrDefault();
		}
	}
}
