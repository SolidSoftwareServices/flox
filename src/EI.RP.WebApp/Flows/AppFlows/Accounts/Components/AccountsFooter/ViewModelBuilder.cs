using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Steps;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountsFooter
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
				NavigationItems = new []
                {
                    BuildNavHelp(),
                    BuildNavContactUs(),
                    BuildNavTermsAndConditions(),
                    BuildNavDisclaimer(),
                    BuildNavPrivacyNotice()
                }
			};

            NavigationItem BuildNavHelp()
            {
                return new NavigationItem
                {
					AnchorLink = new NavigationItem.AnchorLinkItem 
                    {
						Label = "Help",
						TestId = "footer-help-link",
						FlowAction = new NavigationItem.FlowActionItem
                        {
                            TriggerEvent = AccountSelection.StepEvent.ToHelp,
                            EventFlowType = ResidentialPortalFlowType.Accounts,
                            EventAdditionalFields = eventAdditionalFields
                        }
                    }
                };
            }

            NavigationItem BuildNavContactUs()
            {
                return new NavigationItem
                {
                    AnchorLink = new NavigationItem.AnchorLinkItem 
                    {
                        Label = "Contact Us",
                        TestId = "footer-contact-us-link",
                        FlowAction = new NavigationItem.FlowActionItem
                        {
                            TriggerEvent = AccountSelection.StepEvent.ToContactUs,
                            EventFlowType = ResidentialPortalFlowType.Accounts,
                            EventAdditionalFields = eventAdditionalFields
                        }
                    }
                };
            }

            NavigationItem BuildNavTermsAndConditions()
            {
                return new NavigationItem
                {
                    AnchorLink = new NavigationItem.AnchorLinkItem 
                    {
                        Label = "Terms & Conditions",
                        TestId = "footer-terms-and-conditions-link",
                        FlowAction = new NavigationItem.FlowActionItem
                        {
                            TriggerEvent = AccountSelection.StepEvent.ToTermsAndConditions,
                            EventFlowType = ResidentialPortalFlowType.Accounts,
                            EventAdditionalFields = eventAdditionalFields
                        }
                    }
                };
            }

            NavigationItem BuildNavDisclaimer()
            {
                return new NavigationItem
                {
                    AnchorLink = new NavigationItem.AnchorLinkItem 
                    {
                        Label = "Disclaimer",
                        TestId = "footer-disclaimer-link",
                        FlowAction = new NavigationItem.FlowActionItem
                        {
                            TriggerEvent = AccountSelection.StepEvent.ToDisclaimer,
                            EventFlowType = ResidentialPortalFlowType.Accounts,
                            EventAdditionalFields = eventAdditionalFields
                        }
                    }
                };
            }

            NavigationItem BuildNavPrivacyNotice()
            {
                return new NavigationItem
                {
                    AnchorLink = new NavigationItem.AnchorLinkItem 
                    {
                        Label = "Privacy Notice and Cookies Policy",
                        TestId = "footer-privacy-notice-link",
                        FlowAction = new NavigationItem.FlowActionItem
                        {
                            TriggerEvent = AccountSelection.StepEvent.ToPrivacy,
                            EventFlowType = ResidentialPortalFlowType.Accounts,
                            EventAdditionalFields = eventAdditionalFields
                        }
                    }
                };
            }
        }
	}
}