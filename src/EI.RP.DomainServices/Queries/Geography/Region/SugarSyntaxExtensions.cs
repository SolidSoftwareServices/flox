using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ReadOnlyCollections;

namespace EI.RP.DomainServices.Queries.Geography.Region
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<IEnumerable<RegionDetails>> GetRegionByCountryId(this IDomainQueryResolver provider,
			string countryId, bool byPassPipeline = false)
		{
			var regions = await provider
				.FetchAsync<UtilitiesRegionQuery, RegionDetails>(new UtilitiesRegionQuery
				{
					CountryId = countryId
				}, byPassPipeline);
			return regions.ToArray();
		}
	}
}