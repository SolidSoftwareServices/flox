using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;

namespace EI.RP.UiFlows.Core.Facade.Delegates
{
	public delegate Task<TResult> OnExecuteEventDelegate<TResult>(string eventName, UiFlowScreenModel screenModel);
}