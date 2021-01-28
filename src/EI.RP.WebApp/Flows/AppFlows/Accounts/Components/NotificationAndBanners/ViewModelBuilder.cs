using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using System.Threading.Tasks;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.NotificationAndBanners
{
	public class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			return new ViewModel
			{
				AccountNumber = componentInput.AccountNumber,
				ToSmartActivationEvent = componentInput.ToSmartActivationEvent,
				ToCompetitionEvent = componentInput.ToCompetitionEvent,
				ToPromotionEvent = componentInput.ToPromotionEvent,
				DismissSmartActivationBannerEvent = componentInput.DismissSmartActivationBannerEvent,
				DismissCompetitionBannerEvent = componentInput.DismissCompetitionBannerEvent,
				DismissPromotionBannerEvent = componentInput.DismissPromotionBannerEvent,
			};
		}
	}
}
