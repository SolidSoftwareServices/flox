using System.Threading.Tasks;

namespace EI.RP.CoreServices.Caching
{
	public interface ICacheAccountPreLoaderRequester
	{
		Task SubmitRequestAsync(string forUserName);
	}
}