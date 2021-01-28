using System.Linq;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class MeterReadingCategoryType : TypedStringValue<MeterReadingCategoryType>
	{

		[JsonConstructor]
		private MeterReadingCategoryType()
		{
		}

		private MeterReadingCategoryType(string value,string debuggerFriendlyDisplayValue) : base(value,debuggerFriendlyDisplayValue, true)
		{
		}

		public static readonly MeterReadingCategoryType None = new MeterReadingCategoryType(string.Empty,nameof(None));
		public static readonly MeterReadingCategoryType Network = new MeterReadingCategoryType("01", nameof(Network));
		public static readonly MeterReadingCategoryType Customer = new MeterReadingCategoryType("02", nameof(Customer));
		public static readonly MeterReadingCategoryType EstimatedA = new MeterReadingCategoryType("03", nameof(EstimatedA));
		public static readonly MeterReadingCategoryType EstimatedB = new MeterReadingCategoryType("04", nameof(EstimatedB));
		public static readonly MeterReadingCategoryType Actual = new MeterReadingCategoryType("Actual", nameof(Actual));

		private static readonly string[] EstimationTypes = {"03", "04", "05", "91", "92", "93", "94"};
		public bool IsEstimation => EstimationTypes.Any(x => x == ValueId);
	}
}