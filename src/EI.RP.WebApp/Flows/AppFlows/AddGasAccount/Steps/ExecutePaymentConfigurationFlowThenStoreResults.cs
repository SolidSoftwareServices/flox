using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.Gas;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps;
using EI.RP.WebApp.Flows.AppFlows.AddGasAccount.FlowDefinitions;
using Newtonsoft.Json;
using NLog;

namespace EI.RP.WebApp.Flows.AppFlows.AddGasAccount.Steps
{
	public class ExecutePaymentConfigurationFlowThenStoreResults : AddGasAccountScreen
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent AccountsPaymentConfigurationCompleted = new ScreenEvent(nameof(ExecutePaymentConfigurationFlowThenStoreResults),nameof(AccountsPaymentConfigurationCompleted));
		}
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private readonly IDomainCommandDispatcher _domainCommandDispatcher;
		private readonly IDomainQueryResolver _queryResolver;

		public ExecutePaymentConfigurationFlowThenStoreResults(IDomainCommandDispatcher domainCommandDispatcher, IDomainQueryResolver queryResolver)
		{
			_domainCommandDispatcher = domainCommandDispatcher;
			_queryResolver = queryResolver;
		}

		public override string ViewPath { get; } = "PaymentConfigurationFlow";
		public override ScreenName ScreenStep => AddGasAccountStep.ExecutePaymentConfigurationFlowThenStoreResults;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var stepData = new StepData(ResidentialPortalFlowType.AccountsPaymentConfiguration, new AccountsPaymentConfigurationFlowInitializer.RootScreenModel
			{
				StartType = AccountsPaymentConfigurationFlowStartType.AddGasAccount,
				CallbackFlowHandler = contextData.FlowHandler,
				CallbackFlowEvent = StepEvent.AccountsPaymentConfigurationCompleted.ToString(),
				AccountNumber=contextData.GetStepData<AddGasAccountFlowInitializer.RootScreenModel>().ElectricityAccountNumber,
			});

			SetTitle(Title, stepData);

			return stepData;
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventNavigatesTo(ScreenEvent.ErrorOccurred,AddGasAccountStep.ShowErrorMessage )
				.OnEventNavigatesTo(StepEvent.AccountsPaymentConfigurationCompleted,
					AddGasAccountStep.ShowOperationWasCompleted,
					() => contextData.GetCurrentStepData<StepData>().CalledFlowResult.Exit ==
					      AccountsPaymentConfigurationResult.ExitType.CompletedWithResult,
					"Payment method configured")
				.OnEventNavigatesTo(StepEvent.AccountsPaymentConfigurationCompleted,
					AddGasAccountStep.CollectAccountConsumptionDetails,
					() => contextData.GetCurrentStepData<StepData>().CalledFlowResult.Exit ==
					      AccountsPaymentConfigurationResult.ExitType.UserCancelled,
					"User cancelled")
				.OnEventNavigatesTo(StepEvent.AccountsPaymentConfigurationCompleted,
				AddGasAccountStep.ShowErrorMessage,
				() => contextData.GetCurrentStepData<StepData>().CalledFlowResult.Exit ==
				      AccountsPaymentConfigurationResult.ExitType.ErrorOcurred,
				"Error occurred");


		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.AccountsPaymentConfigurationCompleted)
			{
				try
				{
					var configurationResult = contextData.GetCurrentStepData<StepData>().CalledFlowResult;

					switch (configurationResult.Exit)
					{
						case AccountsPaymentConfigurationResult.ExitType.CompletedWithResult:
							await CompleteAddGas(configurationResult);
							break;

						case AccountsPaymentConfigurationResult.ExitType.UserCancelled:
							var dataToClear =
								contextData.GetStepData<CollectAccountConsumptionDetails.ScreenModel>(AddGasAccountStep
									.CollectAccountConsumptionDetails);
							dataToClear.Reset();
							break;


						case AccountsPaymentConfigurationResult.ExitType.NoExitedYet:
							throw new ApplicationException();
					}
				}
				catch (Exception ex)
				{
					Logger.Error(() => ex.ToString());
					contextData.GetCurrentStepData<StepData>().CalledFlowResult.Exit = AccountsPaymentConfigurationResult.ExitType.ErrorOcurred;
				}

			}
		

			async Task CompleteAddGas(AccountsPaymentConfigurationResult configurationResult)
			{
				var accountConfigurationResult = configurationResult.ConfigurationSelectionResults.Single();
				//use the command not from the called flow as a template

				var consumptionData =
					contextData.GetStepData<CollectAccountConsumptionDetails.ScreenModel>(AddGasAccountStep
						.CollectAccountConsumptionDetails);

				string iban = null;
				string nameOnBankAccount = null;

				if (accountConfigurationResult.SelectedPaymentSetUpType == PaymentSetUpType.SetUpNewDirectDebit)
				{
					var templateCommand = accountConfigurationResult.CommandToExecute.ToCommand();
					iban = templateCommand.NewIBAN;
					nameOnBankAccount = templateCommand.NameOnBankAccount;
				}

				// Add account with manual payment
				await _domainCommandDispatcher.ExecuteAsync(new AddGasAccountCommand(consumptionData.GPRN,
					consumptionData.ElectricityAccountNumber, Convert.ToDecimal(consumptionData.GasMeterReading),
					accountConfigurationResult.SelectedPaymentSetUpType, iban, nameOnBankAccount));
			}
		}

		public class StepData : ConnectToFlow<AccountsPaymentConfigurationFlowInitializer.RootScreenModel, AccountsPaymentConfigurationResult>
		{
			public StepData(ResidentialPortalFlowType startFlowType, AccountsPaymentConfigurationFlowInitializer.RootScreenModel startData )
				: base(startFlowType.ToString(), startData)
			{
			}

			[JsonConstructor]
			private StepData()
			{
			}

		}

	}
}