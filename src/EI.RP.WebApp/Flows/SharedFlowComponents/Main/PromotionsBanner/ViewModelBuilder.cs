using System;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.UiFlows.Mvc.FlowTagHelpers;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Steps;
using EI.RP.WebApp.Infrastructure.Flows;
using EI.RP.WebApp.Infrastructure.Settings;
using EI.RP.WebApp.Infrastructure.StringResources;
using Microsoft.AspNetCore.Http;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.PromotionsBanner
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private readonly IUiAppSettings _settings;
		private readonly IHttpContextAccessor _contextAccessor;

		public ViewModelBuilder(IUiAppSettings settings, IHttpContextAccessor contextAccessor)
		{
			_settings = settings;
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
			if (componentInput.ToPromotionEvent == null)
			{
				throw new ArgumentException(nameof(componentInput.ToPromotionEvent));
			}
			if (componentInput.DismissBannerEvent == null)
			{
				throw new ArgumentException(nameof(componentInput.DismissBannerEvent));
			}

			return new ViewModel
			{
				Visible = _settings.IsPromotionsEnabled && !IsPromotionBannerDismissed(),
				FlowAction = GetFlowAction(),
				DismissBannerFlowAction = GetDismissFlowAction(),
				Heading = _settings.PromotionHeading,
				ImageDesktop = _settings.PromotionImageDesktop,
				ImageMobile = _settings.PromotionImageMobile
			};

			NavigationItem.FlowActionItem GetFlowAction()
			{
				return new NavigationItem.FlowActionItem
				{
					TriggerEvent = componentInput.ToPromotionEvent,
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

			bool IsPromotionBannerDismissed()
			{
				if (_contextAccessor.HttpContext.Request.Cookies.TryGetValue(ReusableString.PromotionDismissCookieKey, out var cookieValue))
				{
					return Convert.ToBoolean(cookieValue);
				}

				return false;
			}
		}
	}
}