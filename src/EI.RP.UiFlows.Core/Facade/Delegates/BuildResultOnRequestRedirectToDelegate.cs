using System.Threading.Tasks;

namespace EI.RP.UiFlows.Core.Facade.Delegates
{
	public delegate Task<TResult> BuildResultOnRequestRedirectToDelegate<TResult>(string flowHandler);
}