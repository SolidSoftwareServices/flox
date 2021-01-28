using System;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.DomainModelExtensions
{
	public static class PaymentMethodTypeUiExtensions
	{
		public static string ToUserText(this PaymentMethodType src)
		{
			if (src == PaymentMethodType.DirectDebit) return "Direct Debit";
			if (src == PaymentMethodType.Equalizer) return "Equalizer";
			if (src == PaymentMethodType.DirectDebitNotAvailable) return "Direct Debit Not Available";
			if (src == PaymentMethodType.Manual) return "Manual";
			if (src == PaymentMethodType.AlternativePayer) return "Alternative Payer";
			return "Other";
		}
	}
}