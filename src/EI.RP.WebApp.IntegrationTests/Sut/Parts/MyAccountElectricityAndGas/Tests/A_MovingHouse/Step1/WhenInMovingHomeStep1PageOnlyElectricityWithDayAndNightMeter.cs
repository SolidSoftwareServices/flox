using System;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step1
{
    [TestFixture]
    class WhenInMovingHomeStep1PageOnlyElectricityWithDayAndNightMeter : MyAccountCommonTests<Step1InputMoveOutPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(configureDefaultDevice:false).WithElectricityDayAndNightDevices();
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
            var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.Single();

            var device = accountConfigurators.Premise.Devices.Single();
            var elecDayDevicesMeterReading = device.Registers.Single(x => x.MeterType == MeterType.ElectricityDay);
            var elecNightDevicesMeterReading = device.Registers.Single(x => x.MeterType == MeterType.ElectricityNight);

            Assert.AreEqual(movingOutSelectedDateTime.ToShortDateString(), Sut.MoveOutDatePicker.Value);

            Assert.AreEqual(true, Sut.IncomingOccupantNoCheckedBox.IsChecked);
            Assert.AreEqual(false, Sut.IncomingOccupantYesCheckedBox.IsChecked);
            Assert.IsTrue(Sut.MPRNHeader.TextContent.Contains(userInfo.PointReferenceNumber.ToString()));
        }

        [Test]
        public async Task ThrowsValidations_Fields_Empty()
        {
            var userInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
            var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.Single();

            var sut = (await Sut.ClickOnElement(Sut.GetNextPRNButton())).CurrentPageAs<Step1InputMoveOutPage>();
            Assert.IsTrue(sut.MPRNHeader.TextContent.Contains(userInfo.PointReferenceNumber.ToString()));
            Assert.IsTrue(Sut.ElectricityMeterReadingDescription.TextContent.Contains("We'll need your meter readings from the day of your move to ensure your final bill is accurate."));
		}
    }
}
