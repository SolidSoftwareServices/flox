using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3.UiFlows.Core.Infrastructure.DataSources.Repositories.Strategies
{
	internal interface IInternalUiFlowContextRepository
	{
		ContextStoreStrategy StoreStrategy { get; }
		Task<UiFlowContextData> LoadByFlowHandler(string flowHandler);
		Task<UiFlowContextData> Save(UiFlowContextData contextData);
		Task Remove(string flowHandler);
		Task<IEnumerable<UiFlowContextData>> GetAll();
	}
}