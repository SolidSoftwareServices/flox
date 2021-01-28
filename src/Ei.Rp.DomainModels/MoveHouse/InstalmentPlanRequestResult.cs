using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.MoveHouse
{
	public class InstalmentPlanRequestResult : IQueryResult
	{
		public bool HasInstalmentPlan { get; set; }
    }
}