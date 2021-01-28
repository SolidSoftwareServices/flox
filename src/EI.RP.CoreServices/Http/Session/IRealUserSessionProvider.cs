using System.Security.Claims;

namespace EI.RP.CoreServices.Http.Session
{
	public interface IRealUserSessionProvider:IUserSessionProvider
	{
	}

	public interface IActingAsUserSessionProvider : IUserSessionProvider
	{
	}


}