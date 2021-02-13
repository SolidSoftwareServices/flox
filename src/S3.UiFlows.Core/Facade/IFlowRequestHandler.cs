using System.Threading.Tasks;

namespace S3.UiFlows.Core.Facade
{
	public interface IFlowRequestHandler<in TInput, TResult>
	{
		Task<TResult> Execute(TInput input);
	}
}