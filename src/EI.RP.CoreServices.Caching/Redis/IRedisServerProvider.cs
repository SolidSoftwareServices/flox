using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace EI.RP.CoreServices.Caching.Redis
{
	internal interface IRedisServerProvider:IDisposable
	{
		Task<IServer> GetServerAsync(CancellationToken token = default(CancellationToken));
		Task<ConnectionMultiplexer> GetConnectionAsync(CancellationToken token = default(CancellationToken));
		ConnectionMultiplexer GetConnection();
	}
}