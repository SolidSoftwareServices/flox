using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.Flows.AppFlows.Accounts.FlowDefinitions;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.Infrastructure;
using EI.RP.DomainServices.Commands.Users.SmartActivationNotification.DismissSmartActivationNotification;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Infrastructure.PresentationServices.EventsPublisher;
using EI.RP.WebApp.Infrastructure.StringResources;
using Microsoft.AspNetCore.Http;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using ServiceStack.Text;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Steps
{
	public class AccountSelection : CustomerAccountsScreen
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent ViewFullAccountDetails =
				new ScreenEvent(nameof(AccountSelection), nameof(ViewFullAccountDetails));

			public static readonly ScreenEvent ToPage = new ScreenEvent(nameof(AccountSelection), nameof(ToPage));

			public static readonly ScreenEvent ToContactUs =
				new ScreenEvent(nameof(AccountSelection), nameof(ToContactUs));

			public static readonly ScreenEvent ToHelp = new ScreenEvent(nameof(AccountSelection), nameof(ToHelp));

			public static readonly ScreenEvent ToTermsAndConditions =
				new ScreenEvent(nameof(AccountSelection), nameof(ToTermsAndConditions));

			public static readonly ScreenEvent ToDisclaimer =
				new ScreenEvent(nameof(AccountSelection), nameof(ToDisclaimer));

			public static readonly ScreenEvent ToPrivacy = new ScreenEvent(nameof(AccountSelection), nameof(ToPrivacy));

			public static readonly ScreenEvent ToEditDirectDebit =
				new ScreenEvent(nameof(AccountSelection), nameof(ToEditDirectDebit));

			public static readonly ScreenEvent ToMakeAPayment =
				new ScreenEvent(nameof(AccountSelection), nameof(ToMakeAPayment));

			public static readonly ScreenEvent ToEstimateCost =
				new ScreenEvent(nameof(AccountSelection), nameof(ToEstimateCost));

			public static readonly ScreenEvent ToChangePassword =
				new ScreenEvent(nameof(AccountSelection), nameof(ToChangePassword));

			public static readonly ScreenEvent ToRequestRefund =
				new ScreenEvent(nameof(AccountSelection), nameof(ToRequestRefund));

			public static readonly ScreenEvent ToMyProfile =
				new ScreenEvent(nameof(AccountSelection), nameof(ToMyProfile));

			public static readonly ScreenEvent ToMarketingPreference =
				new ScreenEvent(nameof(AccountSelection), nameof(ToMarketingPreference));

			public static readonly ScreenEvent SubmitMeterReading =
				new ScreenEvent(nameof(AccountSelection), nameof(SubmitMeterReading));

			public static readonly ScreenEvent ToClosedAccounts =
				new ScreenEvent(nameof(AccountSelection), nameof(ToClosedAccounts));

			public static readonly ScreenEvent ToOpenAccounts =
				new ScreenEvent(nameof(AccountSelection), nameof(ToOpenAccounts));

			public static readonly ScreenEvent ToProductsAndServices =
				new ScreenEvent(nameof(AccountSelection), nameof(ToProductsAndServices));

			public static readonly ScreenEvent ToUsage = new ScreenEvent(nameof(AccountSelection), nameof(ToUsage));

			public static readonly ScreenEvent ToCompetition =
				new ScreenEvent(nameof(AccountSelection), nameof(ToCompetition));

			public static readonly ScreenEvent ToPromotion =
				new ScreenEvent(nameof(AccountSelection), nameof(ToPromotion));

			public static readonly ScreenEvent ToSmartActivation =
				new ScreenEvent(nameof(AccountSelection), nameof(ToSmartActivation));

			public static readonly ScreenEvent DismissSmartActivationNotification =
				new ScreenEvent(nameof(AccountSelection), nameof(DismissSmartActivationNotification));

			public static readonly ScreenEvent DismissCompetitionNotification =
				new ScreenEvent(nameof(AccountSelection), nameof(DismissCompetitionNotification));

			public static readonly ScreenEvent DismissPromotionsNotification =
				new ScreenEvent(nameof(AccountSelection), nameof(DismissPromotionsNotification));
		}

		private readonly IDomainQueryResolver _queryResolver;

		public override ScreenName ScreenStep => CustomerAccountsStep.AccountSelection;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.ToEditDirectDebit, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToContactUs, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToHelp, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToDisclaimer, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToPrivacy, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToTermsAndConditions, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToChangePassword, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToMakeAPayment, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.SubmitMeterReading, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToEstimateCost, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToRequestRefund, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToMarketingPreference, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ViewFullAccountDetails, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToProductsAndServices, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToUsage, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToCompetition, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToPromotion, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventNavigatesTo(StepEvent.ToSmartActivation, CustomerAccountsStep.LoadAccountDashboard)
				.OnEventsReentriesCurrent(new[]
				{
					StepEvent.ToClosedAccounts,
					StepEvent.ToOpenAccounts,
					StepEvent.DismissSmartActivationNotification,
					StepEvent.DismissCompetitionNotification,
					StepEvent.DismissPromotionsNotification,
				});
		}

		private readonly IUserSessionProvider _userSessionProvider;
		private readonly IUIEventPublisher _eventsPublisher;
		private readonly IDomainCommandDispatcher _commandDispatcher;
		private readonly IHttpContextAccessor _contextAccessor;

		public AccountSelection(IDomainQueryResolver queryResolver,
			IUserSessionProvider userSessionProvider,
			IUIEventPublisher eventsPublisher,
			IDomainCommandDispatcher commandDispatcher,
			IHttpContextAccessor contextAccessor)
		{
			_queryResolver = queryResolver;
			_userSessionProvider = userSessionProvider;
			_eventsPublisher = eventsPublisher;
			_commandDispatcher = commandDispatcher;
			_contextAccessor = contextAccessor;
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
			
			var data = new ScreenModel
			{
				UiFlowInitiator = rootData.UiFlowInitiator,
				HasStartedFromMeterReading = rootData.UiFlowInitiator == AppLoginType.MeterReading,
				UserAccountNumbers = rootData.UserAccountNumbers,
				IsAgentUser = rootData.IsAgentUser,

			};

			await ResolveAccountSelectionOnStartup(data);
			await BuildStepData(data);

			return data;
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var result = false;
			var refreshedStepData = originalScreenModel.CloneDeep();
			var data = (ScreenModel) refreshedStepData;

			if (stepViewCustomizations != null)
			{
				result |= data.SetFlowCustomizableValue(stepViewCustomizations, x => x.PageIndex);
				result |= data.SetFlowCustomizableValue(stepViewCustomizations, x => x.SelectedAccount.IsOpen);
				result |= data.SetFlowCustomizableValue(stepViewCustomizations,
					x => x.SelectedAccount.AccountTypeValue);
			}

			await ResolveAccountSelection(data);
			await BuildStepData(data);

			return data;
		}

		private async Task ResolveAccountSelectionOnStartup(ScreenModel screenModel)
		{
			var accounts = (await _queryResolver.GetAccountsOverview())
				.Where(x => screenModel.UserAccountNumbers.Contains(x.AccountNumber))
				.ToArray();

			
			var hasOpenAccounts = accounts.Any(x => x.IsOpen);
			screenModel.SelectedAccount.IsOpen = hasOpenAccounts;

			var accountsByStatus = accounts.Where(x => x.IsOpen == hasOpenAccounts).ToArray();
			var hasElectricityAccounts =
				accountsByStatus.Any(x => x.ClientAccountType == ClientAccountType.Electricity);
			var hasGasAccounts =
				accountsByStatus.Any(x => x.ClientAccountType == ClientAccountType.Gas);
			var hasEnergyServiceAccounts =
				accountsByStatus.Any(x => x.ClientAccountType == ClientAccountType.EnergyService);

			if (hasElectricityAccounts)
			{
				screenModel.SelectedAccount.AccountType = ClientAccountType.Electricity;
			}
			else if (hasGasAccounts)
			{
				screenModel.SelectedAccount.AccountType = ClientAccountType.Gas;
			}
			else if (hasEnergyServiceAccounts)
			{
				screenModel.SelectedAccount.AccountType = ClientAccountType.EnergyService;
			}
			else
			{
				throw new ArgumentOutOfRangeException();
			}

			await ResolveAccountSelection(screenModel);
		}

		private async Task ResolveAccountSelection(ScreenModel screenModel)
		{
			if (screenModel.SelectedAccount.AccountType == null && screenModel.SelectedAccount.AccountNumber != null)
			{
				var account =
					await _queryResolver.GetAccountInfoByAccountNumber(screenModel.SelectedAccount.AccountNumber);

				screenModel.SelectedAccount.AccountType = account.ClientAccountType;
				screenModel.SelectedAccount.IsOpen =
					account.IsOpen || account.ContractStatus == ContractStatusType.Pending;
			}
		}

		private async Task BuildStepData(ScreenModel screenModel)
		{
			var accounts = (await _queryResolver.GetAccountsOverview())
				.Where(x => screenModel.UserAccountNumbers.Contains(x.AccountNumber))
				.ToArray();
			screenModel.UserAccounts = accounts;


			screenModel.DefaultAccountNumber = screenModel.UserAccountNumbers.FirstOrDefault();

			screenModel.HasOpenAccounts = accounts.Any(x => x.IsOpen || x.ContractStatus == ContractStatusType.Pending);
			screenModel.HasClosedAccounts =
				accounts.Any(x => !x.IsOpen && x.ContractStatus != ContractStatusType.Pending);


			screenModel.ToSmartActivationEvent = StepEvent.ToSmartActivation;
			screenModel.DismissSmartActivationBannerEvent = StepEvent.DismissSmartActivationNotification;

			screenModel.ToCompetitionEvent = StepEvent.ToCompetition;
			screenModel.DismissCompetitionBannerEvent = StepEvent.DismissCompetitionNotification;

			screenModel.ToPromotionEvent = StepEvent.ToPromotion;
			screenModel.DismissPromotionBannerEvent = StepEvent.DismissPromotionsNotification;
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
		{
			var stepData = contextData.GetCurrentStepData<ScreenModel>();
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);

			var eventsToAccountDetails = new[]
			{
				StepEvent.ViewFullAccountDetails,
				StepEvent.ToContactUs,
				StepEvent.ToHelp,
				StepEvent.ToTermsAndConditions,
				StepEvent.ToDisclaimer,
				StepEvent.ToPrivacy,
				StepEvent.ToEditDirectDebit,
				StepEvent.ToMakeAPayment,
				StepEvent.ToEstimateCost,
				StepEvent.ToChangePassword,
				StepEvent.ToRequestRefund,
				StepEvent.SubmitMeterReading,
				StepEvent.ToMarketingPreference,
				StepEvent.ToProductsAndServices,
				StepEvent.ToUsage,
				StepEvent.ToCompetition,
				StepEvent.ToPromotion,
				StepEvent.ToSmartActivation
			};

			if (eventsToAccountDetails.Any(x => x.Equals(triggeredEvent)))
			{
				rootData.SelectedUserAccountNumber = stepData.SelectedAccount.AccountNumber;
				contextData.SetStepData(ScreenName.PreStart, rootData);
				if (triggeredEvent == StepEvent.ToSmartActivation)
				{
					await PublishEventSmartActivationJourneyThroughNotification(rootData.SelectedUserAccountNumber);
				}
			}
			else if (triggeredEvent == StepEvent.DismissSmartActivationNotification)
			{
				await _commandDispatcher.ExecuteAsync(
					new DismissSmartActivationNotificationCommand(stepData.SelectedAccount.AccountNumber));
				stepData.UserAccountNumbers = (await _queryResolver.GetAccountsOverview()).Select(x=>x.AccountNumber).ToArray();
			}
			else if (triggeredEvent == StepEvent.DismissCompetitionNotification)
			{
				SetCookie(ReusableString.CompetitionnDismissCookieKey, true, DateTime.Now.FirstDayOfNextMonth(), true);
			}
			else if (triggeredEvent == StepEvent.DismissPromotionsNotification)
			{
				SetCookie(ReusableString.PromotionDismissCookieKey, true, DateTime.UtcNow.AddHours(3), true);
			}
		}

		async Task PublishEventSmartActivationJourneyThroughNotification(string accountNumber)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(accountNumber);
			var contractItem = account.BusinessAgreement.Contracts.FirstOrDefault();
			var mprn = contractItem?.Premise?.PointOfDeliveries?.FirstOrDefault()?.Prn ?? string.Empty;
			await _eventsPublisher.Publish(new UiEventInfo
			{
				Description = "Smart Activation Through Notification",
				AccountNumber = account.AccountNumber,
				Partner = account.Partner,
				MPRN = mprn,
				SubCategoryId = EventSubCategory.SmartActivationThroughNotification,
				CategoryId = EventCategory.SmartActivation
			});
		}

		private void SetCookie(string key, bool value, DateTime expireTime, bool isEssential = false)
		{
			SetCookie(key, value.ToString(), expireTime, isEssential);
		}

		private void SetCookie(string key, string value, DateTime expireTime, bool isEssential = false)
		{
			_contextAccessor.HttpContext.Response.Cookies.Append(key, value,
				new CookieOptions {Expires = expireTime, IsEssential = isEssential});
		}

		public class ScreenModel : UiFlowScreenModel
		{
			[JsonIgnore] 
			public IEnumerable<AccountOverview> UserAccounts { get; set; }
			public IEnumerable<string> UserAccountNumbers { get; set; } = new string[0];

			public int PageIndex { get; set; } = 1;

			public bool HasStartedFromMeterReading { get; set; }

			public class AccountSelectionData
			{
				public string AccountNumber { get; set; }

				public string AccountTypeValue
				{
					get => AccountType?.ToString();
					set => AccountType = value != null
						? (ClientAccountType) value
						: null;
				}

				public ClientAccountType AccountType { get; set; }

				public bool IsOpen { get; set; } = true;
			}

			public AccountSelectionData SelectedAccount { get; set; } = new AccountSelectionData();

			public AppLoginType UiFlowInitiator { get; set; }

			[JsonIgnore] public string DefaultAccountNumber { get; set; }

			[JsonIgnore] public bool HasOpenAccounts { get; set; }

			[JsonIgnore] public bool HasClosedAccounts { get; set; }


			public bool IsAgentUser { get; set; }

			public ScreenEvent ToSmartActivationEvent { get; set; }

			public ScreenEvent ToCompetitionEvent { get; set; }

			public ScreenEvent ToPromotionEvent { get; set; }

			public ScreenEvent DismissSmartActivationBannerEvent { get; set; }

			public ScreenEvent DismissPromotionBannerEvent { get; set; }

			public ScreenEvent DismissCompetitionBannerEvent { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == CustomerAccountsStep.AccountSelection;
			}
		}
	}
}