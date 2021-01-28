using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.MoveHouse
{
	public class CheckMoveOutRequestResult : IQueryResult
	{
		public bool IsMoveOutOk { get; set; }

        public bool HasExitFee { get; set; }
    }
}