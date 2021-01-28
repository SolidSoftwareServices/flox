using System.Collections.Generic;

namespace EI.RP.CoreServices.Http.Session
{
	public interface IAppCookieSettings
	{
		IEnumerable<string> AppCookieNames { get; }
	}
}