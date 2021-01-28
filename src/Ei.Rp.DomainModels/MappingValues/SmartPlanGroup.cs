using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class SmartPlanGroup : TypedStringValue<SmartPlanGroup>
	{
		[JsonConstructor]
		public SmartPlanGroup()
		{
		}

		public SmartPlanGroup(string value) : base(value, disableVerifyValueExists:true)
		{
		}

		public static readonly SmartPlanGroup Dual = new SmartPlanGroup("DUAL");
		public static readonly SmartPlanGroup Single = new SmartPlanGroup("SINGLE");

	

	}
}