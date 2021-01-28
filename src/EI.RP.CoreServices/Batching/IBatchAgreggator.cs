using System;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Batching
{
	public interface IBatchAggregator : IDisposable
	{
		Task CompleteBatch();
	}

	public interface IBatchAggregatorEnlister : IBatchAggregator
	{
		Task Enlist<TRequest>(TRequest request);
	}
}