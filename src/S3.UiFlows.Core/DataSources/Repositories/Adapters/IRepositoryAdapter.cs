using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3.UiFlows.Core.DataSources.Repositories.Adapters
{
	internal interface IRepositoryAdapter
	{
		Task<UiFlowContextData> LoadByFlowHandler(string flowHandler);
		Task<UiFlowContextData> Save(UiFlowContextData contextData);
		Task Remove(string flowHandler);
		Task<IEnumerable<UiFlowContextData>> GetAll();
	}
}