using System.Collections.Generic;
using S3.UiFlows.Core.Flows.Screens.ErrorHandling;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Core.Facade.Delegates
{
	public delegate IEnumerable<UiFlowUserInputError> OnExecuteValidationDelegate(UiFlowScreenModel screenModel);
}