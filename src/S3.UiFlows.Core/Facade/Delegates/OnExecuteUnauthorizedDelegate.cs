using System.Collections.Generic;
using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens.ErrorHandling;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.DefaultModels;

namespace S3.UiFlows.Core.Facade.Delegates
{
	public delegate Task<TResult> OnExecuteUnauthorizedDelegate<TResult>(UiFlowStepUnauthorized screenModel);

	public delegate IEnumerable<UiFlowUserInputError> OnExecuteValidationDelegate(UiFlowScreenModel screenModel);
}