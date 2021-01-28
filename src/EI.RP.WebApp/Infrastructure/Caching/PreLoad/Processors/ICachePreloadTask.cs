using System.Threading.Tasks;

namespace EI.RP.WebApp.Infrastructure.Caching.PreLoad.Processors
{
	interface ICachePreloadTask
	{
		int PriorityOrder { get; }
		Task ProcessRequest(CacheAccountPreLoaderQueue.CacheAccountPreLoadRequest message);
	}
}