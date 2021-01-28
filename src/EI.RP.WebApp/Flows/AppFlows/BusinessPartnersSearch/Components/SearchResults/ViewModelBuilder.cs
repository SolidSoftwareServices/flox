using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System.Paging;
using EI.RP.DomainServices.Queries.Contracts.BusinessPartners;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.Components.SearchResults
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
			var getPartners = componentInput.BusinessPartnersIdToShow
				.Select(async x =>
				{
					var bp = await _domainQueryResolver.GetBusinessPartner(x);
					return new ViewModel.Row
					{
						PartnerNumber = bp.NumPartner,
						UserName = componentInput.QueryUserName ?? string.Empty,
						Description = bp.Description,
					};
				});
			var result = new ViewModel
			{
				NumberOfPageLinks = componentInput.NumberPagingLinks,
				IsPagingEnabled = componentInput.IsPagingEnabled,
				ShowDeRegistrationColumn = !string.IsNullOrWhiteSpace(componentInput.QueryUserName),
				IsSingleUserBusinessPartner = componentInput.BusinessPartnersIdToShow.Count() == 1,
				Paging = (await Task.WhenAll(getPartners))
					.ToPagedData(componentInput.PageSize, componentInput.PageIndex, componentInput.IsPagingEnabled)
			};
			return result;
		}
	}
}