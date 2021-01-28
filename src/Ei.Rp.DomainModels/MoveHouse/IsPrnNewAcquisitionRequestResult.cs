using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.MoveHouse
{
	public class IsPrnNewAcquisitionRequestResult : IQueryResult
	{
		public bool IsNewAcquisition { get; set; }
    }
}