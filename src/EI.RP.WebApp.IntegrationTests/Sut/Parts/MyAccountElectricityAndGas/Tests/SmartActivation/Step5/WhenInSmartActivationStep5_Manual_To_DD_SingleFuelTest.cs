using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step5
{
	[TestFixture]
	class WhenInSmartActivationStep5_Manual_To_DD_SingleFuelTest : WhenInSmartActivationStep5Test
	{
		protected override PaymentMethodType PaymentMethod => PaymentMethodType.Manual;
		protected override BillingFrequencyType BillingFrequency => BillingFrequencyType.EveryMonth;
		protected override int BillingDayOfMonth => 25;
		protected override bool IsDualFuel => false;
		private string _nameInput = "Name Surname";
		private string _ibanInput = "IE62AIBK93104777372010";

		protected override async Task<Step4BillingFrequencyPage> NavigateToStep4(Step3PaymentDetailsPage step3)
		{
			step3.SetupDirectDebitElementForManualPayment.NameInput.Value = _nameInput;
			step3.SetupDirectDebitElementForManualPayment.IbanInput.Value = _ibanInput;
			var step4 = (await step3.ClickOnElement(step3.SetupDirectDebitElementForManualPayment.AddDebitCardButton)).CurrentPageAs<Step4BillingFrequencyPage>();
			return step4;
		}

		protected override string GetExpectedPaymentMethod()
		{
			return PaymentMethodType.DirectDebit.ToUserText();
		}

		protected override List<SetUpDirectDebitDomainCommand> GetDirectDebitCommands()
		{
			var account = UserConfig.Accounts.First();
			return new List<SetUpDirectDebitDomainCommand>
			{
				new SetUpDirectDebitDomainCommand(account.AccountNumber, _nameInput, account.IncomingBankAccount.IBAN, _ibanInput, account.Partner,
					account.ClientAccountType, PaymentMethodType.DirectDebit)
			};
		}
	}
}
