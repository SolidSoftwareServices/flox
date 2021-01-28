using System;
using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.MoveHouse
{
	public class MovingHouseInProgressMovingInInfo : IQueryResult
	{		
		public string ContactNumber { get; set; }
		public int ElectricityMeterReadingDayOr24HrsValue { get; set; }
		public int ElectricityMeterReadingNightOrNshValue { get; set; }
		public int GasMeterReadingValue { get; set; }
		public DateTime? MovingInDate { get; set; }
		public bool HasConfirmedAuthority { get; set; }
		public bool HasConfirmedTermsAndConditions { get; set; }
	}
}