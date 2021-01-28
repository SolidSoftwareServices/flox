using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using static EI.RP.UiFlows.Mvc.FlowTagHelpers.FlowActionTagHelper;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.CollectiveAccountMainNavigation
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			return new ViewModel
			{
				NavigationItems = new []
                {
	                BuildMainChangePasswordMobile(),
	                BuildMainHelp(),
                    BuildMainContactUs()
                },
                SettingsItems = new []
                {
	                BuildSettingsChangePassword()
                }
            };

            NavigationItem BuildMainChangePasswordMobile()
            {
	            return new NavigationItem
	            {
		            ClassList = new [] { "portal-menu__nav__item", "d-lg-none" },
		            Role = "none",
		            AnchorLink = new NavigationItem.AnchorLinkItem
		            {
			            Label = "Change Password",
			            TestId = "main-navigation-change-password-link-mobile",
			            ClassList = new [] { "portal-menu__nav__item__link", "profile" },
			            Role = "menuitem",
			            FlowAction = new NavigationItem.FlowActionItem
			            {
				            FlowActionType = FlowActionType.StartFlow,
				            FlowLocation = StartFlowLocation.ContainedInMe,
				            FlowName = ResidentialPortalFlowType.ChangePassword.ToString(),
			            }
		            }
	            };
            }

            NavigationItem BuildMainHelp()
            {
                return new NavigationItem
                {
                    ClassList = new [] { "portal-menu__nav__item" },
                    Role = "none",
                    AnchorLink = new NavigationItem.AnchorLinkItem
                    {
                        Label = "Help",
                        TestId = "main-navigation-help-link",
                        ClassList = new [] { "portal-menu__nav__item__link" },
                        Role = "menuitem",
                        FlowAction = new NavigationItem.FlowActionItem
                        {
                            FlowActionType = FlowActionType.StartFlow,
                            FlowLocation = StartFlowLocation.ContainedInMe,
                            FlowName = ResidentialPortalFlowType.Help.ToString()
                        }
                    }
                };
            }

            NavigationItem BuildMainContactUs()
            {
                return new NavigationItem
                {
                    ClassList = new [] { "portal-menu__nav__item" },
                    Role = "none",
                    AnchorLink = new NavigationItem.AnchorLinkItem
                    {
                        Label = "Contact Us",
                        TestId = "main-navigation-contact-us-link",
                        ClassList = new [] { "portal-menu__nav__item__link", "contact" },
                        Role = "menuitem",
                        FlowAction = new NavigationItem.FlowActionItem
                        {
                            FlowActionType = FlowActionType.StartFlow,
                            FlowLocation = StartFlowLocation.ContainedInMe,
                            FlowName = ResidentialPortalFlowType.ContactUs.ToString()
                        }
                    }
                };
            }

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
                            FlowName = ResidentialPortalFlowType.ChangePassword.ToString(),
                        }
                    }
                };
            }
		}
	}
}