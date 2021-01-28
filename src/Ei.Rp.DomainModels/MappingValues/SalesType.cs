using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class SalesType : TypedStringValue<SalesType>
	{


		[JsonConstructor]
		private SalesType()
		{
		}

		private SalesType(string value) : base(value)
		{
		}

		public static readonly SalesType AllNewAcquisitions = new SalesType("WIN");
		public static readonly SalesType AllExistingAcquisitions = new SalesType("RETEN");
		public static readonly SalesType Mix = new SalesType("CRSSE");

	}
}