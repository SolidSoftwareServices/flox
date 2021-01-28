using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using Moq;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step1
{
    [TestFixture]
    class WhenInMovingHomeStep1PageOnlyElectricityTest : MyAccountCommonTests<Step1InputMoveOutPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(configureDefaultDevice: false).WithElectricity24HrsDevices();
            UserConfig.Execute();
            var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            var movingHomeLandingPage = (await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome()).CurrentPageAs<Step0LandingPage>();

            Sut = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1))
                .CurrentPageAs<Step1InputMoveOutPage>();
        }

        [Test]
        public async Task CanSeeComponents()
        {
            var userInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
            var movingOutSelectedDateTime = userInfo.ContractStartDate == DateTime.Now.AddDays(-1)
                ? DateTime.Now.Date
                : DateTime.Now.AddDays(-1).Date;

            var elecDevicesMeterReading = UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices.Single().Registers.Single();
            Assert.AreEqual(movingOutSelectedDateTime.ToShortDateString(), Sut.MoveOutDatePicker.Value);
            Assert.IsTrue(Sut.ElecMeterReadingTypeTitle.TextContent.Contains(elecDevicesMeterReading.MeterType));
            Assert.IsTrue(Sut.ElectricityMeterReadingDescription.TextContent.Contains("We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
            Assert.IsTrue(Sut.MPRNHeader.TextContent.Contains(userInfo.PointReferenceNumber.ToString()));
            Assert.AreEqual(true, Sut.IncomingOccupantNoCheckedBox.IsChecked);
            Assert.AreEqual(false, Sut.IncomingOccupantYesCheckedBox.IsChecked);
            Assert.IsTrue(Sut.GetNextPRNButton().TextContent.StartsWith("Next - New "), "NextPrnButton should start with: Next - New ");

            AssertHasCorrectReadGeneralTermsLink(Sut);
        }

        [Test]
        public async Task TermsAndConditionsCheckBoxIsStillVisibleOnValidationFailure()
        {
	        AssertHasCorrectReadGeneralTermsLink(Sut);
			await Sut.ClickOnElement(Sut.GetNextPRNButton());
	        AssertHasCorrectReadGeneralTermsLink(App.CurrentPageAs<Step1InputMoveOutPage>());
		}

        [Test]
        public async Task ThrowsValidations_Fields_Empty()
        {
            var userInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
         

            var elecDevicesMeterReading = UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices.Single()
	            .Registers.Single();
            Assert.IsTrue(Sut.ElecMeterReadingTypeTitle.TextContent.Contains(elecDevicesMeterReading.MeterType));
            Assert.IsTrue(Sut.MPRNHeader.TextContent.Contains(userInfo.PointReferenceNumber.ToString()));

            var sut = (await Sut.ClickOnElement(Sut.GetNextPRNButton())).CurrentPageAs<Step1InputMoveOutPage>();
            Assert.IsTrue(sut.MPRNHeader.TextContent.Contains(userInfo.PointReferenceNumber.ToString()));
            Assert.IsTrue(Sut.ElectricityMeterReadingDescription.TextContent.Contains("We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
        }

        [Test]
        public async Task ShowErrorPage_Pod_Locked()
        {
            var step1Page = App.CurrentPageAs<Step1InputMoveOutPage>();
            step1Page = step1Page.InputFormValues(UserConfig);

            UserConfig.DomainFacade.QueryResolver.Current.Setup(_ => 
                _.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Throws(new DomainException(ResidentialDomainError.PoDLocked));

            var page = await step1Page.ClickOnElement(step1Page.GetNextPRNButton());
            var sutErrorPage = page.CurrentPageAs<MovingHouseUnhandledErrorPage>();

            Assert.IsNotNull(sutErrorPage.Title);
            Assert.IsNotNull(sutErrorPage.Subtitle);

            Assert.IsNotNull(sutErrorPage.ContactNumber);
            Assert.IsTrue(sutErrorPage.ContactNumber.TextContent.Contains("Please call 1850 372 372 to complete your move"));

            Assert.IsNotNull(sutErrorPage.BackToAccounts);
            Assert.IsTrue(sutErrorPage.BackToAccounts.TextContent.Contains("Back to My Accounts"));
            (await sutErrorPage.ClickOnElement(sutErrorPage.BackToAccounts))
                .CurrentPageAs<AccountSelectionPage>();
        }

        private void AssertHasCorrectReadGeneralTermsLink(Step1InputMoveOutPage page)
        {
	        Assert.IsNotNull(page.CheckBoxTerms);
	        var electricityLink = page.GeneralTermsAndConditionsLinks.Single();
	        Assert.IsTrue(electricityLink.Attributes["href"].Value.ToLowerInvariant() == "https://electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity", "expected to see link to terms and conditions for electricity");
        }
    }
}