using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using static EI.RP.UiFlows.Mvc.FlowTagHelpers.FlowActionTagHelper;
using EI.RP.WebApp.Flows.AppFlows.ContactUs.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.UserContactDetails.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.AppFlows.AccountDashboard.Components.AccountDashboardMainNavigation
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		public async Task<ViewModel> Resolve(InputModel componentInput,
			UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			if (screenModelContainingTheComponent.GetContainedFlow<ResidentialPortalFlowType>() == ResidentialPortalFlowType.ProductAndServices ||
			screenModelContainingTheComponent.GetContainedFlow<ResidentialPortalFlowType>() == ResidentialPortalFlowType.ContactUs ||
			screenModelContainingTheComponent.GetContainedFlow<ResidentialPortalFlowType>() == ResidentialPortalFlowType.Help ||
			screenModelContainingTheComponent.GetContainedFlow<ResidentialPortalFlowType>() == ResidentialPortalFlowType.PromotionEntry ||
			screenModelContainingTheComponent.GetContainedFlow<ResidentialPortalFlowType>() == ResidentialPortalFlowType.CompetitionEntry)
			{

				return new ViewModel
				{
					IsAgentUser = componentInput.IsAgentUser,
					NavigationItems = FixedNavigationItemsWithNoAccount.Union(BuildMainContactUs().ToOneItemArray()),
					SettingsItems = FixedSettingsItemsWithNoAccount
				};
			}
			return new ViewModel
			{
				IsAgentUser = componentInput.IsAgentUser,
				NavigationItems = FixedNavigationItems.Union(BuildMainContactUs().ToOneItemArray()),
				SettingsItems = FixedSettingsItems
			};


			NavigationItem BuildMainContactUs()
			{
				return new NavigationItem
				{
					ClassList = new[] {"portal-menu__nav__item"},
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Contact Us",
						TestId = "main-navigation-contact-us-link",
						ClassList = new[] {"portal-menu__nav__item__link", "contact"},
						FlowAction = new NavigationItem.FlowActionItem
						{
							FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.ContainedInMe,
							FlowName = ResidentialPortalFlowType.ContactUs.ToString(),
							FlowParameters = new ContactUsInput
							{
								AccountNumber = componentInput.AccountNumber
							}
						}
					}
				};
			}

			
		}

		private static readonly NavigationItem[] FixedSettingsItems = new[]
		{
			BuildSettingsMyDetails(),
			BuildSettingsChangePassword(),
			BuildSettingsMarketing()
		};

		private static readonly NavigationItem[] FixedSettingsItemsWithNoAccount = new[]
		{
			BuildSettingsChangePassword(),
		};

		private static NavigationItem BuildSettingsMyDetails()
		{
			return new NavigationItem
			{
				AnchorLink = new NavigationItem.AnchorLinkItem
				{
					Label = "My Details",
					TestId = "main-navigation-my-profile-link-desktop",
					ClassList = new[] {"portal-menu__profile__drop__item"},
					FlowAction = new NavigationItem.FlowActionItem
					{
						FlowActionType = FlowActionType.StartFlow,
						FlowLocation = StartFlowLocation.ContainedInMe,
						FlowName = ResidentialPortalFlowType.UserContactDetails.ToString(),
						FlowParameters = new
						{
							InitialFlowStartType = UserContactFlowType.ContactDetails
						}
					}
				}
			};
		}

		private static NavigationItem BuildSettingsChangePassword()
		{
			return new NavigationItem
			{
				AnchorLink = new NavigationItem.AnchorLinkItem
				{
					Label = "Change Password",
					TestId = "main-navigation-change-password-link-desktop",
					ClassList = new[] {"portal-menu__profile__drop__item"},
					FlowAction = new NavigationItem.FlowActionItem
					{
						FlowActionType = FlowActionType.StartFlow,
						FlowLocation = StartFlowLocation.ContainedInMe,
						FlowName = ResidentialPortalFlowType.ChangePassword.ToString(),
					}
				}
			};
		}

		private static NavigationItem BuildSettingsMarketing()
		{
			return new NavigationItem
			{
				AnchorLink = new NavigationItem.AnchorLinkItem
				{
					Label = "Marketing",
					TestId = "main-navigation-marketing-link-desktop",
					ClassList = new[] {"portal-menu__profile__drop__item"},
					FlowAction = new NavigationItem.FlowActionItem
					{
						FlowActionType = FlowActionType.StartFlow,
						FlowLocation = StartFlowLocation.ContainedInMe,
						FlowName = ResidentialPortalFlowType.UserContactDetails.ToString(),
						FlowParameters = new
						{
							InitialFlowStartType = UserContactFlowType.MarketingPreferences
						}
					}
				}
			};
		}

		private static readonly IEnumerable<NavigationItem> FixedNavigationItems = new[]
		{
			BuildMainMyDetailsMobile(),
			BuildMainChangePasswordMobile(),
			BuildMainMarketingMobile(),
			BuildMainMyAccounts(),
			BuildMainProductsAndServices(),
			BuildMainHelp()
		};

		private static readonly IEnumerable<NavigationItem> FixedNavigationItemsWithNoAccount = new[]
		{
			BuildMainChangePasswordMobile(),
			BuildMainMyAccounts(),
			BuildMainProductsAndServices(),
			BuildMainHelp()
		};

		private static NavigationItem BuildMainMyDetailsMobile()
		{
			return new NavigationItem
			{
				ClassList = new[] {"portal-menu__nav__item", "d-lg-none"},
				AnchorLink = new NavigationItem.AnchorLinkItem
				{
					Label = "My Details",
					TestId = "main-navigation-my-profile-link-mobile",
					ClassList = new[] {"portal-menu__nav__item__link", "profile"},
					FlowAction = new NavigationItem.FlowActionItem
					{
						FlowActionType = FlowActionType.StartFlow,
						FlowLocation = StartFlowLocation.ContainedInMe,
						FlowName = ResidentialPortalFlowType.UserContactDetails.ToString()
					}
				}
			};
		}

		private static NavigationItem BuildMainChangePasswordMobile()
		{
			return new NavigationItem
			{
				ClassList = new[] {"portal-menu__nav__item", "d-lg-none"},
				AnchorLink = new NavigationItem.AnchorLinkItem
				{
					Label = "Change Password",
					TestId = "main-navigation-change-password-link-mobile",
					ClassList = new[] {"portal-menu__nav__item__link", "profile"},
					FlowAction = new NavigationItem.FlowActionItem
					{
						FlowActionType = FlowActionType.StartFlow,
						FlowLocation = StartFlowLocation.ContainedInMe,
						FlowName = ResidentialPortalFlowType.ChangePassword.ToString(),
					}
				}
			};
		}

		private static NavigationItem BuildMainMarketingMobile()
		{
			return new NavigationItem
			{
				ClassList = new[] {"portal-menu__nav__item", "d-lg-none"},
				AnchorLink = new NavigationItem.AnchorLinkItem
				{
					Label = "Marketing",
					TestId = "main-navigation-marketing-link-mobile",
					ClassList = new[] {"portal-menu__nav__item__link", "profile"},
					FlowAction = new NavigationItem.FlowActionItem
					{
						FlowActionType = FlowActionType.StartFlow,
						FlowLocation = StartFlowLocation.ContainedInMe,
						FlowName = ResidentialPortalFlowType.UserContactDetails.ToString(),
						FlowParameters = new
						{
							InitialFlowStartType = UserContactFlowType.MarketingPreferences
						}
					}
				}
			};
		}

		private static NavigationItem BuildMainMyAccounts()
		{
			return new NavigationItem
			{
				ClassList = new[] {"portal-menu__nav__item"},
				AnchorLink = new NavigationItem.AnchorLinkItem
				{
					Label = "My Accounts",
					TestId = "main-navigation-my-accounts-link",
					ClassList = new[] {"portal-menu__nav__item__link"},
					FlowAction = new NavigationItem.FlowActionItem
					{
						FlowActionType = FlowActionType.StartFlow,
						FlowLocation = StartFlowLocation.NotContained,
						FlowName = ResidentialPortalFlowType.Accounts.ToString()
					}
				}
			};
		}

		private static NavigationItem BuildMainProductsAndServices()
		{
			return new NavigationItem
			{
				ClassList = new[] {"portal-menu__nav__item"},
				AnchorLink = new NavigationItem.AnchorLinkItem
				{
					Label = "Products & Services",
					TestId = "main-navigation-products-and-services-link",
					ClassList = new[] {"portal-menu__nav__item__link"},
					FlowAction = new NavigationItem.FlowActionItem
					{
						FlowActionType = FlowActionType.StartFlow,
						FlowLocation = StartFlowLocation.ContainedInMe,
						FlowName = ResidentialPortalFlowType.ProductAndServices.ToString()
					}
				}
			};
		}

		private static NavigationItem BuildMainHelp()
		{
			return new NavigationItem
			{
				ClassList = new[] {"portal-menu__nav__item"},
				AnchorLink = new NavigationItem.AnchorLinkItem
				{
					Label = "Help",
					TestId = "main-navigation-help-link",
					ClassList = new[] {"portal-menu__nav__item__link"},
					FlowAction = new NavigationItem.FlowActionItem
					{
						FlowActionType = FlowActionType.StartFlow,
						FlowLocation = StartFlowLocation.ContainedInMe,
						FlowName = ResidentialPortalFlowType.Help.ToString()
					}
				}
			};
		}

	}
}