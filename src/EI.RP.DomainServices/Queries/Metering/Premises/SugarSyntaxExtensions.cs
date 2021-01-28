using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Metering.Premises
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<Premise> GetPremise(
			this IDomainQueryResolver provider, string premiseId, bool byPassPipeline = false)
		{
			var query = new PremisesQuery
			{
				PremiseId = premiseId
			};
			var result = await provider
				.FetchAsync<PremisesQuery, Premise>(query, byPassPipeline);
			return result.SingleOrDefault();
		}

		public static async Task<Premise> GetPremiseByPrn(
			this IDomainQueryResolver provider, PointReferenceNumber prn, bool byPassPipeline = false)
		{
			var query = new PremisesQuery
			{
				Prn = prn
			};
			var result = await provider
				.FetchAsync<PremisesQuery, Premise>(query, byPassPipeline);
			return result.SingleOrDefault();
		}
	}
}