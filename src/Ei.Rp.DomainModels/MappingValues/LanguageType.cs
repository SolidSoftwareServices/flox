using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class LanguageType : TypedStringValue<LanguageType>
	{
		
		[JsonConstructor]
		private LanguageType()
		{
		}
		private LanguageType(string value) : base(value)
		{
		}

		public static readonly LanguageType English = new LanguageType("EN");
	   
	}


}