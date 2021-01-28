using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DomainServices.Queries.Competitions;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.UiFlows.Mvc.FlowTagHelpers;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Steps;
using EI.RP.WebApp.Infrastructure.Flows;
using EI.RP.WebApp.Infrastructure.Settings;
using EI.RP.WebApp.Infrastructure.StringResources;
using Microsoft.AspNetCore.Http;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.CompetitionsBanner
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IUiAppSettings _settings;
		private readonly IUserSessionProvider _sessionProvider;
		private readonly IHttpContextAccessor _contextAccessor;

		public ViewModelBuilder(IDomainQueryResolver queryResolver,
			IUiAppSettings settings,
			IUserSessionProvider sessionProvider,
			IHttpContextAccessor contextAccessor)
		{
			_queryResolver = queryResolver;
			_settings = settings;
			_sessionProvider = sessionProvider;
			_contextAccessor = contextAccessor;
		}

		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			if (componentInput == null)
			{
				throw new ArgumentException(nameof(componentInput));
			}
			if (componentInput.AccountNumber == null)
			{
				throw new ArgumentException(nameof(componentInput.AccountNumber));
			}
			if (componentInput.ToCompetitionEvent == null)
			{
				throw new ArgumentException(nameof(componentInput.ToCompetitionEvent));
			}
			if (componentInput.DismissBannerEvent == null)
			{
				throw new ArgumentException(nameof(componentInput.DismissBannerEvent));
			}

			var competitionEntries = _queryResolver.GetCompetitionEntriesByUserName(_sessionProvider.CurrentUserClaimsPrincipal.Identity.Name);
			var result = new ViewModel
			{
				Visible = _settings.IsCompetitionEnabled && !IsCompetitionBannerDismissed(),
				FlowAction = GetFlowAction(),
				DismissBannerFlowAction = GetDismissFlowAction(),
				Answer = (await competitionEntries).SingleOrDefault()?.Answer,
				AccountNumber = componentInput.AccountNumber,
				Heading = _settings.CompetitionHeading,
				ImageDesktop = _settings.CompetitionBannerImages?.RegularImagePath,
				ImageMobile =  _settings.CompetitionBannerImages?.MobileImagePath,
				ImageAltText = _settings.CompetitionBannerImages?.AltText
			};

			return result;

			NavigationItem.FlowActionItem GetFlowAction()
			{
				return new NavigationItem.FlowActionItem
				{
					TriggerEvent = componentInput.ToCompetitionEvent,
					EventFlowType = ResidentialPortalFlowType.Accounts,
					EventAdditionalFields = new[]
					{
							new NavigationItem.EventAdditionalField
							{
								PropertyPath = $"{nameof(AccountSelection.ScreenModel.SelectedAccount)}.{nameof(AccountSelection.ScreenModel.AccountSelectionData.AccountNumber)}",
								Value = componentInput.AccountNumber
							}
						}
				};
			}

			NavigationItem.FlowActionItem GetDismissFlowAction()
			{
				return new NavigationItem.FlowActionItem
				{
					TriggerEvent = componentInput.DismissBannerEvent,
					EventFlowType = ResidentialPortalFlowType.Accounts,
					EventAdditionalFields = new[]
					{
						new NavigationItem.EventAdditionalField
						{
							PropertyPath =
								$"{nameof(AccountSelection.ScreenModel.SelectedAccount)}.{nameof(AccountSelection.ScreenModel.AccountSelectionData.AccountNumber)}",
							Value = componentInput.AccountNumber
						}
					}
				};
			}

			bool IsCompetitionBannerDismissed()
			{
				if (_contextAccessor.HttpContext.Request.Cookies.TryGetValue(ReusableString.CompetitionnDismissCookieKey, out var cookieValue))
				{
					return Convert.ToBoolean(cookieValue);
				}

				return false;
			}
		}
	}
}