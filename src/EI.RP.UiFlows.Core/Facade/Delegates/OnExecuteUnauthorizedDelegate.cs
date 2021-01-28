using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.ErrorHandling;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.DefaultModels;

namespace EI.RP.UiFlows.Core.Facade.Delegates
{
	public delegate Task<TResult> OnExecuteUnauthorizedDelegate<TResult>(UiFlowStepUnauthorized screenModel);

	public delegate IEnumerable<UiFlowUserInputError> OnExecuteValidationDelegate(UiFlowScreenModel screenModel);
}