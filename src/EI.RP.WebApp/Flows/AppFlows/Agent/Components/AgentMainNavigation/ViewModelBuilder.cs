using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.CoreServices.Cqrs.Queries;
using static EI.RP.UiFlows.Mvc.FlowTagHelpers.FlowActionTagHelper;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.AppFlows.Agent.Components.AgentMainNavigation
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
        private readonly IDomainQueryResolver _domainQueryResolver;

        public ViewModelBuilder(IDomainQueryResolver queryResolver)
		{
            _domainQueryResolver = queryResolver;
        }

		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			return new ViewModel
			{
				IsAgentUser = componentInput.IsAgentUser,
				NavigationItems = new NavigationItem[0],
                SettingsItems = new []
                {
	                BuildSettingsChangePassword()
                }
            };

            NavigationItem BuildSettingsChangePassword()
            {
                return new NavigationItem
                {
                    AnchorLink = new NavigationItem.AnchorLinkItem
                    {
                        Label = "Change Password",
                        TestId = "main-navigation-change-password-link-desktop",
                        ClassList = new [] { "portal-menu__profile__drop__item" },
                        Role = "menuitem",
                        FlowAction = new NavigationItem.FlowActionItem
                        {
                            FlowActionType = FlowActionType.StartFlow,
                            FlowLocation = StartFlowLocation.ContainedInMe,
                            FlowName = ResidentialPortalFlowType.ChangePassword.ToString()
                        }
                    }
                };
            }
		}
	}
}