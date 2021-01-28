using System.Threading.Tasks;

namespace EI.RP.UiFlows.Core.Facade
{
	public interface IFlowRequestHandler<in TInput, TResult>
	{
		Task<TResult> Execute(TInput input);
	}
}