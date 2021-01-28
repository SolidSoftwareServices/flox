using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.Gas;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AddGasAccount;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AddGasAccount
{
    [TestFixture]
    internal class WhenInChooseOptionsExistingDirectDebitTest: WhenInCollectAccountConsumptionDetailsTest
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(paymentType: PaymentMethodType.DirectDebit, canAddNewAccount: true);
			UserConfig.Execute();

            var app = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();

            app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMeterReading();
            app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToPlanPage();
            Sut = (await app.ClickOnElement(app.CurrentPageAs<PlanPage>().AddGasFlow)).CurrentPageAs<CollectAccountConsumptionDetailsPage>();
        }

        [Test]
        public async Task WhenConfigureUseExistingDirectDebitForGasAccount()
        {
	        var meterReading = 123435;
	        Sut.InputFormValues(UserConfig, meterReading);
			
            var sut = (await Sut.ClickOnElement(Sut.SubmitButton)).CurrentPageAs<ConfirmAddressPage>();
            var sutOptionsPage = (await sut.ClickOnElement(sut.ConfirmAddressButton)).CurrentPageAs<ChoosePaymentOptionsPage>();
            Assert.IsNotNull(sutOptionsPage);
            Assert.IsNotNull(sutOptionsPage.UseUpExistingDirectDebitHeader);
            Assert.IsNotNull(sutOptionsPage.CheckBoxConfirmContinueDebit);
            sutOptionsPage.CheckBoxConfirmContinueDebit.IsChecked = true;
            Assert.AreEqual("Use existing Direct Debit", sutOptionsPage.UseUpExistingDirectDebitButton.TextContent);

            var sutExistingDirectDebitConfirmation = (await sutOptionsPage.ClickOnElement(sutOptionsPage.UseUpExistingDirectDebitButton)).CurrentPageAs<AddGasDirectDebitPageConfirmation>();

            Assert.IsNotNull(sutExistingDirectDebitConfirmation);
            Assert.AreEqual("Gas Account Set Up & Savings Confirmation", sutExistingDirectDebitConfirmation.Heading.TextContent);


            Assert.IsNotNull(sutOptionsPage.SetUpNewDirectDebitButton);
            Assert.AreEqual("Set up new Direct Debit", sutOptionsPage.SetUpNewDirectDebitButton.TextContent);

            Assert.IsNotNull(sutOptionsPage.GoBack);
            Assert.IsTrue(sutOptionsPage.GoBack.TextContent.Trim().Contains("Go back"));
			
			var baseAccount = UserConfig.ElectricityAndGasAccountConfigurators.Single();
			
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(new AddGasAccountCommand((GasPointReferenceNumber)baseAccount.NewDuelFuelAccountConfigurator.Prn, baseAccount.Model.AccountNumber, meterReading,PaymentSetUpType.UseExistingDirectDebit, null, null));
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<SetUpDirectDebitDomainCommand>();
        }
    }
}