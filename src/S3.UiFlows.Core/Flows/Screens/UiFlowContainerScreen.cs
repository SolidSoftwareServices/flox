using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.Containers;
using S3.UiFlows.Core.Infrastructure.DataSources;

namespace S3.UiFlows.Core.Flows.Screens
{
	public abstract class UiFlowContainerScreen<TFlowType> : UiFlowScreen<TFlowType>
	{
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new EmptyContainerScreenModel();
		}
	}
}