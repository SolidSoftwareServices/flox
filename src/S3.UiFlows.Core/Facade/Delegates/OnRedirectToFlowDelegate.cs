using System.Threading.Tasks;

namespace S3.UiFlows.Core.Facade.Delegates
{
	public delegate Task<TResult> OnRedirectToFlowDelegate<TResult>(string rootFlowType, string rootHandler);
}