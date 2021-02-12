using Newtonsoft.Json;
using S3.CoreServices.System;

namespace S3.CoreServices.AspNet.Http.Headers
{
	public class OnAuthorizationFailedHeaderValue : TypedStringValue<OnAuthorizationFailedHeaderValue>
	{
		[JsonConstructor]
		private OnAuthorizationFailedHeaderValue()
		{

		}

		private OnAuthorizationFailedHeaderValue(string value) : base(value)
		{

		}

		public static readonly OnAuthorizationFailedHeaderValue RedirectToLogin = new OnAuthorizationFailedHeaderValue(nameof(RedirectToLogin));
		public static readonly OnAuthorizationFailedHeaderValue ReturnUnauthorizedStatusCode = new OnAuthorizationFailedHeaderValue(nameof(ReturnUnauthorizedStatusCode));
	}
}