using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;

namespace Ei.Rp.DomainModels.MoveHouse
{
	public class MovingHouseInProgressNewPRNsInfo : IQueryResult
	{
		

		public ElectricityPointReferenceNumber NewMprn { get; set; }
		public GasPointReferenceNumber NewGprn { get; set; }

		public bool HasNewMprn()
		{
			throw new System.NotImplementedException();
		}
	}
}