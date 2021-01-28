using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using System.Threading.Tasks;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.MainNavigation
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
        public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
        {
            return new ViewModel
            {
	            IsAgentUser = componentInput.IsAgentUser,
	            SettingsLabel = "My Profile",
	            NavigationItems = componentInput.NavigationItems,
	            SettingsItems = componentInput.SettingsItems
            };
        }
	}
}
