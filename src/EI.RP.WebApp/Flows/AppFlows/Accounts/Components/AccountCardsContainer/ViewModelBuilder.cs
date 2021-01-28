using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System.Paging;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Infrastructure.PresentationServices.EventsPublisher;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCardsContainer
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private const int SubCategoryId = 1202;
        private const int CategoryId = 120;

        private readonly IUIEventPublisher _eventsPublisher;
        private readonly IDomainQueryResolver _queryResolver;

        public ViewModelBuilder(IUIEventPublisher eventsPublisher,IDomainQueryResolver queryResolver)
        {
	        _eventsPublisher = eventsPublisher;
	        _queryResolver = queryResolver;
        }

        public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
        {
	        var getAccounts= _queryResolver.GetAccountsOverview();
            var result = new ViewModel
			{
				CanSubmitMeterReading = componentInput.HasStartedFromMeterReading,
                IsPagingEnabled = componentInput.IsPagingEnabled,
				NumberOfPageLinks = componentInput.NumberPagingLinks,
                RouteValues = new { AccountType = componentInput.AccountType?.ToString(), componentInput.IsOpen },
				ContainerId = componentInput.ContainerId,
				PaginationId = componentInput.PaginationId,
				WhenChangingPageFocusOn = componentInput.WhenChangingPageFocusOn
			};


			//TODO: use query filters
            var accountOverviews = (await getAccounts).ToArray();
            var accounts = (accountOverviews.ResolveIsMultipageView(componentInput.IsOpen)
		            ? accountOverviews.Where(x => x.ClientAccountType == componentInput.AccountType)
		            : accountOverviews)
	            .Where(x => x.IsOpen == componentInput.IsOpen || x.IsPendingOpening()).ToArray();

            if (accounts.Any())
            {
	            result.Paging = accounts
		            .Select(x=>new ViewModel.Row
		            {
			            AccountNumber = x.AccountNumber,
						Partner=x.Partner,
						IsOpen=x.IsOpen
		            })
		            .ToPagedData(componentInput.PageSize, componentInput.PageIndex);

	            if (result.Paging?.CurrentPageItems != null && result.Paging.CurrentPageItems.Any())
	            {
		            await PublishViewRequestedEvent(result);
	            }
            }

            return result;
        }

       

        private Task PublishViewRequestedEvent(ViewModel vm)
        {
            //as per the legacy system
            var account = vm.Paging.CurrentPageItems.First();
            return _eventsPublisher.Publish(new UiEventInfo
            {
                Description = "View Account Details",
                AccountNumber = account.AccountNumber,
                Partner = account.Partner,
                SubCategoryId = SubCategoryId,
                CategoryId = CategoryId
            });
        }
	}
}
