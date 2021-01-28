using System.Threading.Tasks;

namespace S3.UiFlows.Core.Facade.Delegates
{
	public delegate Task<TResult> OnStartNewFlowDelegate<TResult>(string flowType, object startInfo);
}