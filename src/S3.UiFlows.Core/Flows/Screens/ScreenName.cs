using S3.CoreServices.System;
using Newtonsoft.Json;

namespace S3.UiFlows.Core.Flows.Screens
{
	public sealed class ScreenName : TypedStringValue<ScreenName>
	{
		//Default steps
		public static ScreenName PreStart = new ScreenName("PreStart");

		[JsonConstructor]
		private ScreenName()
		{
		}

		public ScreenName(string valueId) : base(valueId, disableVerifyValueExists: true)
		{
		}

		public static implicit operator ScreenName(string src)
		{
			return new ScreenName(src);
		}
	}
}