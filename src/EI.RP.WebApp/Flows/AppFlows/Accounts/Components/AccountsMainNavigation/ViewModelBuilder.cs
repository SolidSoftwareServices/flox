using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Steps;
using EI.RP.WebApp.Flows.AppFlows.UserContactDetails.FlowDefinitions;
using static EI.RP.UiFlows.Mvc.FlowTagHelpers.FlowActionTagHelper;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountsMainNavigation
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			var eventAdditionalFields = new[]
			{
				new NavigationItem.EventAdditionalField
				{
					PropertyPath = "SelectedAccount.AccountNumber",
					Value = componentInput.AccountNumber
				}
			};

			return new ViewModel
			{
				IsAgentUser = componentInput.IsAgentUser,
				NavigationItems = new[]
				{
					BuildMainChangePasswordMobile(),
					BuildMainMyAccounts(),
					BuildMainProductsAndServices(),
					BuildMainHelp(),
					BuildMainContactUs()
				},
				SettingsItems = new[]
				{
					BuildSettingsChangePassword(),
				}
			};
			
			NavigationItem BuildMainChangePasswordMobile()
			{
				return new NavigationItem
				{
					ClassList = new[] { "portal-menu__nav__item", "d-lg-none" },
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Change Password",
						TestId = "main-navigation-change-password-link-mobile",
						ClassList = new[] { "portal-menu__nav__item__link", "profile" },
						Role = "link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							TriggerEvent = AccountSelection.StepEvent.ToChangePassword,
							EventFlowType = ResidentialPortalFlowType.Accounts,
							EventAdditionalFields = eventAdditionalFields
						}
					}
				};
			}			

			NavigationItem BuildMainMyAccounts()
			{
				return new NavigationItem
				{
					ClassList = new[] { "portal-menu__nav__item" },
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "My Accounts",
						TestId = "main-navigation-my-accounts-link",
						ClassList = new[] { "portal-menu__nav__item__link" },
						Role = "link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.NotContained,
							FlowName = ResidentialPortalFlowType.Accounts.ToString()
						}
					}
				};
			}

			NavigationItem BuildMainProductsAndServices()
			{
				return new NavigationItem
				{
					ClassList = new[] { "portal-menu__nav__item" },
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Products & Services",
						TestId = "main-navigation-products-and-services-link",
						ClassList = new[] { "portal-menu__nav__item__link" },
						Role = "link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							TriggerEvent = AccountSelection.StepEvent.ToProductsAndServices,
							EventFlowType = ResidentialPortalFlowType.Accounts,
							EventAdditionalFields = eventAdditionalFields
						}
					}
				};
			}

			NavigationItem BuildMainHelp()
			{
				return new NavigationItem
				{
					ClassList = new[] { "portal-menu__nav__item" },
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Help",
						TestId = "main-navigation-help-link",
						ClassList = new[] { "portal-menu__nav__item__link" },
						Role = "link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							TriggerEvent = AccountSelection.StepEvent.ToHelp,
							EventFlowType = ResidentialPortalFlowType.Accounts,
							EventAdditionalFields = eventAdditionalFields
						}
					}
				};
			}

			NavigationItem BuildMainContactUs()
			{
				return new NavigationItem
				{
					ClassList = new[] { "portal-menu__nav__item" },
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Contact Us",
						TestId = "main-navigation-contact-us-link",
						ClassList = new[] { "portal-menu__nav__item__link", "contact" },
						Role = "link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							TriggerEvent = AccountSelection.StepEvent.ToContactUs,
							EventFlowType = ResidentialPortalFlowType.Accounts,
							EventAdditionalFields = eventAdditionalFields
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
						ClassList = new[] { "portal-menu__profile__drop__item" },
						Role = "link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							TriggerEvent = AccountSelection.StepEvent.ToChangePassword,
							EventFlowType = ResidentialPortalFlowType.Accounts,
							EventAdditionalFields = eventAdditionalFields
						}
					}
				};
			}			
		}
	}
}