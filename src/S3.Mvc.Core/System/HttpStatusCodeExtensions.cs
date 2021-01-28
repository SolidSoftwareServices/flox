using System.Net;

namespace S3.Mvc.Core.System
{
	public static class HttpStatusCodeExtensions
	{
		public static bool IsSuccessStatusCode(this int src)
		{
			return ((HttpStatusCode) src).IsSuccessStatusCode();
		}
		public static bool IsSuccessStatusCode(this HttpStatusCode src)
		{
			var i = (int) src;
			return i >= 200 && i <= 299;
		}

		public static bool IsRedirectionStatusCode(this int src)
		{
			return ((HttpStatusCode)src).IsRedirectionStatusCode();
		}
		public static bool IsRedirectionStatusCode(this HttpStatusCode src)
		{
			var i = (int)src;
			return i >= 300 && i <= 399;
		}
		
	}
}