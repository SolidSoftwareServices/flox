using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using System.Linq;
using System;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.Header
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
			var canShowAccountDetails = !string.IsNullOrWhiteSpace(componentInput.AccountNumber);

			var result = new ViewModel 
			{
				ShowAccountDetails = canShowAccountDetails && (componentInput.ShowAccountNumber || componentInput.ShowAddress),
				ShowAccountNumber = componentInput.ShowAccountNumber,
				ShowAddress = componentInput.ShowAddress
			};

			AccountInfo account=null;
			if (result.ShowAccountDetails)
			{
				account = await _domainQueryResolver.GetAccountInfoByAccountNumber(componentInput.AccountNumber);

				result.AccountNumber = account.AccountNumber;
				result.AccountDescription = account.Description.Replace(",", ", ");
				ResolveLayoutPathFragment();
			}
			
			ResolveTitle(account?.ClientAccountType);

			return result;

			void ResolveLayoutPathFragment()
			{
				var elecGasClientAccountTypes = new[] {ClientAccountType.Electricity, ClientAccountType.Gas};
				result.LayoutPathFragment = elecGasClientAccountTypes.Contains(account.ClientAccountType)
					? "ElectricityAndGas"
					: account.ClientAccountType == ClientAccountType.EnergyService
						? "EnergyService"
						: throw new ArgumentOutOfRangeException();
			}

			void ResolveTitle(ClientAccountType clientAccountType = null)
			{
				var accountType = clientAccountType == ClientAccountType.EnergyService
						? "Energy Services"
						: clientAccountType;

				result.Title = componentInput.Title ?? accountType ?? string.Empty;
			}
		}
	}
}