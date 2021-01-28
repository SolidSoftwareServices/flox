using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class MeterReadingStatus : TypedStringValue<MeterReadingStatus>
	{
		
		[JsonConstructor]
		private MeterReadingStatus()
		{
		}
		private MeterReadingStatus(string value, string debuggerFriendlyDisplayValue) : base(value,debuggerFriendlyDisplayValue,true)
		{
		}


		
        public static readonly MeterReadingStatus OrderCreated = new MeterReadingStatus("0",nameof(OrderCreated));
        public static readonly MeterReadingStatus Billable = new MeterReadingStatus("1", nameof(Billable));
		public static readonly MeterReadingStatus AutomaticallyLocked = new MeterReadingStatus("2", nameof(AutomaticallyLocked));
        public static readonly MeterReadingStatus LockedByAgent = new MeterReadingStatus("3", nameof(LockedByAgent));
        public static readonly MeterReadingStatus ReleasedByAgent = new MeterReadingStatus("4", nameof(ReleasedByAgent));
        public static readonly MeterReadingStatus CheckedIndependently = new MeterReadingStatus("5", nameof(CheckedIndependently));
		public static readonly MeterReadingStatus Billed = new MeterReadingStatus("7", nameof(Billed));

	

	}

	
}