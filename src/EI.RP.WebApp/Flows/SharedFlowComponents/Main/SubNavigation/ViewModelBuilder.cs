using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.CoreServices.Platform;
using EI.RP.WebApp.Flows.AppFlows;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.SubNavigation
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
        public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
            return new ViewModel
			{
                CurrentFlowType = screenModelContainingTheComponent.GetContainedFlow<ResidentialPortalFlowType>(),
				NavigationItems = componentInput.NavigationItems
            };
        }
	}
}