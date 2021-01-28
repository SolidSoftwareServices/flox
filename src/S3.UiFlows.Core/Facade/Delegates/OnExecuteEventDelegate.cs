using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Core.Facade.Delegates
{
	public delegate Task<TResult> OnExecuteEventDelegate<TResult>(string eventName, UiFlowScreenModel screenModel);
}