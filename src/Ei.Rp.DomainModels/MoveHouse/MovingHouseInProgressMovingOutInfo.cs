using System;
using EI.RP.CoreServices.Cqrs.Queries;

using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;

namespace Ei.Rp.DomainModels.MoveHouse
{
	public class MovingHouseInProgressMovingOutInfo:IQueryResult
	{
		/// <summary>
		/// Handler
		/// </summary>
		public string InitiatedFromAccountNumber { get; set; }

		public string OtherAccountNumber { get; set; }
		public DateTime MovingOutDate { get; set; }
		public int ElectricityMeterReadingDayOr24HrsValue { get; set; }
		public int ElectricityMeterReadingNightOrNshValue { get; set; }
		public int GasMeterReadingValue { get; set; }
		public bool InformationCollectionAuthorized { get; set; }
		public bool TermsAndConditionsAccepted { get; set; }
		public bool IncomingOccupant { get; set; }
		public string LettingAgentName { get; set; }
		public string LettingPhoneNumber { get; set; }
		public bool OccupierDetailsAccepted { get; set; }

	}
}