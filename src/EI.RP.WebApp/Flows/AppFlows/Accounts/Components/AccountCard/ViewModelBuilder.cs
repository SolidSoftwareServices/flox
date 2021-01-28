using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCard
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private readonly IDomainQueryResolver _domainQueryResolver;

		public ViewModelBuilder(IDomainQueryResolver domainQueryResolver)
		{
			_domainQueryResolver = domainQueryResolver;
		}

		public async Task<ViewModel> Resolve(InputModel componentInput,
			UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			var accountInfo = await _domainQueryResolver.GetAccountInfoByAccountNumber(componentInput.AccountNumber);
			var result = new ViewModel
			{
				CanSubmitMeterReading = componentInput.CanSubmitMeterReading,
				ShowBillingDetails = componentInput.ShowBillingDetails,
				AccountNumber = accountInfo.AccountNumber,
				AccountType = accountInfo.ClientAccountType,
				AccountLocation = accountInfo.Description.Replace(",", ", "),
				AccountIsOpen = accountInfo.IsOpen
			};

			return result;
		}
	}
}