using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.PrivacyNoticeMessage
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		public ViewModelBuilder()
		{
		}

		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			var result = new ViewModel
			{
				AccountNumber = componentInput.AccountNumber,
				PrependedMessage = componentInput.PrependedMessage
			};

			return result;
		}
	}
}