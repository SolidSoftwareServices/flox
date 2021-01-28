using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class QuotationStatusType : TypedStringValue<QuotationStatusType>
	{
		

		[JsonConstructor]
		private QuotationStatusType()
		{
		}

		private QuotationStatusType(string value) : base(value)
		{
		}

		public static readonly QuotationStatusType Finished = new QuotationStatusType("I1005");
		public static readonly QuotationStatusType InProgress = new QuotationStatusType("I1055");

	}
}