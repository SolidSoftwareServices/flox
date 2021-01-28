using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Containers;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.UiFlows.Core.Flows.Screens
{
	public abstract class UiFlowContainerScreen<TFlowType> : UiFlowScreen<TFlowType>
	{
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new EmptyContainerScreenModel();
		}
	}
}