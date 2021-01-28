using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.UiFlows.Mvc.FlowTagHelpers;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCardsContainer;
using EI.RP.WebApp.Flows.AppFlows.Accounts.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountsSubNavigation
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private readonly IDomainQueryResolver _queryResolver;

		public ViewModelBuilder(IDomainQueryResolver queryResolver)
		{
			_queryResolver = queryResolver;
		}

		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			var accountsOverview = _queryResolver.GetAccountsOverview();


			var result = new ViewModel
            {
                IsMultiPageView = (await accountsOverview).ResolveIsMultipageView(componentInput.IsOpen)
            };

            if (!result.IsMultiPageView) return result;

            var navigationItems = new List<NavigationItem>();
            var accounts = (await accountsOverview).Where(x=>x.IsOpen==componentInput.IsOpen ||x.IsPendingOpening()).ToArray();

            if (accounts.Any(x=>x.IsElectricityAccount()))
            {
                navigationItems.Add(
                    BuildNavigationItem(
                        componentInput.AccountType,
                        "Electricity",
                        "sub-navigation-item-electricity",
                        ClientAccountType.Electricity,
                        componentInput.IsOpen));
            }

            if (accounts.Any(x=>x.IsGasAccount()))
            {
                navigationItems.Add(
                    BuildNavigationItem(
                        componentInput.AccountType,
                        "Gas",
                        "sub-navigation-item-gas",
                        ClientAccountType.Gas,
                        componentInput.IsOpen));
            }

            if (accounts.Any(x=>x.IsEnergyServicesAccount()))
            {
                navigationItems.Add(
                    BuildNavigationItem(
                        componentInput.AccountType,
                        "Energy Services",
                        "sub-navigation-item-energy-services",
                        ClientAccountType.EnergyService,
                        componentInput.IsOpen));
            }

            result.NavigationItems = navigationItems.ToArray();

            return result;
        }

        private static NavigationItem BuildNavigationItem(ClientAccountType selectedAccountType, 
            string label, string testId, ClientAccountType accountType, bool isOpen)
        {
            var isSelectedType = selectedAccountType == accountType;

            return new NavigationItem
            {
                ClassList = new[] { isSelectedType ? "active" : string.Empty },
                AnchorLink = new NavigationItem.AnchorLinkItem
                {
                    Label = label,
                    TestId = testId,
                    FlowAction = new NavigationItem.FlowActionItem
                    {
                        FlowActionType = FlowActionTagHelper.FlowActionType.ReloadStepWithChanges,
                        FlowLocation = FlowActionTagHelper.StartFlowLocation.SameContainerAsMe,
                        FlowName = ResidentialPortalFlowType.Accounts.ToString(),
                        FlowParameters = new AccountsFlowInput
                        {
                            AccountTypeValue = accountType,
                            IsOpen = isOpen,
                            PageIndex = 1
                        }
                    }
                }
            };
        }
	}
}