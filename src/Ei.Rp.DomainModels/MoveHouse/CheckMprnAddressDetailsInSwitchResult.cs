using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.MoveHouse
{
	public class CheckMprnAddressDetailsInSwitchResult : IQueryResult
	{
		public bool HasAddressDetails { get; set; }
    }
}