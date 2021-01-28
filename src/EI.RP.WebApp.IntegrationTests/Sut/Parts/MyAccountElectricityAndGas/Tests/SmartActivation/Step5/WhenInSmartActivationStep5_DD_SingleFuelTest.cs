using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step5
{
	[TestFixture]
	class WhenInSmartActivationStep5_DD_SingleFuelTest : WhenInSmartActivationStep5Test
	{
		protected override PaymentMethodType PaymentMethod => PaymentMethodType.DirectDebit;
		protected override bool IsDualFuel => false;
		protected override BillingFrequencyType BillingFrequency => BillingFrequencyType.EveryMonth;
		protected override int BillingDayOfMonth => 12;
		
		protected override List<SetUpDirectDebitDomainCommand> GetDirectDebitCommands()
		{
			var account = UserConfig.Accounts.First();
			return new List<SetUpDirectDebitDomainCommand>
			{
				new SetUpDirectDebitDomainCommand(account.AccountNumber, account.IncomingBankAccount.NameInBankAccount, account.IncomingBankAccount.IBAN, account.IncomingBankAccount.IBAN, account.Partner,
					account.ClientAccountType, PaymentMethodType.DirectDebit)
			};
		}
	}
}