using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System.Paging;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;


namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.BillsAndPaymentsTable
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
			var billAndPaymentDetailsTask =
				_domainQueryResolver.GetAccountBillingsByAccountNumber(componentInput.AccountNumber);
			var accountInfoTask = _domainQueryResolver.GetAccountInfoByAccountNumber(componentInput.AccountNumber);

			var result = new ViewModel
			{
				ScreenModel = screenModelContainingTheComponent,
				ContainedInFlowType = componentInput.ContainedInFlowType,
				AccountNumber = componentInput.AccountNumber,
				IsPagingEnabled = componentInput.IsPagingEnabled,
				NumberOfPageLinks = componentInput.NumberPagingLinks,
				Paging = (await billAndPaymentDetailsTask)
					.OrderByDescending(x => x.OriginalDate)
					.Select(x =>
						new ViewModel.Row
						{
							Date = x.OriginalDate.ToString("dd/MM/yyyy"),
							Description = x.Description,
							DueAmount = x.DueAmount.ToString().Replace("€0.00", "-"),
							ReceivedAmount = x.ReceivedAmount.ToString().Replace("€0.00", "-"),
							HasInvoiceFile = x.InvoiceFileAvailable,
							ReferenceDocNumber = x.ReferenceDocNumber
						})
					.ToArray()
					.ToPagedData(componentInput.PageSize, componentInput.PageIndex),
				TableId = componentInput.TableId,
				PaginationId = componentInput.PaginationId,
				WhenChangingPageFocusOn = componentInput.WhenChangingPageFocusOn,
				IsAccountClosed = !(await accountInfoTask).IsOpen
			};

			return result;
		}
	}
}