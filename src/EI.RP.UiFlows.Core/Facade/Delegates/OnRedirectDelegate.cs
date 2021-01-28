using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;

namespace EI.RP.UiFlows.Core.Facade.Delegates
{
	public delegate Task<TResult> OnRedirectDelegate<TResult>(UiFlowExitRedirection model);
}