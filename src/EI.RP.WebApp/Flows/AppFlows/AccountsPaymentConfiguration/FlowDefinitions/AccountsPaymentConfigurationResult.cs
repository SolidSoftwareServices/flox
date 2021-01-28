using System.Collections.Generic;
using Ei.Rp.DomainModels.Banking;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using Newtonsoft.Json;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions
{
	public class AccountsPaymentConfigurationResult
	{
        public ExitType Exit { get; set; } = ExitType.NoExitedYet;

		public IEnumerable<AccountConfigurationInfo> ConfigurationSelectionResults { get; set; } =
			new AccountConfigurationInfo[0];

		public enum ExitType
		{
			NoExitedYet = 0,
			ErrorOcurred = 1,
			UserCancelled,
			CompletedWithResult
		}

		public class AccountConfigurationInfo
		{
			public PaymentSetUpType SelectedPaymentSetUpType { get; set; }
			public bool IsNewAccount { get; set; }

			public bool ConfigurationCompleted { get; set; } = false;

			public SetUpDirectDebitInfo CommandToExecute { get; set; }
			public AccountInfo Account { get; set; }
			public BankAccountInfo BankAccount { get; set; }
			public ClientAccountType TargetAccountType { get; set; }

			public class SetUpDirectDebitInfo
			{
				[JsonConstructor]
				public SetUpDirectDebitInfo()
				{
				}

				public SetUpDirectDebitInfo(
					AccountConfigurationInfo baseAccount,
					ClientAccountType targetAccountType)
				{
					BasedOnAccountNumber = baseAccount.Account.AccountNumber;
					NameOnBankAccount = baseAccount.BankAccount?.NameInBankAccount;
					IBAN = baseAccount.BankAccount?.IBAN;
					BusinessPartner = baseAccount.Account.Partner;
					AccountType = targetAccountType;
				}

				public string IBAN { get; set; }

				public string BasedOnAccountNumber { get; set; }
				public string NameOnBankAccount { get; set; }
				public string BusinessPartner { get; set; }
				public ClientAccountType AccountType { get; set; }
                
				public SetUpDirectDebitDomainCommand ToCommand()
				{
					return new SetUpDirectDebitDomainCommand(BasedOnAccountNumber, NameOnBankAccount, null, IBAN,
						BusinessPartner, AccountType, PaymentMethodType.DirectDebit, null, null, null);
				}
			}
        }
	}
}