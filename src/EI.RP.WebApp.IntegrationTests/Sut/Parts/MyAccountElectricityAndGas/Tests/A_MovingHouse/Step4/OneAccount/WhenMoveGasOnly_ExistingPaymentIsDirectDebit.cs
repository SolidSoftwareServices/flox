using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.OneAccount
{
    [TestFixture]
	class WhenMoveGasOnly_ExistingPaymentIsDirectDebit : WhenMoveElectricityOnly_ExistingPaymentIsDirectDebit
	{
        protected override ClientAccountType ScenarioAccountType { get; } = ClientAccountType.Gas;
        protected override PaymentMethodType ScenarioPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

        protected override bool IsPRNDegistered { get; } = false;

        public override async Task HandlerUserPath_UserHasExistingDD_ConfirmChecked_GoesToNextStep()
        {
	        Sut.ConfirmContinueDebit.IsChecked = true;
	        var account = UserConfig.GasAccounts().Single();

	        var step5 = (await Sut.ClickOnElement(Sut.UseExistingDirectDebitButton))
		        .CurrentPageAs<Step5ReviewAndCompletePage>();
	        AssertStep5Review(step5);
	        Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
	        Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(account.Model.IncomingBankAccount.IBAN.Substring(account.Model.IncomingBankAccount.IBAN.Length - 4)));
        }
	}
}