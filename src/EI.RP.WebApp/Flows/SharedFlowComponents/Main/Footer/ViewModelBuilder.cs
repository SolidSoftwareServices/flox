using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.Footer
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			return new ViewModel
            {
				NavigationItems = componentInput.NavigationItems,
				IsAlwaysOnBottom = componentInput.IsAlwaysOnBottom
            };
		}
	}
}
