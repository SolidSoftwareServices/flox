using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Steps;
using EI.RP.WebApp.Infrastructure.Flows;
using EI.RP.WebApp.Infrastructure.PresentationServices.EventsPublisher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.SmartMeterActivationBanner
{
	public class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IUIEventPublisher _eventsPublisher;

		public ViewModelBuilder(IDomainQueryResolver queryResolver,
			IUIEventPublisher eventsPublisher)
		{
			_queryResolver = queryResolver;
			_eventsPublisher = eventsPublisher;
		}

		public async Task<ViewModel> Resolve(InputModel componentInput,
			UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			if (componentInput == null)
			{
				throw new ArgumentNullException(nameof(componentInput));
			}
			
			if (componentInput.ToSmartActivationEvent == null)
			{
				throw new ArgumentException(nameof(componentInput.ToSmartActivationEvent));
			}

			if (componentInput.DismissBannerEvent == null)
			{
				throw new ArgumentException(nameof(componentInput.DismissBannerEvent));
			}


			var accountsSource = await _queryResolver.GetAccounts();
			var eligibleAccounts = accountsSource.Where(x =>
				(x.IsOpen || x.ContractStatus == ContractStatusType.Pending)
				&& x.ClientAccountType == ClientAccountType.Electricity
				&& !x.IsSmart()
				&& x.SmartActivationStatus == SmartActivationStatus.SmartAndEligible
				&& !x.SwitchToSmartPlanDismissed).ToArray();
		

			var viewModel = new ViewModel();
			if (eligibleAccounts.Any())
			{
				viewModel.CanOptToSmart = true;
				var cmdmPushTasks = new List<Task>(eligibleAccounts.Length);
				foreach (var account in eligibleAccounts)
				{
					viewModel.SmartActivationEligibleItems.Add(new ViewModel.NotificationViewModel
					{
						AccountNumber = account.AccountNumber,
						Mprn = account.PointReferenceNumber.ToString(),
						DismissNotificationAction = GetDismissFlowAction(account),
						FlowAction = GetFlowAction(account)
					});
					cmdmPushTasks.Add(PublishEventNotificationShownToUser(account));
				}

				Task.WaitAll(cmdmPushTasks.ToArray());
			}

			return viewModel;

			NavigationItem.FlowActionItem GetFlowAction(AccountInfo forAccount)
			{
				return new NavigationItem.FlowActionItem
				{
					TriggerEvent = componentInput.ToSmartActivationEvent,
					EventFlowType = ResidentialPortalFlowType.Accounts,
					EventAdditionalFields = new[]
					{
						new NavigationItem.EventAdditionalField
						{
							PropertyPath =
								$"{nameof(AccountSelection.ScreenModel.SelectedAccount)}.{nameof(AccountSelection.ScreenModel.AccountSelectionData.AccountNumber)}",
							Value = forAccount.AccountNumber
						},
						new NavigationItem.EventAdditionalField
						{
							PropertyPath = "SourceFlowType",
							Value = ResidentialPortalFlowType.Accounts.ToString()
						}
					}
				};
			}

			NavigationItem.FlowActionItem GetDismissFlowAction(AccountInfo forAccount)
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
							Value = forAccount.AccountNumber
						}
					}
				};
			}
		}

		private Task PublishEventNotificationShownToUser(AccountInfo forAccount)
		{
			return _eventsPublisher.Publish(new UiEventInfo
			{
				Description = "View Smart Activation Notice",
				AccountNumber = forAccount.AccountNumber,
				Partner = forAccount.Partner,
				MPRN = forAccount.PointReferenceNumber?.ToString(),
				SubCategoryId = EventSubCategory.ShowSmartActivationNotificationToUser,
				CategoryId = EventCategory.View
			});
		}
	}
}