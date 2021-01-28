using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.Mvc.Core.ViewModels.Validations;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.ErrorHandling;
using EI.RP.UiFlows.Core.Flows.Screens.Metadata;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.StringResources;
using EI.RP.WebApp.Infrastructure.Validation.CustomValidators;
using Newtonsoft.Json;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps
{
    public class ChoosePaymentOption : AccountsPaymentConfigurationScreen
    {
	    private readonly IConfigurableTestingItems _configurableTestingItems;

	    public ChoosePaymentOption(IConfigurableTestingItems configurableTestingItems)
	    {
		    _configurableTestingItems = configurableTestingItems;
	    }

	    public override ScreenName ScreenStep => AccountsPaymentConfigurationStep.ChoosePaymentOption;
        public static class StepEvent
        {
	        public static readonly ScreenEvent ManualPaymentSelected =
		        new ScreenEvent(nameof(ChoosePaymentOption),nameof(ManualPaymentSelected));

	        public static readonly ScreenEvent UseExistingDirectDebitSelected =
		        new ScreenEvent(nameof(ChoosePaymentOption), nameof(UseExistingDirectDebitSelected));

	        public static readonly ScreenEvent SetUpNewDirectDebit =
		        new ScreenEvent(nameof(ChoosePaymentOption), nameof(SetUpNewDirectDebit));

	        public static readonly ScreenEvent AlternativePayerSelected =
		        new ScreenEvent(nameof(ChoosePaymentOption), nameof(AlternativePayerSelected));

	        public static readonly ScreenEvent GoBackSelected = new ScreenEvent(nameof(ChoosePaymentOption), nameof(GoBackSelected));
	        public static readonly ScreenEvent CancelFlow = new ScreenEvent(nameof(ChoosePaymentOption), nameof(CancelFlow));
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred, () => contextData.LastError.LifecycleStage == ScreenLifecycleStage.ValidateTransitionCompletedWithErrors, "Validation error")
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, AccountsPaymentConfigurationStep.CompleteFlowAndReturnToCaller, () => contextData.LastError.LifecycleStage != ScreenLifecycleStage.ValidateTransitionCompletedWithErrors,"Lifecycle error")

				.OnEventsNavigatesTo(new[]
                {
                    StepEvent.ManualPaymentSelected,
                    StepEvent.AlternativePayerSelected,
                    StepEvent.GoBackSelected,
                    StepEvent.CancelFlow
                }, AccountsPaymentConfigurationStep.CompleteFlowAndReturnToCaller)

                .OnEventNavigatesTo(StepEvent.SetUpNewDirectDebit,
                    AccountsPaymentConfigurationStep.InputDirectDebitDetailsStep, 
                    () => contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart).StartType != AccountsPaymentConfigurationFlowStartType.SmartActivation,
					"SetUpNewDirectDebit")
                .OnEventNavigatesTo(StepEvent.SetUpNewDirectDebit,
                    AccountsPaymentConfigurationStep.CompleteFlowAndReturnToCaller, 
                    () => contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart).StartType == AccountsPaymentConfigurationFlowStartType.SmartActivation,
					"SetUpNewDirectDebit for smart activation")
                .OnEventNavigatesTo(StepEvent.UseExistingDirectDebitSelected,
                    AccountsPaymentConfigurationStep.CompleteFlowAndReturnToCaller
                    , () => !UseExistingWhenOneAccountMustBeSetAndIndividuallySetUp(), "When use existing for both duel fuel accounts")
                .OnEventNavigatesTo(StepEvent.UseExistingDirectDebitSelected,
                    AccountsPaymentConfigurationStep.InputDirectDebitDetailsStep,
                    UseExistingWhenOneAccountMustBeSetAndIndividuallySetUp, 
                    "When duel fuel one account must be set and selected use existing");

            bool UseExistingWhenOneAccountMustBeSetAndIndividuallySetUp()
            {
                var currentStepData = contextData.GetCurrentStepData<ScreenModel>();
                var rootStepData = contextData
                    .GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);
                var result = currentStepData.ConfigureAccountsIndividually
                       && (rootStepData
                           .FlowResult.ConfigurationSelectionResults
                           .Count(x => x.Account?.PaymentMethod == PaymentMethodType.Manual) == 1 || rootStepData.CurrentAccount() != null);
                return result;
            }
        }

        protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
            IUiFlowContextData contextData)
        {
            var rootData =
                contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);

            var stepData = contextData.GetStepData<ScreenModel>();
            if (triggeredEvent == StepEvent.ManualPaymentSelected)
            {
                rootData.FlowResult.Exit = AccountsPaymentConfigurationResult.ExitType.CompletedWithResult;

                var accountConfigurationResult = rootData.CurrentAccount();
                accountConfigurationResult.SelectedPaymentSetUpType = PaymentSetUpType.Manual;
                accountConfigurationResult.ConfigurationCompleted = true;

            }

            else if (triggeredEvent == StepEvent.UseExistingDirectDebitSelected)
            {
                stepData.ConfigureAccountsIndividually = !stepData.UseExistingSingleSetupForAllAccounts;

                OnUseExistingDirectDebit();
            }
            else if (triggeredEvent == StepEvent.SetUpNewDirectDebit)
            {
	            SetNewDirectDebitResult(stepData, rootData.FlowResult);
				stepData.ConfigureAccountsIndividually = !stepData.UseNewSingleSetupForAllAccounts || !stepData.UseNewSingleSetupForAllAccountsFromManualConfirmDialog;
            }
			else if (triggeredEvent == StepEvent.AlternativePayerSelected)
            {
	            rootData.FlowResult.Exit = AccountsPaymentConfigurationResult.ExitType.CompletedWithResult;

	            var accountConfigurationResult = rootData.CurrentAccount();
	            accountConfigurationResult.SelectedPaymentSetUpType = PaymentSetUpType.AlternativePayer;
	            accountConfigurationResult.ConfigurationCompleted = true;
			}
            else if (new[] { StepEvent.GoBackSelected, StepEvent.CancelFlow }.Contains(triggeredEvent))
            {
                rootData.FlowResult.Exit = AccountsPaymentConfigurationResult.ExitType.UserCancelled;
            }
            else if (triggeredEvent == ScreenEvent.ErrorOccurred)
            {
                rootData.FlowResult.Exit = AccountsPaymentConfigurationResult.ExitType.ErrorOcurred;
            }
            SetSelectedOption();
            return;

            void SetSelectedOption()
            {
                stepData.SelectedOption = null;
                if (triggeredEvent == StepEvent.ManualPaymentSelected)
                {
                    stepData.SelectedOption = OptionSelected.ManualPaymentSelected;
                }
                else if (triggeredEvent == StepEvent.UseExistingDirectDebitSelected)
                {
                    stepData.SelectedOption = OptionSelected.UseExistingDirectDebitSelected;
                }
                else if (triggeredEvent == StepEvent.SetUpNewDirectDebit)
                {
                    stepData.SelectedOption = OptionSelected.SetUpNewDirectDebitSelected;
                }
                else if (triggeredEvent == StepEvent.AlternativePayerSelected)
                {
                    stepData.SelectedOption = OptionSelected.AlternativePayer;
                }
            }

            void OnUseExistingDirectDebit()
            {
                if (rootData.StartType == AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas)
                {
                    var directDebitAccounts =
                        rootData.FlowResult.ConfigurationSelectionResults.Where(x =>
                            x.Account.PaymentMethod == PaymentMethodType.DirectDebit).ToArray();

                    foreach (var directDebitAccount in directDebitAccounts)
                    {
                        directDebitAccount.ConfigurationCompleted = true;

                        directDebitAccount.SelectedPaymentSetUpType = PaymentSetUpType.UseExistingDirectDebit;
                        directDebitAccount.CommandToExecute =
                            new AccountsPaymentConfigurationResult.AccountConfigurationInfo.SetUpDirectDebitInfo(
                                directDebitAccount,
                                directDebitAccount.TargetAccountType);
                        if (directDebitAccounts.Length == 1)
                        {
                            var manualAccount =
                                rootData.FlowResult.ConfigurationSelectionResults.Single(x => x != directDebitAccount);
                            manualAccount.SelectedPaymentSetUpType = PaymentSetUpType.UseExistingDirectDebit;
                            manualAccount.CommandToExecute =
                                new AccountsPaymentConfigurationResult.AccountConfigurationInfo.SetUpDirectDebitInfo(
                                    directDebitAccount,
                                    manualAccount.TargetAccountType);
                        }
                    }

                    if (contextData.GetCurrentStepData<ScreenModel>().UseExistingSingleSetupForAllAccounts)
                    {
                        rootData.FlowResult.Exit = AccountsPaymentConfigurationResult.ExitType.CompletedWithResult;
                    }

                }
                else
                {
                    rootData.FlowResult.Exit = AccountsPaymentConfigurationResult.ExitType.CompletedWithResult;

                    SetExistingDirectDebitResult();
                    if (rootData.StartType == AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother)
                    {
                        if (stepData.UseExistingSingleSetupForAllAccounts)
                        {
                            stepData.ConfigureAccountsIndividually = false;
                            SetExistingDirectDebitResult();
                        }
                        else
                        {
                            stepData.ConfigureAccountsIndividually = true;

						}
					}
				}
			}

            void SetExistingDirectDebitResult()
            {
                var currentAccount = rootData.CurrentAccount();

                currentAccount.SelectedPaymentSetUpType = PaymentSetUpType.UseExistingDirectDebit;
                currentAccount.CommandToExecute =
                    new AccountsPaymentConfigurationResult.AccountConfigurationInfo.SetUpDirectDebitInfo(
                        currentAccount.IsNewAccount ? rootData.PrimaryAccount() : currentAccount,
                        rootData.CurrentAccount().TargetAccountType);

                currentAccount.ConfigurationCompleted = true;
            }

			void SetNewDirectDebitResult(ScreenModel input, AccountsPaymentConfigurationResult flowResult)
			{
				if (input.StartType == AccountsPaymentConfigurationFlowStartType.SmartActivation)
				{
					SetupNewDirectDebitForCurrentAccount();
					if (input.IsDualFuelAccount)
					{
						SetupNewDirectDebitForCurrentAccount();
					}
					flowResult.Exit = AccountsPaymentConfigurationResult.ExitType.CompletedWithResult;
				}

				void SetupNewDirectDebitForCurrentAccount()
				{
					var currentAccount = rootData.CurrentAccount();
					currentAccount.SelectedPaymentSetUpType = PaymentSetUpType.SetUpNewDirectDebit;
					currentAccount.CommandToExecute =
						new AccountsPaymentConfigurationResult.AccountConfigurationInfo.SetUpDirectDebitInfo(
							currentAccount.IsNewAccount ? rootData.PrimaryAccount() : currentAccount,
							rootData.CurrentAccount().TargetAccountType)
						{
							IBAN = input.UserInputIBAN,
							NameOnBankAccount = input.NameOnBankAccount
						};
					currentAccount.ConfigurationCompleted = true;
				}
			}
		}
        
        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
	        return ConfigureStepData(contextData, new ScreenModel());
        }

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(
            IUiFlowContextData contextData,
            UiFlowScreenModel originalScreenModel,
            IDictionary<string, object> stepViewCustomizations = null)
        {

            return ConfigureStepData(contextData, (ScreenModel)originalScreenModel);
        }

        private static ScreenModel ConfigureStepData(IUiFlowContextData contextData, ScreenModel screenModel)
        {
            var rootData = contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);
            screenModel.StartType = rootData.StartType;
            var configuringAccount = rootData.CurrentAccount().Account;
            switch (rootData.StartType)
            {
                case AccountsPaymentConfigurationFlowStartType.AddGasAccount:
                case AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother:
				case AccountsPaymentConfigurationFlowStartType.MoveElectricity:
				case AccountsPaymentConfigurationFlowStartType.MoveGas:
				case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndCloseGas:
				case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas:
				case AccountsPaymentConfigurationFlowStartType.SmartActivation:
	                screenModel.ExistingPaymentMethod = configuringAccount != null ?
						configuringAccount.PaymentMethod :
						rootData.FlowResult.ConfigurationSelectionResults.First(x => x.Account != null).Account.PaymentMethod;
					screenModel.TargetAccount = rootData.CurrentAccount();
					screenModel.HasAnyExistingPaymentMethodOfDirectDebit = rootData.FlowResult.ConfigurationSelectionResults
						.Any(x => x.Account != null && x.Account.PaymentMethod == PaymentMethodType.DirectDebit);

                    screenModel.HasBothAccountsExistingPaymentMethodIsDirectDebit = rootData.FlowResult.ConfigurationSelectionResults
                        .Count(x => x.Account != null && x.Account.PaymentMethod == PaymentMethodType.DirectDebit) == 2;
	                screenModel.IsDualFuelAccount = rootData.IsDualFuelAccount;

                    break;

                case AccountsPaymentConfigurationFlowStartType.ShowHistory:
                case AccountsPaymentConfigurationFlowStartType.ConfigureDirectDebit:
                case AccountsPaymentConfigurationFlowStartType.EditDirectDebit:
                case AccountsPaymentConfigurationFlowStartType.EstimateYourCost:
                case AccountsPaymentConfigurationFlowStartType.UseExistingAccountDirectDebit:
                    throw new InvalidOperationException();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return screenModel;
        }



        protected override bool OnValidateTransitionAttempt(
	        ScreenEvent triggeredEvent,
	        IUiFlowContextData contextData,
	        out string errorMessage)
        {
	        errorMessage = null;
	        var input = contextData.GetCurrentStepData<ScreenModel>();
			if (input.StartType == AccountsPaymentConfigurationFlowStartType.SmartActivation &&
			    triggeredEvent == StepEvent.SetUpNewDirectDebit )
	        {
		        if (string.IsNullOrWhiteSpace(input.NameOnBankAccount))
		        {
			        errorMessage += "Please enter a Bank Account name";
			        input.IsNameOnBankAccountInvalid = true;
		        }

				if (string.IsNullOrEmpty(input.UserInputIBAN))
		        {
			        errorMessage = "Please enter a valid IBAN";
			        input.IsIbanInvalid = true;

		        }
				
		        if (input.IsIbanInvalid || input.IsNameOnBankAccountInvalid)
		        {
			        return false;
				}
	        }

			if (contextData.LastError?.LifecycleStage == ScreenLifecycleStage.ValidateTransitionCompletedWithErrors && 
			         contextData.LastError?.OccurredOnStep == nameof(ChoosePaymentOption))
			{
				contextData.LastError = null;
			}

	        return base.OnValidateTransitionAttempt(triggeredEvent, contextData, out errorMessage);
        }

		public class ScreenModel : UiFlowScreenModel
        {
            public PaymentMethodType ExistingPaymentMethod { get; set; }

            [RequiredIf(RequiredIfAttribute.SourceType.Property, nameof(HasAnyExistingPaymentMethodOfDirectDebit), IfValue = true,
                ErrorMessage = "Please confirm that you are authorised to provide Electric Ireland with this information")]
            public bool HasConfirmedDetailsAreCorrect { get; set; }

            public AccountsPaymentConfigurationResult.AccountConfigurationInfo TargetAccount { get; set; }
            public AccountsPaymentConfigurationFlowStartType StartType { get; set; }

            public bool HasAnyExistingPaymentMethodOfDirectDebit { get; set; }

            public bool HasBothAccountsExistingPaymentMethodIsDirectDebit { get; set; }
            public bool UseNewSingleSetupForAllAccounts { get; set; } = true;
            public bool UseExistingSingleSetupForAllAccounts { get; set; } = true;

            public bool UseNewSingleSetupForAllAccountsFromManualConfirmDialog { get; set; } = true;
            public bool ConfigureAccountsIndividually { get; set; }
            public OptionSelected? SelectedOption { get; set; }

            [DisplayFormat(ConvertEmptyStringToNull = false)]
            [ValidIBAN("Please enter a valid IBAN")]
            [DenyReservedIban("Please enter a valid IBAN")]
            public string UserInputIBAN { get; set; }

            public bool IsIbanInvalid { get; set; }

			[DisplayFormat(ConvertEmptyStringToNull = false)]
			[RegularExpression(ReusableRegexPattern.ValidName, ErrorMessage = "Please enter a Bank Account name")]
			public string NameOnBankAccount { get; set; }

			public bool IsNameOnBankAccountInvalid { get; set; }

			public bool IsDualFuelAccount { get; set; }
        }

        public enum OptionSelected
        {
            ManualPaymentSelected = 1,
            UseExistingDirectDebitSelected,
            SetUpNewDirectDebitSelected,
			AlternativePayer
        }
    }
}