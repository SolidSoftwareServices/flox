using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Billing.LatestBill;

using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;


namespace EI.RP.WebApp.Flows.AppFlows.EnergyServicesAccountOverview.Components.EnergyServicesBillingDetails
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
			var latestBillInfo = 
				await _domainQueryResolver.GetLatestBillByAccountNumber(componentInput.AccountNumber);

			var result = new ViewModel
			{
				AccountDescription = latestBillInfo.AccountDescription,
				Amount = latestBillInfo.Amount,
				PaymentMethod = latestBillInfo.PaymentMethod,
				PaymentDate = latestBillInfo.DueDate
			};

			return result;
		}
	}
}