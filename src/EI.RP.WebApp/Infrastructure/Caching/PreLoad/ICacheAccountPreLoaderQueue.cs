using EI.RP.CoreServices.Caching;

namespace EI.RP.WebApp.Infrastructure.Caching.PreLoad
{
	public interface ICacheAccountPreLoaderQueue<TRequest>: ICacheAccountPreLoaderRequester
	{
		bool IsEmpty { get; }
		bool TryDequeue(out TRequest request);
	}
}