using System.Linq;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;

namespace EI.RP.DomainServices.ModelExtensions
{
	internal static class ContractItemDtoDomainExtensions
	{
		public static decimal DiscountPercentage(this ContractItemDto source)
		{
			var discountString = source.Attributes.SingleOrDefault(x => x.AttributeID == "DISCOUNT_AMT")?.Value;
			return discountString != null ? decimal.Parse(discountString) : 0M;
		}
		public static bool IsFiniteContractTerm(this ContractItemDto source)
		{
			var value = source.Attributes.SingleOrDefault(x => x.AttributeID == "CONTTERM")?.Value;
			return value != null && !value.IsOneOf(string.Empty,"0","000", "999");
		}
	}
}