using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.DomainServices.Queries.Billing.LatestBill;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Infrastructure.Settings;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.Components.PlanDetails
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private readonly IDomainQueryResolver _domainQueryResolver;

		public ViewModelBuilder(IDomainQueryResolver queryResolver)
		{
			_domainQueryResolver = queryResolver;
		}

		public async Task<ViewModel> Resolve(InputModel componentInput,
			UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			componentInput.FlowScreenModel = screenModelContainingTheComponent;
			var accounts = (await _domainQueryResolver.GetDuelFuelAccountsByAccountNumber(componentInput.AccountNumber)).ToArray();
			var accountInfo = accounts.Single(x=>x.AccountNumber==componentInput.AccountNumber);
			var latestBill = await _domainQueryResolver.GetLatestBillByAccountNumber(componentInput.AccountNumber);

			var result = new ViewModel
			{
				AccountNumber = componentInput.AccountNumber,
				PlanName = accountInfo.PlanName,
				Discount = accountInfo.DiscountAppliedPercentage,
				CanAddGas = latestBill.CanAddGasAccount && !accountInfo.IsSmart()
			};
			
			var couldUpgrade = !accountInfo.IsSmart();
			if (couldUpgrade)
			{
				result.Upgrading.IsUpgradeToSmartAvailable =  accountInfo.SmartActivationStatus == SmartActivationStatus.SmartAndEligible;
				if (result.Upgrading.IsUpgradeToSmartAvailable)
				{
					result.Upgrading.UpgradePlanName = accounts.Length == 1 ? "Home Electric+" : "Home Dual+";
				}
			}

			return result;
		}
	}
}