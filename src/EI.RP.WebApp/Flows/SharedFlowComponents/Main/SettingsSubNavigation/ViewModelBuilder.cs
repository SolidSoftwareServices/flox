using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.UiFlows.Mvc.FlowTagHelpers;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.UserContactDetails.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.SettingsSubNavigation
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			var result = new ViewModel();

            var navigationItems = new List<NavigationItem>
            {
                BuildNavigationItem(
                    "My Details",
                    "sub-navigation-item-my-details",
                    componentInput.IsMyDetails,
                    ResidentialPortalFlowType.UserContactDetails),
                BuildNavigationItem(
	                "Change Password",
	                "sub-navigation-item-change-password",
	                componentInput.IsChangePassword,
	                ResidentialPortalFlowType.ChangePassword),
                BuildNavigationItem(
                    "Marketing",
                    "sub-navigation-item-marketing",
                    componentInput.IsMarketing,
                    ResidentialPortalFlowType.UserContactDetails,
                    UserContactFlowType.MarketingPreferences)
            };

            result.NavigationItems = navigationItems.ToArray();

            return result;
        }

		private static NavigationItem BuildNavigationItem(
			string label, 
			string testId, 
			bool isActive,
			ResidentialPortalFlowType flowType = ResidentialPortalFlowType.NoFlow)
		{
			return new NavigationItem
			{
				ClassList = new[] { isActive ? "active" : string.Empty },
				AnchorLink = new NavigationItem.AnchorLinkItem
				{
					Label = label,
					TestId = testId,
					FlowAction = flowType != ResidentialPortalFlowType.NoFlow 
						? new NavigationItem.FlowActionItem
						{
							FlowActionType = FlowActionTagHelper.FlowActionType.StartFlow,
							FlowLocation = FlowActionTagHelper.StartFlowLocation.SameContainerAsMe,
							FlowName = flowType.ToString()
						}
						: null
				}
			};
		}

		private static NavigationItem BuildNavigationItem(
			string label, 
			string testId, 
			bool isActive,
			ResidentialPortalFlowType flowType,
			UserContactFlowType startFlowType)
		{
			var navigationItem = BuildNavigationItem(label, testId, isActive, flowType);

			if(navigationItem.AnchorLink?.FlowAction != null)
			{
				navigationItem.AnchorLink.FlowAction.FlowParameters = new 
				{
					InitialFlowStartType = startFlowType
				};
			}

			return navigationItem;
		}
	}
}