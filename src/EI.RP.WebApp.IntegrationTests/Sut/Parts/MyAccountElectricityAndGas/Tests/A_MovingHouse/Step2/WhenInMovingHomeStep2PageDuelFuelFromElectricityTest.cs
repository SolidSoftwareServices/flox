using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step2
{

    [TestFixture]
    class WhenInMovingHomeStep2PageDuelFuelFromElectricityTest : MyAccountCommonTests<Step2InputPrnsPage>
    {
	    protected override async Task TestScenarioArrangement()
	    {

			UserConfig = App.ConfigureUser("a@A.com", "test");
		    UserConfig.AddElectricityAccount(configureDefaultDevice: false).WithElectricity24HrsDevices();
		    UserConfig.AddGasAccount(duelFuelSisterAccount: UserConfig.ElectricityAndGasAccountConfigurators.Single());
		    UserConfig.Execute();

		    await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);

		    await App.CurrentPageAs<AccountSelectionPage>()
			    .SelectAccount(UserConfig.Accounts.Last().AccountNumber);
		    await App
			    .CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();
		    var movingHomeLandingPage = App.CurrentPageAs<Step0LandingPage>();

		    var step1Page = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1))
			    .CurrentPageAs<Step1InputMoveOutPage>();
		    await step1Page.InputFormValues(UserConfig).ClickOnElement(step1Page.GetNextPRNButton());
		    Sut = App.CurrentPageAs<Step2InputPrnsPage>();
	    }


		[Test]
		public async Task WhenClickOnSubmitPrns_WithSameMoveInPrnAsMoveOut_ShowsValidationErrors()
		{
			Sut.GPRNInput.Value = UserConfig.GasAccount().Premise.GasPrn.ToString();
			Sut.MPRNInput.Value = UserConfig.ElectricityAccount().Premise.ElectricityPrn.ToString();
			await Sut.ClickOnElement(Sut.SubmitPRNS);
			var sut = App.CurrentPageAs<Step2InputPrnsPage>();
			Assert.AreEqual(
				Step2InputPrnsPage.ValidationMessages.GprnIsTheSameAsTheHomeYouAreLeaving,
				sut.GPRNInputError.TextContent);
			Assert.AreEqual(
				Step2InputPrnsPage.ValidationMessages.MprnIsTheSameAsTheHomeYouAreLeaving,
				sut.MPRNInputError.TextContent);
		}

		[Test]
	    public async Task WhenClickOnSubmitPRNS_RedirectTo_ConfirmAddressPage()
	    {
		    await InitialStateIsCorrect();
		    Sut.InputFormValues(UserConfig);
		    var newGprn = (string)Fixture.Create<GasPointReferenceNumber>();

		    Sut.GPRNInput.Value = newGprn;

		    await Sut.ClickOnElement(Sut.SubmitPRNS);
		    var sut = App.CurrentPageAs<Step2ConfirmAddressPage>(); Assert.IsNotNull(sut.AddressTitle);
		    Assert.IsTrue(sut.AddressTitle.TextContent.Contains("The following address matches the MPRN/GPRN you entered"));
		    Assert.IsNotNull(sut.GPRNTitleAndNumber);
		    Assert.IsTrue(sut.GPRNTitleAndNumber.TextContent.Contains("GPRN"));
		    Assert.IsTrue(sut.GPRNTitleAndNumber.TextContent.Contains(Sut.GPRNInput.Value));

		    Assert.IsNotNull(sut.MPRNTitleAndNumber);
		    Assert.IsTrue(sut.MPRNTitleAndNumber.TextContent.Contains("MPRN"));
		    Assert.IsTrue(sut.MPRNTitleAndNumber.TextContent.Contains(Sut.MPRNInput.Value));

		    Assert.IsNotNull(sut.MPRNAddress);

		    Assert.IsTrue(sut.NeedHelp.TextContent.Contains("Need help? Contact us on 1850 372 372 to complete your move"));
		    Assert.IsNotNull(sut.ButtonContinue);
		    Assert.AreEqual("Continue", sut.ButtonContinue.TextContent);

		    Assert.IsNotNull(sut.ButtonReEnter);
		    Assert.IsTrue(sut.ButtonReEnter.TextContent.Contains("Re-enter my details"));

		    async Task InitialStateIsCorrect()
		    {
			    Assert.IsNotNull(Sut.InputTitle);
			    Assert.IsTrue(Sut.InputTitle.TextContent.Contains("Enter your new MPRN and GPRN"));
			    Assert.IsNotNull(Sut.GPRNInput);
			    Assert.IsNotNull(Sut.MPRNInput);
			    Assert.IsNotNull(Sut.GPRNLabel);
			    Assert.IsTrue(Sut.GPRNLabel.TextContent.Contains("GPRN"));
			    Assert.IsNotNull(Sut.MPRNLabel);
			    Assert.IsTrue(Sut.MPRNLabel.TextContent.Contains("MPRN"));
			    Assert.IsNotNull(Sut.SubmitPRNS);
		    }
		}


        [Test]
        public async Task WhenClickOnSubmitPRNS_WithEmptyValues_ShowsValidationError()
        {
            Sut.MPRNInput.Value = "";

            Sut.GPRNInput.Value = "";

            await Sut.ClickOnElement(Sut.SubmitPRNS);
            var sut = App.CurrentPageAs<Step2InputPrnsPage>();
            Assert.IsTrue(sut.MPRNInput.ClassList.Contains("input-validation-error"));
            Assert.IsTrue(sut.GPRNInput.ClassList.Contains("input-validation-error"));
        }

        [Test]
        public async Task WhenClickOnSubmitPRNS_NotValidFormatPRNS_ShowsValidationError()
        {
            Sut.MPRNInput.Value = "123";

            Sut.GPRNInput.Value = "124";
            await Sut.ClickOnElement(Sut.SubmitPRNS);
			var sut = App.CurrentPageAs<Step2InputPrnsPage>();
            Assert.IsTrue(sut.MPRNInput.ClassList.Contains("input-validation-error"));
            Assert.IsTrue(sut.GPRNInput.ClassList.Contains("input-validation-error"));
        }

       
        [Test]
        public async Task WhenClickOnSubmit_IfMPRN_NotExist()
        {
			

			Sut.MPRNInput.Value = Fixture.Create<ElectricityPointReferenceNumber>().ToString(); 
			Sut.GPRNInput.Value = (string)UserConfig.ElectricityAndGasAccountConfigurators
				.FirstOrDefault(x => x.Model.ClientAccountType == ClientAccountType.Gas)
				?.NewPremise.GasPrn; 

            var sut = (await Sut.ClickOnElement(Sut.SubmitPRNS)).CurrentPageAs<Step2InputPrnsPage>();
            Assert.NotNull(sut.TrySubmitNewPRNS);
            Assert.IsTrue(sut.TrySubmitNewPRNS.TextContent.Contains("Try New MPRN/GRPN again"));
            Assert.NotNull(sut.NeedHelp);
            Assert.IsTrue(sut.NeedHelp.TextContent.Contains("Need help? Contact us on 1850 372 372 to complete your move"));

			Sut.MPRNInput.Value = Fixture.Create<ElectricityPointReferenceNumber>().ToString();
			var sutErrorPage = (await sut.ClickOnElement(sut.TrySubmitNewPRNS)).CurrentPageAs<MovingHomeErrorPage>();
            Assert.IsNotNull(sutErrorPage.ErrorMessage);
            Assert.IsTrue(sutErrorPage.ErrorMessage.TextContent.Contains("We are having trouble locating your MPRN"));

            Assert.IsNotNull(sutErrorPage.ContactNumber);
            Assert.IsTrue(sutErrorPage.ContactNumber.TextContent.Contains("Please call 1850 372 372"));

            Assert.IsNotNull(sutErrorPage.BackToAccounts);
            Assert.IsTrue(sutErrorPage.BackToAccounts.TextContent.Contains("Back to My Accounts"));
            (await sutErrorPage.ClickOnElement(sutErrorPage.BackToAccounts))
                .CurrentPageAs<AccountSelectionPage>();
        }

        [Test]
        public async Task WhenClickOnSubmit_If_MPRNAndGPRN_AddressNotMatches()
        {
			Sut.InputFormValues(UserConfig);
			SetInvalidDualGprnAddress();

			var sut = (await Sut.ClickOnElement(Sut.SubmitPRNS)).CurrentPageAs<Step2InputPrnsPage>();
			Assert.NotNull(sut.TrySubmitNewPRNS);
            Assert.IsTrue(sut.TrySubmitNewPRNS.TextContent.Contains("Try New MPRN/GRPN again"));
            Assert.NotNull(sut.NeedHelp);
            Assert.IsTrue(sut.NeedHelp.TextContent.Contains("Need help? Contact us on 1850 372 372 to complete your move"));

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

        private void SetInvalidDualGprnAddress()
        {
	        var gasPointReferenceNumber = Fixture.Create<GasPointReferenceNumber>();
	        Sut.GPRNInput.Value = gasPointReferenceNumber.ToString();
	        App.DomainFacade.QueryResolver.ExpectQuery(new PointOfDeliveryQuery
	        {
		        Prn = gasPointReferenceNumber
	        }, Fixture.Create<PointOfDeliveryInfo>().ToOneItemArray());
        }

        [Test]
        public async Task WhenClickOnSubmit_If_MPRNAndGPRN_AddressNotMatches_2Times()
        {
	        Sut.InputFormValues(UserConfig);
	        SetInvalidDualGprnAddress();
			var sut = (await Sut.ClickOnElement(Sut.SubmitPRNS)).CurrentPageAs<Step2InputPrnsPage>();
			Assert.NotNull(sut.TrySubmitNewPRNS);
            Assert.IsTrue(sut.TrySubmitNewPRNS.TextContent.Contains("Try New MPRN/GRPN again"));
            Assert.NotNull(sut.NeedHelp);
            Assert.IsTrue(sut.NeedHelp.TextContent.Contains("Need help? Contact us on 1850 372 372 to complete your move"));

            Sut.GPRNInput.Value = Fixture.Create<long>().ToString().PadLeft(7, '0');

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