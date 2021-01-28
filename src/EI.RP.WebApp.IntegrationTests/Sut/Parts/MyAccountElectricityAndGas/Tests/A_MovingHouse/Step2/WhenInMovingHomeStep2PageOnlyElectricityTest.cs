using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step2
{
    [TestFixture]
    class WhenInMovingHomeStep2PageOnlyElectricityTest : MyAccountCommonTests<Step2InputPrnsPage>
    {
        protected override async Task TestScenarioArrangement()
        {

            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(configureDefaultDevice: false).WithElectricity24HrsDevices();
            UserConfig.Execute();

            await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();
            var movingHomeLandingPage = App.CurrentPageAs<Step0LandingPage>();

            await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1);
            var step1Page = App.CurrentPageAs<Step1InputMoveOutPage>();
			step1Page=step1Page.InputFormValues(UserConfig);
            await step1Page.ClickOnElement(step1Page.GetNextPRNButton());

			Sut = App.CurrentPageAs<Step2InputPrnsPage>();
        }

        [Test]
        public async Task WhenClickOnSubmitPRNS_WithEmptyValues_ShowsValidationError()
        {
			Sut.MPRNInput.Value = "";
			await Sut.ClickOnElement(Sut.SubmitPRNS);
            var sut = App.CurrentPageAs<Step2InputPrnsPage>();
            Assert.IsTrue(sut.MPRNInput.ClassList.Contains("input-validation-error"));
        }

		[Test]
		public async Task WhenClickOnSubmitPrns_WithSameMoveInPrnAsMoveOut_ShowsValidationErrors()
		{
			Sut.MPRNInput.Value = UserConfig.ElectricityAccount().Premise.ElectricityPrn.ToString();
			await Sut.ClickOnElement(Sut.SubmitPRNS);
			var sut = App.CurrentPageAs<Step2InputPrnsPage>();
			Assert.AreEqual(
				Step2InputPrnsPage.ValidationMessages.MprnIsTheSameAsTheHomeYouAreLeaving,
				sut.MPRNInputError.TextContent);
		}

		[Test]
        public async Task WhenClickOnSubmitPRNS_NotValidFormatPRNS_ShowsValidationError()
        {
            Sut.MPRNInput.Value = "124";

			await Sut.ClickOnElement(Sut.SubmitPRNS);
			var sut = App.CurrentPageAs<Step2InputPrnsPage>();
			Assert.IsTrue(sut.MPRNInput.ClassList.Contains("input-validation-error"));
        }

        [Test]
        public async Task CanSeeComponents()
        {
            Assert.IsNotNull(Sut.InputTitle);
            Assert.IsTrue(Sut.InputTitle.TextContent.Contains("Enter your new MPRN"));
            Assert.IsNotNull(Sut.MPRNInput);
            Assert.IsNotNull(Sut.MPRNLabel);
            Assert.IsTrue(Sut.MPRNLabel.TextContent.Contains("MPRN"));
            Assert.IsNotNull(Sut.SubmitPRNS);
           
        }

        [Test]
        public async Task WhenClickOnSubmit()
        {
	        Sut.InputFormValues(UserConfig);
	        await Sut.ClickOnElement(Sut.SubmitPRNS);
	        var sut = App.CurrentPageAs<Step2ConfirmAddressPage>();
            Assert.IsNotNull(sut.AddressTitle);
            Assert.IsTrue(sut.AddressTitle.TextContent.Contains("The following address matches the MPRN you entered"));

            Assert.IsNotNull(sut.MPRNTitleAndNumber);
            Assert.IsTrue(sut.MPRNTitleAndNumber.TextContent.Contains("MPRN"));
            Assert.IsTrue(sut.MPRNTitleAndNumber.TextContent.Contains(Sut.MPRNInput.Value));

            Assert.IsNotNull(sut.MPRNAddress);

            Assert.IsTrue(sut.NeedHelp.TextContent.Contains("Need help? Contact us on 1850 372 372 to complete your move"));
            Assert.IsNotNull(sut.ButtonContinue);
            Assert.AreEqual("Continue", sut.ButtonContinue.TextContent);

            Assert.IsNotNull(sut.ButtonReEnter);
            Assert.IsTrue(sut.ButtonReEnter.TextContent.Contains("Re-enter my details"));
        }

        [Test]
        public async Task WhenClickOnSubmit_IfMPRN_NotExist()
        {
         

            var newMprn = (string)Fixture.Create<ElectricityPointReferenceNumber>();
            Sut.MPRNInput.Value =newMprn;

            var sut = (await Sut.ClickOnElement(Sut.SubmitPRNS)).CurrentPageAs<Step2InputPrnsPage>();
            Assert.NotNull(sut.TrySubmitNewPRNS);
            Assert.IsTrue(sut.TrySubmitNewPRNS.TextContent.Contains("Try New MPRN again"));
            Assert.NotNull(sut.NeedHelp);
            Assert.IsTrue(sut.NeedHelp.TextContent.Contains("Need help? Contact us on 1850 372 372 to complete your move"));

            Sut.MPRNInput.Value = newMprn;
            var sutErrorPage = (await sut.ClickOnElement(sut.TrySubmitNewPRNS)).CurrentPageAs<MovingHomeErrorPage>();
            Assert.IsNotNull(sutErrorPage.ErrorMessage);
            Assert.IsTrue(sutErrorPage.ErrorMessage.TextContent.Contains("We are having trouble locating your MPRN"));

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
