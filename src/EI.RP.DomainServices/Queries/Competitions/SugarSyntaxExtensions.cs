using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Competitions;

namespace EI.RP.DomainServices.Queries.Competitions
{
	/// <summary>
	///     simplify Domain queries here
	/// </summary>
	public static class SugarSyntaxExtensions
	{
		public static async Task<IEnumerable<CompetitionEntry>> GetCompetitionEntriesByUserName(
			this IDomainQueryResolver provider, string userName, bool byPassPipeline = false)
		{
			var query = new CompetitionQuery {UserEmail = userName};
			return await provider.FetchAsync<CompetitionQuery, CompetitionEntry>(
				query, byPassPipeline);
		}
	}
}