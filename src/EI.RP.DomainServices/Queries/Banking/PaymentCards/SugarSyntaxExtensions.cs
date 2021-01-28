using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels;
using Ei.Rp.DomainModels.Banking;

namespace EI.RP.DomainServices.Queries.Banking.PaymentCards
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<IEnumerable<PaymentCardInfo>> GetPaymentCardsInfo(this IDomainQueryResolver provider,
			string partner, bool byPassPipeline = false)
		{
			var query = new PaymentCardsQuery
			{
				Partner = partner
			};

			return await provider
				.FetchAsync<PaymentCardsQuery, PaymentCardInfo>(query,byPassPipeline);
		}

	}
}