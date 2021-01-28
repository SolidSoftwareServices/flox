using System;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using static EI.RP.UiFlows.Mvc.FlowTagHelpers.FlowActionTagHelper;
using EI.RP.WebApp.Infrastructure.Flows;
using EI.RP.WebApp.Flows.AppFlows.TermsInfo.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.TermsInfo.Steps;

namespace EI.RP.WebApp.Flows.AppFlows.AccountDashboard.Components.AccountDashboardFooter
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{

		public Task<ViewModel> Resolve(InputModel componentInput,
			UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			var result = new ViewModel
			{
				NavigationItems = FixedNavigationItems
			};

			return Task.FromResult(result);
		}

		private static readonly NavigationItem[] FixedNavigationItems=new[]
		{
			BuildNavHelp(),
			BuildNavContactUs(),
			BuildNavTermsAndConditions(),
			BuildNavDisclaimer(),
			BuildNavPrivacyNotice()
		};
		private static NavigationItem BuildNavHelp()
			{
				return new NavigationItem
				{
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Help",
						TestId = "footer-help-link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.ContainedInMe,
							FlowName = ResidentialPortalFlowType.Help.ToString()
						}
					}
				};
			}

		private static NavigationItem BuildNavContactUs()
			{
				return new NavigationItem
				{
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Contact Us",
						TestId = "footer-contact-us-link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.ContainedInMe,
							FlowName = ResidentialPortalFlowType.ContactUs.ToString()
						}
					}
				};
			}

			private static NavigationItem BuildNavTermsAndConditions()
			{
				return new NavigationItem
				{
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Terms & Conditions",
						TestId = "footer-terms-and-conditions-link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.ContainedInMe,
							FlowName = ResidentialPortalFlowType.TermsInfo.ToString(),
							FlowParameters = new TermsInfoInput
							{
								StartType = TermsInfoFlowInitializer.StartType.TermsAndConditions
							}
						}
					}
				};
			}

			private static NavigationItem BuildNavDisclaimer()
			{
				return new NavigationItem
				{
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Disclaimer",
						TestId = "footer-disclaimer-link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.ContainedInMe,
							FlowName = ResidentialPortalFlowType.TermsInfo.ToString(),
							FlowParameters = new TermsInfoInput
							{
								StartType = TermsInfoFlowInitializer.StartType.Disclaimer
							}
						}
					}
				};
			}

			private static NavigationItem BuildNavPrivacyNotice()
			{
				return new NavigationItem
				{
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Privacy Notice and Cookies Policy",
						TestId = "footer-privacy-notice-link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.ContainedInMe,
							FlowName = ResidentialPortalFlowType.TermsInfo.ToString(),
							FlowParameters = new TermsInfoInput
							{
								StartType = TermsInfoFlowInitializer.StartType.Privacy
							}
						}
					}
				};
			}
	}
}