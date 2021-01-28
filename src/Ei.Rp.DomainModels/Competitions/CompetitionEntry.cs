using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.Competitions
{
	public class CompetitionEntry:IQueryResult
	{
		public string Answer { get; set; }
	}
}