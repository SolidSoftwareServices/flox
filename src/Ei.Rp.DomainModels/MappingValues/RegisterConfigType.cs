using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class RegisterConfigType : TypedStringValue<RegisterConfigType>
	{
		[JsonConstructor]
		private RegisterConfigType()
		{
		}
		private RegisterConfigType(string value) : base(value,disableVerifyValueExists:true)
		{
		}


		public static readonly RegisterConfigType MCC01 = new RegisterConfigType("MCC01");
		public static readonly RegisterConfigType MCC12 = new RegisterConfigType("MCC12");
		public static readonly RegisterConfigType MCC16 = new RegisterConfigType("MCC16");


		public bool IsSmartElectricity()
		{
			return IsOneOf(MCC12, MCC16);
		}

		public bool IsSmartConfigurationActive()
		{
			return this == RegisterConfigType.MCC12;
		}

		public bool CanOptToSmartActive()
		{
			return IsOneOf(MCC01, MCC16);
		}
	}
}