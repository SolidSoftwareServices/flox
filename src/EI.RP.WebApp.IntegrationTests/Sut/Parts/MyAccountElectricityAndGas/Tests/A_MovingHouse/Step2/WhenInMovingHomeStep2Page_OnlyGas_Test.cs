using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step2
{
    class WhenInMovingHomeStep2Page_OnlyGas_Test : MyAccountCommonTests<Step2InputPrnsPage>
    {
        protected override async Task TestScenarioArrangement()
        {

            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddGasAccount();
            UserConfig.Execute();

            var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            var movingHomeLandingPage = (await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome()).CurrentPageAs<Step0LandingPage>();

            var step1Page = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1))
                .CurrentPageAs<Step1InputMoveOutPage>();


            await step1Page.InputFormValues(UserConfig).ClickOnElement(step1Page.GetNextPRNButton());
            Sut = App.CurrentPageAs<Step2InputPrnsPage>();
        }


        [Test]
        public async Task CanSeeComponents()
        {
            Assert.IsNotNull(Sut.InputTitle);
            Assert.IsTrue(Sut.InputTitle.TextContent.Contains("Enter your new GPRN"));
            Assert.IsNotNull(Sut.GPRNInput);
            Assert.IsNotNull(Sut.GPRNLabel);
            Assert.IsTrue(Sut.GPRNLabel.TextContent.Contains("GPRN"));
            Assert.IsNotNull(Sut.SubmitPRNS);
        }

        [Test]
        public async Task WhenClickOnSubmitPRNS_WithEmptyValues_ShowsValidationError()
        {
            Sut.GPRNInput.Value = "";

			await Sut.ClickOnElement(Sut.SubmitPRNS);
			var sut = App.CurrentPageAs<Step2InputPrnsPage>();
			Assert.IsTrue(sut.GPRNInput.ClassList.Contains("input-validation-error"));
        }

		[Test]
		public async Task WhenClickOnSubmitPrns_WithSameMoveInPrnAsMoveOut_ShowsValidationErrors()
		{
			Sut.GPRNInput.Value = UserConfig.GasAccount().Premise.GasPrn.ToString();
			await Sut.ClickOnElement(Sut.SubmitPRNS);
			var sut = App.CurrentPageAs<Step2InputPrnsPage>();
			Assert.AreEqual(
				Step2InputPrnsPage.ValidationMessages.GprnIsTheSameAsTheHomeYouAreLeaving,
				sut.GasOnlyGPRNInputError.TextContent);
		}

		[Test]
        public async Task WhenClickOnSubmitPRNS_NotValidFormatPRNS_ShowsValidationError()
        {
            Sut.GPRNInput.Value = "124";

            await Sut.ClickOnElement(Sut.SubmitPRNS);
            var sut = App.CurrentPageAs<Step2InputPrnsPage>();
            Assert.IsTrue(sut.GPRNInput.ClassList.Contains("input-validation-error"));
        }

        [Test]
        public async Task WhenClickOnSubmit()
        {
            Sut.GPRNInput.Value =(string) UserConfig.ElectricityAndGasAccountConfigurators
                .FirstOrDefault(x => x.Model.ClientAccountType == ClientAccountType.Gas)
                ?.NewPremise.GasPrn;

            await Sut.ClickOnElement(Sut.SubmitPRNS);
            var sut = App.CurrentPageAs<Step2ConfirmAddressPage>();
            Assert.IsNotNull(sut.AddressTitle);
            Assert.IsTrue(sut.AddressTitle.TextContent.Contains("The following address matches the GPRN you entered"));

            Assert.IsNotNull(sut.GPRNTitleAndNumberOnlyIfGasAccountExits);
            Assert.IsTrue(sut.GPRNTitleAndNumberOnlyIfGasAccountExits.TextContent.Contains("GPRN"));
            Assert.IsTrue(sut.GPRNTitleAndNumberOnlyIfGasAccountExits.TextContent.Contains(Sut.GPRNInput.Value));

            Assert.IsNotNull(sut.GPRNAddressOnlyIfGasAccountExits);

            Assert.IsTrue(sut.NeedHelp.TextContent.Contains("Need help? Contact us on 1850 372 372 to complete your move"));
            Assert.IsNotNull(sut.ButtonContinue);
            Assert.AreEqual("Continue", sut.ButtonContinue.TextContent);

            Assert.IsNotNull(sut.ButtonReEnter);
            Assert.IsTrue(sut.ButtonReEnter.TextContent.Contains("Re-enter my details"));
        }

        [Test]
        public async Task WhenClickOnSubmit_IsGPRN_NotExist()
        {

            var newGprn = (string)Fixture.Create<GasPointReferenceNumber>();
			Sut.GPRNInput.Value = newGprn;

          
            var sutErrorPage = (await Sut.ClickOnElement(Sut.SubmitPRNS)).CurrentPageAs<MovingHomeErrorPage>();
            Assert.IsNotNull(sutErrorPage.ErrorMessage);
            Assert.IsTrue(sutErrorPage.ErrorMessage.TextContent.Contains("We cannot process your move request at the moment"));

            Assert.IsNotNull(sutErrorPage.ContactNumber);
            Assert.IsTrue(sutErrorPage.ContactNumber.TextContent.Contains("Please call 1850 372 372"));

            Assert.IsNotNull(sutErrorPage.BackToAccounts);
            Assert.IsTrue(sutErrorPage.BackToAccounts.TextContent.Contains("Back to My Accounts"));
            var accountsPage = (await sutErrorPage.ClickOnElement(sutErrorPage.BackToAccounts))
                .CurrentPageAs<AccountSelectionPage>();
        }

        [Test]
        public async Task WhenClickOnCancel()
        {
            Assert.IsNotNull(Sut.CancelButton);
            Assert.IsTrue(Sut.CancelButton.TextContent.Contains("Cancel"));

            var sutLandingPage = (await Sut.ClickOnElement(Sut.CancelMovePage))
                .CurrentPageAs<Step0LandingPage>();
        }
    }
}

