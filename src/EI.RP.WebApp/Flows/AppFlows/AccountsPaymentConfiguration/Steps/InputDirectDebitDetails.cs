using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.StringResources;
using EI.RP.WebApp.Infrastructure.Validation.CustomValidators;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps
{
    public class InputDirectDebitDetails : AccountsPaymentConfigurationScreen
    {
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;
        private readonly IDomainQueryResolver _queryResolver;

        public static class StepEvent
        {
            public static readonly ScreenEvent DirectDebitSetupCompleted = new ScreenEvent(nameof(InputDirectDebitDetails),nameof(DirectDebitSetupCompleted));
            public static readonly ScreenEvent ManualPaymentSetupCompleted = new ScreenEvent(nameof(InputDirectDebitDetails), nameof(ManualPaymentSetupCompleted));
            public static readonly ScreenEvent CancelDirectDebitSetup = new ScreenEvent(nameof(InputDirectDebitDetails), nameof(CancelDirectDebitSetup));
        }

        public InputDirectDebitDetails(IDomainCommandDispatcher domainCommandDispatcher,
	        IDomainQueryResolver queryResolver)
        {
	        _domainCommandDispatcher = domainCommandDispatcher;
	        _queryResolver = queryResolver;
        }

        public override ScreenName ScreenStep => AccountsPaymentConfigurationStep.InputDirectDebitDetailsStep;

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
	        return screenConfiguration
		        .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
		        .OnEventsReentriesCurrent(
			        new[] {
				        StepEvent.DirectDebitSetupCompleted, 
				        StepEvent.ManualPaymentSetupCompleted
			        },
			        HasPendingConfigurations,
			        "Account completed(1 of 2)")
		        .OnEventNavigatesTo(
			        StepEvent.DirectDebitSetupCompleted,
			        AccountsPaymentConfigurationStep.ConfirmationOfChangesStep,
			        () => !IncludedInAnotherFlow() && !HasPendingConfigurations(),
			        "Not included in another flow and configuration completed")
		        .OnEventNavigatesTo(
			        StepEvent.DirectDebitSetupCompleted,
			        AccountsPaymentConfigurationStep.CompleteFlowAndReturnToCaller,
			        () => IncludedInAnotherFlow() && !HasPendingConfigurations(),
			        "Included in another flow and configuration completed")
		        .OnEventNavigatesTo(
			        StepEvent.ManualPaymentSetupCompleted,
			        AccountsPaymentConfigurationStep.CompleteFlowAndReturnToCaller,
			        () => !HasPendingConfigurations(),
			        "Configuration completed")
		        .OnEventNavigatesTo(
			        StepEvent.CancelDirectDebitSetup,
			        AccountsPaymentConfigurationStep.ChoosePaymentOption
		        );

            bool HasPendingConfigurations()
            {
                var stepData = contextData.GetStepData<ChoosePaymentOption.ScreenModel>();
                if (stepData == null)
                {
                    return false;
                }

                return contextData
                           .GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart)
                           .CurrentAccount() != null;
            }

            bool IncludedInAnotherFlow()
            {
                var root = contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);
                return root.MustReturnToCaller;
            }

        }

        protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
            IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var equalizerMonthlyPaymentsStepData = contextData.GetStepData<EqualizerMonthlyPayments.ScreenModel>();
            var currentStepData = contextData.GetCurrentStepData<ScreenModel>();
            var setupEqualizerMonthlyPaymentsStepData = contextData.GetStepData<SetupEqualizerMonthlyPayments.ScreenModel>();
            var current = rootData.CurrentAccount();
            try
            {
                if (triggeredEvent == StepEvent.DirectDebitSetupCompleted)
                {
                    await HandleDirectDebitSetupCompleted();
                }
                else if (triggeredEvent == StepEvent.ManualPaymentSetupCompleted)
                {
                    HandleManualSetupCompleted();
                }
                else if (triggeredEvent == StepEvent.CancelDirectDebitSetup)
                {
                    HandleCancelDirectDebitSetup();
                }
            }
            catch 
            {
                current.ConfigurationCompleted = false;
                throw;
            }

            //ready to exit
            if (rootData.CurrentAccount() == null)
            {
                rootData.FlowResult.Exit = AccountsPaymentConfigurationResult.ExitType.CompletedWithResult;
            }
            return;

			void HandleCancelDirectDebitSetup()
			{
				currentStepData.SecondaryClientAccountType = null;
				foreach (var account in rootData.FlowResult.ConfigurationSelectionResults)
				{
					account.ConfigurationCompleted = false;
				}

				var choosePaymentOptionStepData = contextData.GetStepData<ChoosePaymentOption.ScreenModel>();

				if (choosePaymentOptionStepData != null)
				{
					choosePaymentOptionStepData.UseNewSingleSetupForAllAccounts = true;
				}
			}

            void HandleManualSetupCompleted()
            {

                switch (rootData.StartType)
                {
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricity:
                    case AccountsPaymentConfigurationFlowStartType.MoveGas:
                    case AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother:
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndCloseGas:
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas:
                        {
							//set up primary account
							SetManualPaymentsResult();
							var optionSelected = contextData.GetStepData<ChoosePaymentOption.ScreenModel>();
							if (!optionSelected.ConfigureAccountsIndividually &&
								rootData.StartType.IsOneOf(AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas,
														   AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother))
							{
								//setup secondary account
								SetManualPaymentsResult();
							}
						}
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(rootData.StartType.ToString());
                }
            }

            async Task HandleDirectDebitSetupCompleted()
            {
                var isEqualizer = currentStepData.PaymentMethod == PaymentMethodType.Equalizer;

                if (isEqualizer &&
                    contextData.EventsLog.Any(x => x.Event == EqualizerMonthlyPayments.StepEvent.SetupEqualizerMonthlyPayments))
                {
                    currentStepData.ViewMode = ScreenModel.StepMode.EqualizerSetup;
                }

				var input = contextData.GetCurrentStepData<ScreenModel>();
				
				switch (rootData.StartType)
                {
                    case AccountsPaymentConfigurationFlowStartType.AddGasAccount:
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricity:
                    case AccountsPaymentConfigurationFlowStartType.MoveGas:
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndCloseGas:
                        SetNewDirectDebitResult(input);
                        break;
                    case AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother:
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas:
						//set up primary account
						SetNewDirectDebitResult(input);
                        var optionSelected = contextData.GetStepData<ChoosePaymentOption.ScreenModel>();
                        if (!optionSelected.ConfigureAccountsIndividually)
                        {
							//setup secondary account
							SetNewDirectDebitResult(input);
                        }

                        break;

                    default:
                        var inputAccount = rootData.PrimaryAccount();
                        string contractId = null;
                        DateTime? equalizerStartDate = null;
                        DateTime? equalizerFirstDueDate = null;
						if (isEqualizer)
						{
							if(equalizerMonthlyPaymentsStepData == null)
							{
								var billingInfo =
									await _queryResolver.GetAccountBillingInfoByAccountNumber(input.AccountNumber);
								contractId = inputAccount.Account.ContractId;
								equalizerStartDate = billingInfo.EqualizerStartDate;
								equalizerFirstDueDate = billingInfo.EqualizerNextBillDate;
							}
							else
							{
								contractId = equalizerMonthlyPaymentsStepData.EqualizerMonthlyPaymentDetails.ContractId;
								equalizerStartDate = equalizerMonthlyPaymentsStepData.EqualizerMonthlyPaymentDetails.StartDate;
								equalizerFirstDueDate = setupEqualizerMonthlyPaymentsStepData.FirstPaymentDate;
							}
						}
						
                        var setUpDirectDebitDomainCommand = new SetUpDirectDebitDomainCommand(
	                        input.AccountNumber,
	                        input.NameOnBankAccount,
	                        input.IBAN,
	                        input.UserInputIBAN,
	                        inputAccount.Account.Partner,
	                        inputAccount.Account.ClientAccountType,
	                        currentStepData.PaymentMethod,
	                        contractId,
	                        equalizerStartDate,
	                        equalizerFirstDueDate);

						await _domainCommandDispatcher.ExecuteAsync(setUpDirectDebitDomainCommand);
                        break;
                }

                input.ClearInputFields();
            }

            void SetManualPaymentsResult()
            {
                var accountConfigurationResult = rootData.CurrentAccount();
                accountConfigurationResult.SelectedPaymentSetUpType = PaymentSetUpType.Manual;
                accountConfigurationResult.CommandToExecute = null;
                accountConfigurationResult.ConfigurationCompleted = true;
            }

            void SetNewDirectDebitResult(ScreenModel input)
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

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return ConfigureStepData(contextData, new ScreenModel());
		}

		private UiFlowScreenModel ConfigureStepData(IUiFlowContextData contextData, ScreenModel data)
		{
			var rootData = contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);
			SetViewModeAndTargetAccount();
			SetInputData();
			SetEqualizerIfNeeded();
			SetShowOrCancelDirectDebitSetupFlags();
			SetSubmitButtonText();
			SetAccountHeaderText();
			SetTitle(ResolveTitle(), data);

			return data;

			void SetAccountHeaderText()
			{
				var startTypesToSetHeaderFor = new List<AccountsPaymentConfigurationFlowStartType>()
				{
					AccountsPaymentConfigurationFlowStartType.AddGasAccount,
					AccountsPaymentConfigurationFlowStartType.MoveElectricity,
					AccountsPaymentConfigurationFlowStartType.MoveGas,
					AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother,
					AccountsPaymentConfigurationFlowStartType.MoveElectricityAndCloseGas,
					AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas,
				};
				
				if(startTypesToSetHeaderFor.Any(x => x == rootData.StartType))
				{
					var chooseData = contextData.GetStepData<ChoosePaymentOption.ScreenModel>();

					if (chooseData.ConfigureAccountsIndividually)
					{
						var currentAccount = rootData.CurrentAccount();
						data.AccountHeaderText = $"{currentAccount.TargetAccountType}";
						if (!currentAccount.IsNewAccount)
						{
							data.AccountHeaderText += $" ({currentAccount.Account.AccountNumber})";
						}
					}
					else
					{
						var firstAccount = rootData.FlowResult.ConfigurationSelectionResults.First();

						data.AccountHeaderText = $"{firstAccount.TargetAccountType}";

						if (!firstAccount.IsNewAccount)
						{
							data.AccountHeaderText += $" ({firstAccount.Account.AccountNumber})";
						}

						var secondAccount = rootData.FlowResult.ConfigurationSelectionResults.LastOrDefault(x => x != firstAccount);
						if (secondAccount != null)
						{
							data.AccountHeaderText += $" & {secondAccount.TargetAccountType}";

							if (!secondAccount.IsNewAccount)
							{
								data.AccountHeaderText += $" ({secondAccount.Account.AccountNumber})";
							}
						}
					}
				}
			}

			void SetSubmitButtonText()
			{
				data.SubmitButtonText = "Complete Direct Debit Setup";
				if (rootData.StartType == AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas ||
					rootData.StartType == AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother)
				{
					var chooseData = contextData.GetStepData<ChoosePaymentOption.ScreenModel>();

                    if (chooseData.ConfigureAccountsIndividually
                        && chooseData.SelectedOption.Value ==
                        ChoosePaymentOption.OptionSelected.SetUpNewDirectDebitSelected
                        && rootData.FlowResult.ConfigurationSelectionResults.Count(x => !x.ConfigurationCompleted) == 2)
                    {


                        data.SubmitButtonText =
                            $"Continue to {rootData.FlowResult.ConfigurationSelectionResults.Last().TargetAccountType.ToString().ToLower()} direct debit";
                    }
                }
            }

			void SetShowOrCancelDirectDebitSetupFlags()
            {
                var chooseData = contextData.GetStepData<ChoosePaymentOption.ScreenModel>();
                ChoosePaymentOption.OptionSelected currentOption;
                var account = rootData.CurrentAccount();
                switch (rootData.StartType)
                {
                    case AccountsPaymentConfigurationFlowStartType.ShowHistory:
                    case AccountsPaymentConfigurationFlowStartType.ConfigureDirectDebit:
                    case AccountsPaymentConfigurationFlowStartType.EditDirectDebit:
                    case AccountsPaymentConfigurationFlowStartType.EstimateYourCost:
                    case AccountsPaymentConfigurationFlowStartType.UseExistingAccountDirectDebit:
                    case AccountsPaymentConfigurationFlowStartType.AddGasAccount:
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricity:
                    case AccountsPaymentConfigurationFlowStartType.MoveGas:
                    case AccountsPaymentConfigurationFlowStartType.EqualizerMonthlySetup:
                        if (rootData.FlowResult.ConfigurationSelectionResults.Any(x => x.Account?.PaymentMethod == PaymentMethodType.DirectDebit))
                        {
                            data.ShowCancelDirectDebitSetup = true;
                        }
                        if (rootData.FlowResult.ConfigurationSelectionResults.Any(x => x.Account?.PaymentMethod == PaymentMethodType.Manual) && !data.ShowCancelDirectDebitSetup)
                        {
                            data.ShowSkipDirectDebitSetup = true;
                        }
                        break;
                    case AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother:
                        currentOption = ResolveSelectedOption();

                        bool isSetUpNewDirectDebitSelected = currentOption == ChoosePaymentOption.OptionSelected.SetUpNewDirectDebitSelected;
                        if (rootData.FlowResult.ConfigurationSelectionResults.Any(x => x.Account?.PaymentMethod == PaymentMethodType.DirectDebit && isSetUpNewDirectDebitSelected))
                        {
                            if (data.AccountNumber == null && data.ClientAccountType == ClientAccountType.Electricity && chooseData.ConfigureAccountsIndividually)
                            {
                                data.ShowSkipDirectDebitSetup = true;
                                data.ShowCancelDirectDebitSetup = false;
                            }
                            else
                            {
                                data.ShowCancelDirectDebitSetup = true;
                                data.ShowSkipDirectDebitSetup = false;
                            }
                        }
                        else
                        {
                            data.ShowSkipDirectDebitSetup = true;
                            data.ShowCancelDirectDebitSetup = false;
                        }
                        break;
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndCloseGas:
                        if (account.Account != null && account.Account.PaymentMethod == PaymentMethodType.DirectDebit)
                        {
                            data.ShowCancelDirectDebitSetup = true;
                            data.ShowSkipDirectDebitSetup = false;
                        }
                        else if (account.Account?.AccountNumber == data.AccountNumber)
                        {
                            data.ShowCancelDirectDebitSetup = false;
                            data.ShowSkipDirectDebitSetup = true;
                        }

                        break;
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas:
                        currentOption = ResolveSelectedOption();
                        if (rootData.FlowResult.ConfigurationSelectionResults.Any(x =>
                            x.Account != null && x.Account.PaymentMethod == PaymentMethodType.DirectDebit))
                        {
                            if (currentOption == ChoosePaymentOption.OptionSelected.SetUpNewDirectDebitSelected &&
                                !chooseData.ConfigureAccountsIndividually)
                            {
                                data.ShowCancelDirectDebitSetup = true;
                                data.ShowSkipDirectDebitSetup = false;
                            }
                            else if (currentOption == ChoosePaymentOption.OptionSelected.SetUpNewDirectDebitSelected &&
                                     chooseData.ConfigureAccountsIndividually)
                            {
                                var isElectricityDirectDebit = rootData.FlowResult.ConfigurationSelectionResults.Any(
                                    x => x.Account != null &&
                                         x.Account.ClientAccountType == ClientAccountType.Electricity
                                         && x.Account.PaymentMethod == PaymentMethodType.DirectDebit);

                                var isGasManual = rootData.FlowResult.ConfigurationSelectionResults.Any(
                                    x => x.Account != null
                                         && x.Account.ClientAccountType == ClientAccountType.Gas
                                         && x.Account.PaymentMethod == PaymentMethodType.Manual);

                                var isConfiguringElectricity =
                                    account.Account.ClientAccountType == ClientAccountType.Electricity;
                                var isConfiguringGas = account.Account.ClientAccountType == ClientAccountType.Gas;

                                data.ShowCancelDirectDebitSetup =
                                    account.Account.PaymentMethod == PaymentMethodType.DirectDebit;
                                data.ShowSkipDirectDebitSetup = !data.ShowCancelDirectDebitSetup;

                                if (isConfiguringElectricity && isElectricityDirectDebit && isGasManual)
                                {
                                    data.ShowCancelDirectDebitSetup = true;
                                    data.ShowSkipDirectDebitSetup = false;
                                }
                                else if (isConfiguringGas && isElectricityDirectDebit && isGasManual)
                                {
                                    data.ShowCancelDirectDebitSetup = false;
                                    data.ShowSkipDirectDebitSetup = true;
                                }
                            }
                            else if (currentOption ==
                                     ChoosePaymentOption.OptionSelected.UseExistingDirectDebitSelected &&
                                     chooseData.ConfigureAccountsIndividually)
                            {
                                data.ShowCancelDirectDebitSetup =
                                    account.Account.PaymentMethod != PaymentMethodType.Manual;
                                data.ShowSkipDirectDebitSetup = !data.ShowCancelDirectDebitSetup;
                            }
                            else
                            {
                                if (account.Account.AccountNumber == data.AccountNumber)
                                {
                                    data.ShowSkipDirectDebitSetup = true;
                                    data.ShowCancelDirectDebitSetup = false;
                                }

                                if (account.Account.AccountNumber == data.SecondaryAccountNumber)
                                {
                                    data.ShowCancelDirectDebitSetup = true;
                                    data.ShowSkipDirectDebitSetup = false;
                                }
                            }
                        }
                        else
                        {
                            data.ShowSkipDirectDebitSetup = true;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            void SetViewModeAndTargetAccount()
            {
                data.StartType = rootData.StartType;
                switch (rootData.StartType)
                {
                    case AccountsPaymentConfigurationFlowStartType.AddGasAccount:
                        data.ViewMode = ScreenModel.StepMode.GasAccountSetUp;

                        break;
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricity:
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndCloseGas:
                        data.ViewMode = ScreenModel.StepMode.MoveElectricity;
                        break;
                    case AccountsPaymentConfigurationFlowStartType.MoveGas:
                        data.ViewMode = ScreenModel.StepMode.MoveGas;
                        break;
                    case AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother:
                    case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas:
                        var optionSelected = contextData.GetStepData<ChoosePaymentOption.ScreenModel>();

                        var accountResult = rootData.CurrentAccount();
                        if (!optionSelected.ConfigureAccountsIndividually)
                        {
                            data.ViewMode = ScreenModel.StepMode.MultipleAccountsSetUp;
                            data.SecondaryClientAccountType =
                                accountResult.TargetAccountType == ClientAccountType.Electricity
                                    ? ClientAccountType.Gas
                                    : ClientAccountType.Electricity;
                        }
                        else
                        {
                            if (accountResult.TargetAccountType == ClientAccountType.Electricity)
                            {
                                data.ViewMode = ScreenModel.StepMode.MoveElectricity;
                            }
                            else if (accountResult.TargetAccountType == ClientAccountType.Gas)
                            {
                                data.ViewMode = ScreenModel.StepMode.MoveGas;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException();
                            }
                        }

                        break;
                }
            }

            void SetEqualizerIfNeeded()
            {
                if (contextData.EventsLog.Count > 0)
                {
                    var isEqualizerSetup = contextData.EventsLog.Any(x =>
                        x.Event == EqualizerMonthlyPayments.StepEvent.SetupEqualizerMonthlyPayments);

                    if (isEqualizerSetup)
                        data.ViewMode = ScreenModel.StepMode.EqualizerSetup;
                }

                if (data.ViewMode == ScreenModel.StepMode.EqualizerSetup)
                {
                    var setupEqualizerMonthlyPaymentsStepData =
                        contextData.GetStepData<SetupEqualizerMonthlyPayments.ScreenModel>();
                    data.DisplayFirstPaymentDate = setupEqualizerMonthlyPaymentsStepData.FirstPaymentDate.GetValueOrDefault();
                    data.EqualizedAmount = setupEqualizerMonthlyPaymentsStepData.EqualizerMonthlyPaymentAmount;
                    data.PaymentMethod = PaymentMethodType.Equalizer;
                }
            }

            ChoosePaymentOption.OptionSelected ResolveSelectedOption()
            {
                var currentOption = contextData.GetStepData<ChoosePaymentOption.ScreenModel>().SelectedOption;
                if (!currentOption.HasValue)
                {
                    throw new ApplicationException();
                }

                return currentOption.Value;
            }

            void ResolveInputDetails(AccountsPaymentConfigurationResult.AccountConfigurationInfo currentAccount)
            {
				string nameOnBankAccount = null;
				if (currentAccount.CommandToExecute != null)
				{
					data.PaymentMethod = PaymentMethodType.DirectDebit;
					data.IBAN = currentAccount.CommandToExecute.IBAN;
					nameOnBankAccount = currentAccount.CommandToExecute.NameOnBankAccount;
				}
				else
				{
					data.PaymentMethod = currentAccount.BankAccount.PaymentMethod;
					data.IBAN = currentAccount.BankAccount.IBAN;
					nameOnBankAccount = currentAccount.BankAccount.NameInBankAccount;
				}

				if (ShouldPopulateUserInputFields(currentAccount) && !HasUserEnteredAnyValues())
				{
					data.UserInputIBAN = data.IBAN?.Mask('*', data.IBAN.Length - 4);
					data.NameOnBankAccount = nameOnBankAccount;
				}
				else if (!HasUserEnteredAnyValues())
				{
					data.UserInputIBAN = null;
					data.NameOnBankAccount = null;
				}
            }

			void SetInputData()
            {
	            var currentAccount = rootData.CurrentAccount();
                data.ClientAccountType = currentAccount.TargetAccountType;

				if (rootData.StartType == AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother)
				{
					var currentOption = ResolveSelectedOption();
					if(currentOption == ChoosePaymentOption.OptionSelected.UseExistingDirectDebitSelected)
					{
						var account = rootData.FlowResult.ConfigurationSelectionResults.FirstOrDefault(x => x.Account?.PaymentMethod == PaymentMethodType.DirectDebit);
						if (account != null)
						{
							ResolveInputDetails(account);
						}
					}
					else if(currentOption == ChoosePaymentOption.OptionSelected.SetUpNewDirectDebitSelected)
					{
						data.AccountNumber = currentAccount.Account?.AccountNumber;
						data.SecondaryAccountNumber = rootData.SecondaryAccountNumber;
					}
				}
				else
				{
					if (!currentAccount.IsNewAccount && !new[] { AccountsPaymentConfigurationFlowStartType.MoveElectricity, AccountsPaymentConfigurationFlowStartType.MoveGas }.Contains(rootData.StartType))
					{
						ResolveInputDetails(currentAccount);
					}

					data.AccountNumber = currentAccount.Account.AccountNumber;
					data.SecondaryAccountNumber = rootData.SecondaryAccountNumber;
				}
			}

			bool ShouldPopulateUserInputFields(AccountsPaymentConfigurationResult.AccountConfigurationInfo currentAccount)
			{
				switch (rootData.StartType)
				{
					case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndCloseGas:
					case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas:
						if (currentAccount.SelectedPaymentSetUpType != PaymentSetUpType.UseExistingDirectDebit)
						{
							return false;
						}
						break;
				}

				return true;
			}

			bool HasUserEnteredAnyValues()
			{
				return !string.IsNullOrEmpty(data.UserInputIBAN) ||
						!string.IsNullOrEmpty(data.NameOnBankAccount) ||
						data.TermsAndConditionsAccepted;
			}

			string ResolveTitle()
			{
				switch (data.ViewMode)
				{
					case ScreenModel.StepMode.DefaultSetUp:
					case ScreenModel.StepMode.DefaultEdit:
						return "Direct Debit Settings";
					case ScreenModel.StepMode.EqualizerSetup:
						return "Equal Monthly Payments";
					case ScreenModel.StepMode.GasAccountSetUp:
						return "Add Gas";
                    default:
						return null;
				}
			}
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(
            IUiFlowContextData contextData,
            UiFlowScreenModel originalScreenModel,
            IDictionary<string, object> stepViewCustomizations = null)
        {
            var originalData = (ScreenModel)originalScreenModel;
            if (contextData.EventsLog.Any())
            {
                var setupEqualizerMonthlyPaymentsStepData = contextData.GetStepData<SetupEqualizerMonthlyPayments.ScreenModel>();
                if (setupEqualizerMonthlyPaymentsStepData != null)
                    originalData.DisplayFirstPaymentDate = setupEqualizerMonthlyPaymentsStepData.FirstPaymentDate.GetValueOrDefault();

                if (contextData.EventsLog.Any(x => x.Event == EqualizerMonthlyPayments.StepEvent.SetupEqualizerMonthlyPayments))
                {
                    originalData.ViewMode = ScreenModel.StepMode.EqualizerSetup;
                }
            }

            ConfigureStepData(contextData, originalData);
            return originalData;
        }

        public class ScreenModel : UiFlowScreenModel
        {
	        public bool ShowSkipDirectDebitSetup { get; set; }
            public bool ShowCancelDirectDebitSetup { get; set; }

            public override bool IsValidFor(ScreenName screenStep)
            {				
                return screenStep == AccountsPaymentConfigurationStep.InputDirectDebitDetailsStep;
            }
            public PaymentMethodType PaymentMethod { get; set; }
            public ClientAccountType ClientAccountType { get; set; }

            public ClientAccountType SecondaryClientAccountType { get; set; }

            //TODO: [MM] explore options to remove next
            [DisplayFormat(ConvertEmptyStringToNull = false)]
			[Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid IBAN")]
            [ValidIBAN("Please enter a valid IBAN", nameof(IBAN))]
			[DenyReservedIban("Please enter a valid IBAN")]
            public string UserInputIBAN { get; set; }

            public string IBAN { get; set; }

			[DisplayFormat(ConvertEmptyStringToNull = false)]
			[Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a Bank Account name")]
            [RegularExpression(ReusableRegexPattern.ValidName, ErrorMessage = "Please enter a Bank Account name")]
			public string NameOnBankAccount { get; set; }

			[Range(typeof(bool), "true", "true", ErrorMessage = "Please confirm that you have read and accept the Electric Ireland Terms and Conditions")]
            public bool TermsAndConditionsAccepted { get; set; }

            public string AccountNumber { get; set; }
            public EuroMoney EqualizedAmount { get; set; }
            public DateTime DisplayFirstPaymentDate { get; set; }

            public enum StepMode
            {
                DefaultSetUp = 0,
                DefaultEdit,
                EqualizerSetup,
                GasAccountSetUp,
                MoveElectricity,
                MoveGas,
                MultipleAccountsSetUp
            }

            public StepMode ViewMode { get; set; }

            public override IEnumerable<ScreenEvent> DontValidateEvents => base.DontValidateEvents.Union(new[] {
                    StepEvent.ManualPaymentSetupCompleted,
                    StepEvent.CancelDirectDebitSetup
                });

            public AccountsPaymentConfigurationFlowStartType StartType { get; set; }
            public string SecondaryAccountNumber { get; set; }

			public string SubmitButtonText { get; set; }
			public string AccountHeaderText { get; set; }

            public void ClearInputFields()
            {
                TermsAndConditionsAccepted = false;
                PaymentMethod = PaymentMethodType.DirectDebit;
                IBAN =
                    UserInputIBAN =
                        NameOnBankAccount =
                            AccountNumber = null;
            }
        }
	}
}