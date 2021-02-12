using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3.UiFlows.Core.Flows
{
	public interface IAppUiFlowsCollection
	{
		IUiFlow GetByFlowType(string flowType);
		Task<IUiFlow> GetByFlowHandler(string flowHandler);

		Task<IEnumerable<IUiFlow>> GetAll();

	}
}