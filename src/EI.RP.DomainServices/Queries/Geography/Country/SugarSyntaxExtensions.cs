using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ReadOnlyCollections;

namespace EI.RP.DomainServices.Queries.Geography.Country
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<IEnumerable<CountryDetails>> GetCountries(this IDomainQueryResolver provider,
			bool byPassPipeline = false)
		{
			var regions = await provider
				.FetchAsync<CountryDetailsQuery, CountryDetails>(new CountryDetailsQuery
				{
				}, byPassPipeline);
			return regions.ToArray();
		}
	}
}