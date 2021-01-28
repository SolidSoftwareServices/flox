using System.Threading.Tasks;

namespace EI.RP.UiFlows.Core.Facade.Delegates
{
	public delegate Task<TResult> OnRedirectToFlowDelegate<TResult>(string rootFlowType, string rootHandler);
}