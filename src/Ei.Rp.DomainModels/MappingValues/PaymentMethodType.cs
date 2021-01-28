using System;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class PaymentMethodType : TypedStringValue<PaymentMethodType>
	{
	
		[JsonConstructor]
		private PaymentMethodType()
		{
		}
		public PaymentMethodType(string value,string debuggerFriendlyDisplayValue) : base(value,debuggerFriendlyDisplayValue, true)
		{
		}

		public static readonly PaymentMethodType Manual = new PaymentMethodType(string.Empty, nameof(Manual));

		public static readonly PaymentMethodType DirectDebit = new PaymentMethodType("D",nameof(DirectDebit));
		public static readonly PaymentMethodType Equalizer = new PaymentMethodType("E",nameof(Equalizer));

		public static readonly PaymentMethodType DirectDebitNotAvailable = new PaymentMethodType("A",nameof(DirectDebitNotAvailable));

		public static readonly PaymentMethodType AlternativePayer = new PaymentMethodType("AlternativePayerCA",nameof(AlternativePayer));
		public static readonly PaymentMethodType ESBPremises = new PaymentMethodType("F", nameof(ESBPremises));

		public bool HasDirectDebitAvailable()
		{
			return this.IsOneOf
			(
				Manual,
				DirectDebit,
				Equalizer,
				ESBPremises
			//add others as they are created here if dd can be an option given the payment method
			);
		}

		public bool IsEqualiser()
		{
			return this == Equalizer;
		}
		public bool IsAlternativePayer()
		{
			return this == AlternativePayer;
		}


		//TODO: the other sugar syntax
		
	}
}