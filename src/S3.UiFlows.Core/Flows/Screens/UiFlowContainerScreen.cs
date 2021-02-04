using System.Threading.Tasks;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.Containers;

namespace S3.UiFlows.Core.Flows.Screens
{
	public abstract class UiFlowContainerScreen<TFlowType> : UiFlowScreen<TFlowType>
	{
		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			return new EmptyContainerScreenModel();
		}
	}
}