using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Platform;
using Ei.Rp.DomainModels.Billing;
using EI.RP.DomainServices.Infrastructure.Settings;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.WebApp.Flows.AppFlows.AccountAndMeterDetails.FlowDefinitions;
using static EI.RP.UiFlows.Mvc.FlowTagHelpers.FlowActionTagHelper;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.Plan.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.Usage.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Flows;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Billing.LatestBill;
using EI.RP.WebApp.Flows.AppFlows.MakeAPayment.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Steps;

namespace EI.RP.WebApp.Flows.AppFlows.AccountDashboard.Components.AccountDashboardSubNavigation
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private readonly IDomainSettings _domainSettings;
		private readonly IDomainQueryResolver _domainQueryResolver;

		public ViewModelBuilder(IDomainSettings domainSettings, IDomainQueryResolver queryResolver)
		{
			_domainSettings = domainSettings;
			_domainQueryResolver = queryResolver;
		}

		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			var account =
				await _domainQueryResolver.GetAccountInfoByAccountNumber(componentInput.AccountNumber);

			var result = new ViewModel
			{
				AccountNumber = account.AccountNumber
			};

			var navItems = new List<NavigationItem>();
			if (account.IsOpen)
			{
				navItems.AddRange(new[]
				{
					BuildNavUsage(),
					BuildNavPayments(),
					BuildNavDetails()
				});
				navItems.Add(BuildNavPlan());
                navItems.Add(BuildNavMeterReadings());

				if (!_domainSettings.IsInternalDeployment)
				{
                    navItems.Add(BuildNavMovingHouse());
				}
			}
			else
			{
				navItems.Add(BuildNavUsage());
				navItems.Add(BuildNavPayments());
				if (account.ClientAccountType.IsOneOf(ClientAccountType.Electricity, ClientAccountType.Gas))
				{
					var latestBill =
							await _domainQueryResolver.GetLatestBillByAccountNumber(account.AccountNumber);
					if (latestBill.CurrentBalanceAmount > 0)
					{
						navItems.Add(BuildNavMakeAPayment());
					}
				}
				
				navItems.Add(BuildNavDetails());
			}

			
			if(account.ContractStatus == ContractStatusType.Pending)
			{
				navItems.Add(BuildNavPlan());
			}

			result.NavigationItems = navItems.OrderBy(x => x.Order).ToArray();

			return result;

			NavigationItem BuildNavUsage()
			{
                return new NavigationItem
                {
					Order = 1,
                    RelatedFlowTypes = new[]
                    {
                        ResidentialPortalFlowType.Usage
                    },
                    AnchorLink = new NavigationItem.AnchorLinkItem
                    {
                        Label = "Usage",
                        TestId = "nav-usage-link",
                        FlowAction = new NavigationItem.FlowActionItem
                        {
                            FlowActionType = FlowActionType.StartFlow,
                            FlowLocation = StartFlowLocation.ContainedInMe,
                            FlowName = ResidentialPortalFlowType.Usage.ToString(),
                            FlowParameters = new UsageInput
                            {
                                AccountNumber = account.AccountNumber
                            }
                        }
                    }
                };
			}

			NavigationItem BuildNavPayments()
			{
				return new NavigationItem
				{
					Order = 2,
					RelatedFlowTypes = new[]
						{
							ResidentialPortalFlowType.AccountsPaymentConfiguration,
							ResidentialPortalFlowType.MakeAPayment,
							ResidentialPortalFlowType.RequestRefund
						},
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Bills & Payments",
						TestId = "nav-payments-link",
						FlowAction = new NavigationItem.FlowActionItem
						{
                            FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.ContainedInMe,
							FlowName = ResidentialPortalFlowType.AccountsPaymentConfiguration.ToString(),
							FlowParameters = new AccountsPaymentConfigurationFlowInput
							{
								AccountNumber = account.AccountNumber,
								SecondaryAccountNumber = (string)null,
								StartType = AccountsPaymentConfigurationFlowStartType.ShowHistory
							}
						}
					}
				};
			}

			NavigationItem BuildNavPlan()
			{
				return new NavigationItem
				{
					Order = 3,
					RelatedFlowTypes = new[]
					{
						ResidentialPortalFlowType.Plan,
						ResidentialPortalFlowType.AddGasAccount
					},
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Plan",
						TestId = "nav-plan-link",
						FlowAction = new NavigationItem.FlowActionItem
						{
						FlowActionType = FlowActionType.StartFlow,
						FlowLocation = StartFlowLocation.ContainedInMe,
						FlowName = ResidentialPortalFlowType.Plan.ToString(),
						FlowParameters = new PlanInput
						{
							AccountNumber = account.AccountNumber
						}
					}
					}
				};
			}
			NavigationItem BuildNavMakeAPayment()
			{
				return new NavigationItem
				{
					Order = 3,
					RelatedFlowTypes = new[]
					{
						ResidentialPortalFlowType.MakeAPayment
					},
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Make A Payment",
						TestId = "nav-make-payment-link",
						FlowAction = new NavigationItem.FlowActionItem
						{
							FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.ContainedInMe,
							FlowName = ResidentialPortalFlowType.MakeAPayment.ToString(),
							FlowParameters = new MakeAPaymentInput
							{
								AccountNumber = account.AccountNumber,
								StartType=PaymentFlowInitializer.StartType.FromLastBill
							}
						}
					}
				};
			}
			NavigationItem BuildNavDetails()
			{
				return new NavigationItem
				{
					Order = 4,
					RelatedFlowTypes = new[]
						{
							ResidentialPortalFlowType.AccountAndMeterDetails
						},
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Details",
						TestId = "nav-details-link",
						FlowAction = new NavigationItem.FlowActionItem
						{
                            FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.ContainedInMe,
							FlowName = ResidentialPortalFlowType.AccountAndMeterDetails.ToString(),
							FlowParameters = new AccountAndMeterDetailsInput
							{
								AccountNumber = account.AccountNumber
							}
						}
					}
				};
			}

			NavigationItem BuildNavMeterReadings()
			{
				return new NavigationItem
				{
					Order = 5,
					RelatedFlowTypes = new[]
						{
							ResidentialPortalFlowType.MeterReadings
						},
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Meter Reading",
						TestId = "nav-meter-reading-link",
						FlowAction = new NavigationItem.FlowActionItem
						{
                            FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.ContainedInMe,
							FlowName = ResidentialPortalFlowType.MeterReadings.ToString(),
							FlowParameters = new MeterReadingsInput
							{
								AccountNumber = account.AccountNumber,
								StartType = MeterReadingFlowInitializer.StartType.ShowAndEditMeterReading
							}
						}
					}
				};
			}

			NavigationItem BuildNavMovingHouse()
			{
				return new NavigationItem
				{
					Order = 6,
					RelatedFlowTypes = new[]
						{
							ResidentialPortalFlowType.MovingHouse
						},
					AnchorLink = new NavigationItem.AnchorLinkItem
					{
						Label = "Moving House",
						TestId = "nav-moving-house-link",
						FlowAction = new NavigationItem.FlowActionItem
						{
                            FlowActionType = FlowActionType.StartFlow,
							FlowLocation = StartFlowLocation.ContainedInMe,
							FlowName = ResidentialPortalFlowType.MovingHouse.ToString(),
							FlowParameters = new MovingHouseInput
							{
								InitiatedFromAccountNumber = account.AccountNumber
							}
						}
					}
				};
			}
		}
	}
}