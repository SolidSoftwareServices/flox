using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;
using Newtonsoft.Json;
using NLog;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps
{
	public class Step3PaymentDetails : SmartActivationScreen
	{
		protected string StepTitle => string.Join(" | ", "3. Payment Details", Title);

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDomainQueryResolver _domainQueryResolver;

		public Step3PaymentDetails(IDomainQueryResolver domainQueryResolver)
		{
			_domainQueryResolver = domainQueryResolver;
		}

		public override ScreenName ScreenStep => SmartActivationStep.Step3PaymentDetails;

		public static class StepEvent
		{
			public static readonly ScreenEvent AccountsPaymentConfigurationCompleted = new ScreenEvent(nameof(Step3PaymentDetails), nameof(AccountsPaymentConfigurationCompleted));
		}

		protected override Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var screenModel = new ScreenModel(ResidentialPortalFlowType.AccountsPaymentConfiguration,
				new AccountsPaymentConfigurationFlowInitializer.RootScreenModel
				{
					CallbackFlowHandler = contextData.FlowHandler,
					CallbackFlowEvent = StepEvent.AccountsPaymentConfigurationCompleted.ToString(),
				});

			SetTitle(StepTitle, screenModel);

			return BuildStepData(contextData, screenModel);
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, SmartActivationStep.ShowFlowGenericError)
				.OnEventNavigatesTo(StepEvent.AccountsPaymentConfigurationCompleted, SmartActivationStep.Step4BillingFrequency);
		}
		
		protected override Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData, UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var screenModel = (ScreenModel)originalScreenModel;

			SetTitle(StepTitle, screenModel);

			return BuildStepData(contextData, screenModel);
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.AccountsPaymentConfigurationCompleted)
			{
				AccountsPaymentConfigurationResult configurationResult = null;

				try
				{
					configurationResult = contextData.GetCurrentStepData<ScreenModel>().CalledFlowResult;
					if (configurationResult.Exit == AccountsPaymentConfigurationResult.ExitType.ErrorOcurred)
					{
						throw new Exception(" AccountsPaymentFlow failed");
					}
				}
				catch (Exception ex)
				{
					Logger.Warn(() => ex.ToString());
					contextData.GetCurrentStepData<ScreenModel>().CalledFlowResult.Exit =
						AccountsPaymentConfigurationResult.ExitType.ErrorOcurred;
					throw;
				}
			}
		}

		async Task<UiFlowScreenModel> BuildStepData(IUiFlowContextData contextData, ScreenModel model)
		{
			var rootData = contextData.GetStepData<SmartActivationFlowInitializer.StepsSharedData>(ScreenName.PreStart);
			var getAccountsInfoTask = _domainQueryResolver.GetDuelFuelAccountsByAccountNumber(rootData.AccountNumber);

			model.SourceFlowType = rootData.SourceFlowType;

			var startData = model.StartData;
			startData.AccountNumber = rootData.AccountNumber;
			startData.StartType = AccountsPaymentConfigurationFlowStartType.SmartActivation;

			var accounts = (await getAccountsInfoTask).ToArray();
			model.PaymentMethod  = accounts.Single(x => x.AccountNumber == rootData.AccountNumber).PaymentMethod;
			model.StartData.IsDualFuelAccount = accounts.Any(x => x.IsGasAccount());

			if (startData.IsDualFuelAccount)
			{
				startData.SecondaryAccountNumber = accounts.First(x => x.IsGasAccount()).AccountNumber;
			}

			return model;
		}

		public class ScreenModel : ConnectToFlow<AccountsPaymentConfigurationFlowInitializer.RootScreenModel, AccountsPaymentConfigurationResult>
		{
			public ScreenModel(ResidentialPortalFlowType startFlowType, AccountsPaymentConfigurationFlowInitializer.RootScreenModel startData = null)
				: base(startFlowType.ToString(), startData, true, true)
			{
			}

			[JsonConstructor]
			private ScreenModel() : base(true)
			{
			}

			public PaymentMethodType PaymentMethod { get; set; }

			public ResidentialPortalFlowType SourceFlowType { get; set; }
		}
	}
}