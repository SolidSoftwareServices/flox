using System.Linq;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class MeterReadingRegisterType : TypedStringValue<MeterReadingRegisterType>
	{

		[JsonConstructor]
		private MeterReadingRegisterType()
		{
		}

		private MeterReadingRegisterType(string value,string debuggerFriendlyDisplayValue) : base(value,debuggerFriendlyDisplayValue, true)
		{
		}

		public static readonly MeterReadingRegisterType None = new MeterReadingRegisterType(string.Empty,nameof(None));
		public static readonly MeterReadingRegisterType ActiveEnergyRegisterType = new MeterReadingRegisterType("001", nameof(ActiveEnergyRegisterType));
		public static readonly MeterReadingRegisterType ReactiveEnergyRegisterType = new MeterReadingRegisterType("002", nameof(ReactiveEnergyRegisterType));
		public static readonly MeterReadingRegisterType ActivePowerRegisterType = new MeterReadingRegisterType("003", nameof(ActivePowerRegisterType));
		public static readonly MeterReadingRegisterType ReactivePowerRegisterType = new MeterReadingRegisterType("004", nameof(ReactivePowerRegisterType));
		public static readonly MeterReadingRegisterType ResettingRegisterType = new MeterReadingRegisterType("005", nameof(ResettingRegisterType));



		public static readonly MeterReadingRegisterType Unknown006 = new MeterReadingRegisterType("006", nameof(Unknown006));
		public static readonly MeterReadingRegisterType Unknown007 = new MeterReadingRegisterType("007", nameof(Unknown007));
		public static readonly MeterReadingRegisterType Unknown008 = new MeterReadingRegisterType("008", nameof(Unknown008));
		public static readonly MeterReadingRegisterType Unknown009 = new MeterReadingRegisterType("009", nameof(Unknown009));


	}
}