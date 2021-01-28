using Ei.Rp.DomainModels.Contracts;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Queries;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Metering;

namespace EI.RP.DomainServices.Queries.Contracts.PointOfDelivery
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<PointOfDeliveryInfo> GetPointOfDeliveryInfoByPrn(this IDomainQueryResolver provider,
			PointReferenceNumber prn,
			bool byPassPipeline = false)
		{
			var query = new PointOfDeliveryQuery
			{
				Prn = prn,
			};
			var result = await provider
				.FetchAsync<PointOfDeliveryQuery, PointOfDeliveryInfo>(query, byPassPipeline);
			return result.SingleOrDefault();
		}
	
	}
}