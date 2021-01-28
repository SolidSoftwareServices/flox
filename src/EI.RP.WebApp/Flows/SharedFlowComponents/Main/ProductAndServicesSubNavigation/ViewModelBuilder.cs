using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DomainServices.Queries.Competitions;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.UiFlows.Mvc.FlowTagHelpers;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Infrastructure.Flows;
using EI.RP.WebApp.Infrastructure.Settings;
using EI.RP.WebApp.Infrastructure.StringResources;
using Microsoft.AspNetCore.Http;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.ProductAndServicesSubNavigation
{
    internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
    {
        private readonly IDomainQueryResolver _queryResolver;
        private readonly IUserSessionProvider _sessionProvider;
        private readonly IUiAppSettings _settings;
        private readonly IHttpContextAccessor _contextAccessor;

        public ViewModelBuilder(IDomainQueryResolver queryResolver,
            IUserSessionProvider sessionProvider,
            IUiAppSettings settings,
            IHttpContextAccessor contextAccessor)
        {
            _queryResolver = queryResolver;
            _sessionProvider = sessionProvider;
            _settings = settings;
            _contextAccessor = contextAccessor;
        }

        public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
        {
            var result = new ViewModel();

            var navigationItems = new List<NavigationItem>
            {
                BuildNavigationItem(
                    "Products & Services",
                    "sub-navigation-item-products-and-services",
                    componentInput.IsProductsAndServices,
                    ResidentialPortalFlowType.ProductAndServices)
            };

            if (await IsCompetitionsAvailable())
            {
                navigationItems.Add(
                    BuildNavigationItem(
                        "Competitions",
                        "sub-navigation-item-competitions",
                        componentInput.IsCompetitions,
                        ResidentialPortalFlowType.CompetitionEntry));
            }

            if (IsPromotionsAvailable())
            {
                navigationItems.Add(
                    BuildNavigationItem(
                        "Promotions",
                        "sub-navigation-item-promotions",
                        componentInput.IsPromotions,
                        ResidentialPortalFlowType.PromotionEntry));
            }

            result.NavigationItems = navigationItems.ToArray();

            return result;

            async Task<bool> IsCompetitionsAvailable()
            {
                var competitionEntries = _queryResolver.GetCompetitionEntriesByUserName(_sessionProvider.CurrentUserClaimsPrincipal.Identity.Name);
                var answer = (await competitionEntries).SingleOrDefault()?.Answer;

                return _settings.IsCompetitionEnabled && string.IsNullOrEmpty(answer);
            }

            bool IsPromotionsAvailable()
            {
                return _settings.IsPromotionsEnabled;
            }
        }

        private static NavigationItem BuildNavigationItem(
            string label,
            string testId,
            bool isActive = false,
            ResidentialPortalFlowType flowType = ResidentialPortalFlowType.NoFlow)
        {
            return new NavigationItem
            {
                ClassList = new[] { isActive ? "active" : string.Empty },
                AnchorLink = new NavigationItem.AnchorLinkItem
                {
                    Label = label,
                    TestId = testId,
                    FlowAction = flowType != ResidentialPortalFlowType.NoFlow
                        ? new NavigationItem.FlowActionItem
                        {
                            FlowActionType = FlowActionTagHelper.FlowActionType.StartFlow,
                            FlowLocation = FlowActionTagHelper.StartFlowLocation.SameContainerAsMe,
                            FlowName = flowType.ToString()
                        }
                        : null
                }
            };
        }
    }
}