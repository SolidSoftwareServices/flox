using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class SmartConsent : TypedStringValue<SmartConsent>
	{

		[JsonConstructor]
		private SmartConsent()
		{
		}

		private SmartConsent(string value) : base(value)
		{
		}

		public static readonly SmartConsent ConsentGiven = new SmartConsent("001");
		public static readonly SmartConsent ConsentRejected= new SmartConsent("002");
	}
}