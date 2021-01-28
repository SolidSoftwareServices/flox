using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class ReadingType : TypedStringValue<ReadingType>
	{
		
		[JsonConstructor]
		private ReadingType()
		{
		}
		private ReadingType(string value) : base(value)
		{
		}

		public static readonly ReadingType Actual = new ReadingType("Actual");
        public static readonly ReadingType None = new ReadingType(string.Empty);

    }
}