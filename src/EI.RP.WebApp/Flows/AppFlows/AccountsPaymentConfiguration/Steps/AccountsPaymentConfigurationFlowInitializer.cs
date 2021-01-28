using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using Ei.Rp.DomainModels.Banking;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps
{
	public class AccountsPaymentConfigurationFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType, AccountsPaymentConfigurationFlowInitializer.RootScreenModel>
	{
		private readonly IDomainQueryResolver _domainQueryResolver;

		private readonly IUserSessionProvider _userSessionProvider;

		public AccountsPaymentConfigurationFlowInitializer(IUserSessionProvider userSessionProvider,
			IDomainQueryResolver domainQueryResolver)

		{
			_userSessionProvider = userSessionProvider;
			_domainQueryResolver = domainQueryResolver;
		}

		public override bool Authorize()
		{
			return !_userSessionProvider.IsAnonymous();
		}

		public override ResidentialPortalFlowType InitializerOfFlowType =>
			ResidentialPortalFlowType.AccountsPaymentConfiguration;


		private static readonly AccountsPaymentConfigurationFlowStartType[] ToChoosePaymentOptionStartTypes =
		{
			AccountsPaymentConfigurationFlowStartType.AddGasAccount,
			AccountsPaymentConfigurationFlowStartType.MoveElectricity,
			AccountsPaymentConfigurationFlowStartType.MoveGas,
			AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother,
			AccountsPaymentConfigurationFlowStartType.MoveElectricityAndCloseGas,
			AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas,
		};

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
			IScreenFlowConfigurator preStartCfg, UiFlowContextData contextData)
		{

			return preStartCfg
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred,
					AccountsPaymentConfigurationStep.ShowFlowGenericError)
				.OnEventNavigatesTo(ScreenEvent.Start,
					AccountsPaymentConfigurationStep.SetupDirectDebitWithPaymentOptions,
					() => contextData.GetStepData<RootScreenModel>(ScreenName.PreStart).StartType ==
						  AccountsPaymentConfigurationFlowStartType.ConfigureDirectDebit,
					"Configure Direct Debit")
				.OnEventNavigatesTo(ScreenEvent.Start, AccountsPaymentConfigurationStep.ShowPaymentsHistory,
					() => contextData.GetStepData<RootScreenModel>(ScreenName.PreStart).StartType ==
						  AccountsPaymentConfigurationFlowStartType.ShowHistory,
					"Show Payments History")
				.OnEventNavigatesTo(ScreenEvent.Start, AccountsPaymentConfigurationStep.InputDirectDebitDetailsStep,
					() => contextData.GetStepData<RootScreenModel>(ScreenName.PreStart).StartType ==
						  AccountsPaymentConfigurationFlowStartType.EditDirectDebit,
					"Edit Direct Debit")
				.OnEventNavigatesTo(ScreenEvent.Start, AccountsPaymentConfigurationStep.ShowAccountCostEstimation,
					() => contextData.GetStepData<RootScreenModel>(ScreenName.PreStart).StartType ==
						  AccountsPaymentConfigurationFlowStartType.EstimateYourCost,
					"Estimate Your Cost")
				.OnEventNavigatesTo(ScreenEvent.Start, AccountsPaymentConfigurationStep.ChoosePaymentOption,
					() => ToChoosePaymentOptionStartTypes.Any(x => x == contextData.GetStepData<RootScreenModel>(ScreenName.PreStart).StartType),
					"When accounts are being added or moved")
				.OnEventNavigatesTo(ScreenEvent.Start, AccountsPaymentConfigurationStep.ChoosePaymentOption,
					() => contextData.GetStepData<RootScreenModel>(ScreenName.PreStart).StartType ==
						  AccountsPaymentConfigurationFlowStartType.SmartActivation,
					"Account payment for smart activation")
				.OnEventNavigatesTo(ScreenEvent.Start, AccountsPaymentConfigurationStep.ConfirmationOfChangesStep,
					() => contextData.GetStepData<RootScreenModel>(ScreenName.PreStart).StartType ==
						  AccountsPaymentConfigurationFlowStartType.UseExistingAccountDirectDebit,
					"Use Existing Account Direct Debit")
				.OnEventNavigatesTo(ScreenEvent.Start, AccountsPaymentConfigurationStep.EqualizerMonthlyPayments,
					() => contextData.GetStepData<RootScreenModel>(ScreenName.PreStart).StartType ==
						  AccountsPaymentConfigurationFlowStartType.EqualizerMonthlySetup,
					"Setup Equaliser Monthly payments");
		}

		protected override async Task<RootScreenModel> OnBuildStartData(UiFlowContextData newContext,
			RootScreenModel preloadedInputData)
		{
			preloadedInputData.FlowResult = new AccountsPaymentConfigurationResult();
			var inputAccounts = await ResolveInputAccounts();

			InitializeResult();

			return preloadedInputData;

			void InitializeResult()
			{
				var primaryAccount =
					inputAccounts.Single(x => x.UserAccount.AccountNumber == preloadedInputData.AccountNumber);
				var secondaryAccount = inputAccounts.SingleOrDefault(x =>
					x.UserAccount.AccountNumber == preloadedInputData.SecondaryAccountNumber);
				switch (preloadedInputData.StartType)
				{
					case AccountsPaymentConfigurationFlowStartType.AddGasAccount:

						preloadedInputData.FlowResult.ConfigurationSelectionResults = new[]
						{
							new AccountsPaymentConfigurationResult.AccountConfigurationInfo
							{

								IsNewAccount = true,
								TargetAccountType = ClientAccountType.Gas,
								Account = primaryAccount.UserAccount,
								BankAccount = primaryAccount.IncomingBankAccount
							}
						};
						break;
					case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndCloseGas:
					case AccountsPaymentConfigurationFlowStartType.MoveElectricity:
					case AccountsPaymentConfigurationFlowStartType.MoveGas:
						preloadedInputData.FlowResult.ConfigurationSelectionResults = new[]
						{
							new AccountsPaymentConfigurationResult.AccountConfigurationInfo
							{
								IsNewAccount = false,
								TargetAccountType = primaryAccount.UserAccount.ClientAccountType,
								Account = primaryAccount.UserAccount,
								BankAccount = primaryAccount.IncomingBankAccount
							}
						};
						break;

					case AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother:
						var secondaryClientAccountType = primaryAccount.UserAccount.ClientAccountType == ClientAccountType.Electricity
									? ClientAccountType.Gas
									: ClientAccountType.Electricity;

						var primaryAccountConfigurationInfo = new AccountsPaymentConfigurationResult.AccountConfigurationInfo
						{
							IsNewAccount = false,
							TargetAccountType = primaryAccount.UserAccount.ClientAccountType,
							Account = primaryAccount.UserAccount,
							BankAccount = primaryAccount.IncomingBankAccount
						};

						var secondaryAccountConfigurationInfo = new AccountsPaymentConfigurationResult.AccountConfigurationInfo
						{
							IsNewAccount = true,
							TargetAccountType = secondaryClientAccountType,
						};

						if (secondaryClientAccountType == ClientAccountType.Electricity)
						{
							preloadedInputData.FlowResult.ConfigurationSelectionResults = new[]
							{
								secondaryAccountConfigurationInfo,
								primaryAccountConfigurationInfo,
							};
						}
						else
						{
							preloadedInputData.FlowResult.ConfigurationSelectionResults = new[]
							{
								primaryAccountConfigurationInfo,
								secondaryAccountConfigurationInfo,
							};
						}
						break;

					case AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas:
						preloadedInputData.FlowResult.ConfigurationSelectionResults = new[]
						{
							new AccountsPaymentConfigurationResult.AccountConfigurationInfo
							{
								IsNewAccount = false,
								TargetAccountType = primaryAccount.UserAccount.ClientAccountType,
								Account = primaryAccount.UserAccount,
								BankAccount = primaryAccount.IncomingBankAccount
							},
							new AccountsPaymentConfigurationResult.AccountConfigurationInfo
							{
								IsNewAccount = false,
								TargetAccountType = secondaryAccount.UserAccount.ClientAccountType,
								Account = secondaryAccount.UserAccount,
								BankAccount = secondaryAccount.IncomingBankAccount,

								ConfigurationCompleted =
									preloadedInputData.StartType == AccountsPaymentConfigurationFlowStartType
										.MoveElectricityAndCloseGas
							},
						};
						break;
					case AccountsPaymentConfigurationFlowStartType.SmartActivation:
						var configurationSelectionResults = new List<AccountsPaymentConfigurationResult.AccountConfigurationInfo>
							{
								new AccountsPaymentConfigurationResult.AccountConfigurationInfo
								{
									IsNewAccount = false,
									TargetAccountType = primaryAccount.UserAccount.ClientAccountType,
									Account = primaryAccount.UserAccount,
									BankAccount = primaryAccount.IncomingBankAccount
								}
							};
						if (preloadedInputData.IsDualFuelAccount)
						{
							configurationSelectionResults.Add(

								new AccountsPaymentConfigurationResult.AccountConfigurationInfo
								{
									IsNewAccount = false,
									TargetAccountType = secondaryAccount.UserAccount.ClientAccountType,
									Account = secondaryAccount.UserAccount,
									BankAccount = secondaryAccount.IncomingBankAccount,
								});
						}
						preloadedInputData.FlowResult.ConfigurationSelectionResults =
							configurationSelectionResults;
						break;
					case AccountsPaymentConfigurationFlowStartType.ShowHistory:
					case AccountsPaymentConfigurationFlowStartType.ConfigureDirectDebit:
					case AccountsPaymentConfigurationFlowStartType.EditDirectDebit:
					case AccountsPaymentConfigurationFlowStartType.EstimateYourCost:
					case AccountsPaymentConfigurationFlowStartType.UseExistingAccountDirectDebit:
					case AccountsPaymentConfigurationFlowStartType.EqualizerMonthlySetup:
						preloadedInputData.FlowResult.ConfigurationSelectionResults = new[]
						{
							new AccountsPaymentConfigurationResult.AccountConfigurationInfo
							{
								IsNewAccount = false,
								TargetAccountType = primaryAccount.UserAccount.ClientAccountType,
								Account = primaryAccount.UserAccount,
								BankAccount = primaryAccount.IncomingBankAccount
							}
						};
						//in these cases do nothing since they don't return results
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			async Task<List<InputAccount>> ResolveInputAccounts()
			{
				var getAccountInfo1Task =
					_domainQueryResolver.GetAccountInfoByAccountNumber(preloadedInputData.AccountNumber);

				Task<AccountInfo> getAccountInfo2Task = null;
				if (!string.IsNullOrWhiteSpace(preloadedInputData.SecondaryAccountNumber))
				{
					getAccountInfo2Task =
						_domainQueryResolver.GetAccountInfoByAccountNumber(preloadedInputData.SecondaryAccountNumber);
				}

				var accountInfo1 = await getAccountInfo1Task;
				var bankAccount1 = accountInfo1.IncomingBankAccount;
				var resolveInputAccounts = new List<InputAccount>
				{
					new InputAccount
					{
						UserAccount = accountInfo1,
						IncomingBankAccount = bankAccount1,
						AccountType = accountInfo1.ClientAccountType
					}
				};
				if (getAccountInfo2Task != null)
				{
					var accountInfo2 = await getAccountInfo2Task;
					var bankAccount2 = accountInfo2.IncomingBankAccount;
					resolveInputAccounts.Add(
						new InputAccount
						{
							UserAccount = accountInfo2,
							IncomingBankAccount = bankAccount2,
							AccountType = accountInfo2.ClientAccountType
						});
				}

				return resolveInputAccounts;
			}
		}

		private class InputAccount
		{
			public AccountInfo UserAccount { get; set; }
			public BankAccountInfo IncomingBankAccount { get; set; }
			public ClientAccountType AccountType { get; set; }
		}

		public class RootScreenModel : InitialFlowScreenModel, IAccountsPaymentConfigurationFlowInput
		{
			public AccountsPaymentConfigurationFlowStartType StartType { get; set; }

			public string AccountNumber { get; set; }
			public string SecondaryAccountNumber { get; set; }

			public AccountsPaymentConfigurationResult FlowResult { get; set; }

			public AccountsPaymentConfigurationResult.AccountConfigurationInfo CurrentAccount()
			{
				return FlowResult.ConfigurationSelectionResults.FirstOrDefault(x => !x.ConfigurationCompleted);
			}

			public AccountsPaymentConfigurationResult.AccountConfigurationInfo PrimaryAccount()
			{
				return FlowResult.ConfigurationSelectionResults.First(x => x.Account != null);
			}
			public AccountsPaymentConfigurationResult.AccountConfigurationInfo LastCompletedAccount()
			{
				return FlowResult.ConfigurationSelectionResults.LastOrDefault(x => x.ConfigurationCompleted);
			}

			public bool IsDualFuelAccount { get; set; }
		}
	}
}