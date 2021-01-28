using System.Threading.Tasks;

namespace EI.RP.Stubs.Helpers
{
	public interface ISessionOperations
	{
		Task Login(string userName, string password);
	}
}