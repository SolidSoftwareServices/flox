using System;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.DomainModelExtensions
{
	public static class ContactQueryTypeExtensions
	{
		public static string ToDisplayValue(this ContactQueryType src)
		{
			if (src == ContactQueryType.AddAdditionalAccount)
				return "Add an additional account";
			if (src == ContactQueryType.BillOrPayment)
				return "Bill or payment query";
			if (src == ContactQueryType.MeterRead)
				return "Meter read query";
			if (src == ContactQueryType.Other)
				return "Other";
			throw new ArgumentOutOfRangeException();
		}
	}
}