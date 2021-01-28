using System.Collections.Generic;
using System.Threading.Tasks;

namespace EI.RP.UiFlows.Core.Flows
{
	public interface IAppUiFlowsCollection
	{
		IUiFlow GetByFlowType(string flowType);
		Task<IUiFlow> GetByFlowHandler(string flowHandler);

		Task<IEnumerable<IUiFlow>> GetAll();

	}
}