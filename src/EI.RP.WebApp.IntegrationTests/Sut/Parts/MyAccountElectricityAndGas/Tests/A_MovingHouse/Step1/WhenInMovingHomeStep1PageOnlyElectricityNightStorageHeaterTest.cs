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
    class WhenInMovingHomeStep1PageOnlyElectricityNightStorageHeaterTest : MyAccountCommonTests<Step1InputMoveOutPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(configureDefaultDevice:false)
	            .WithElectricityNightStorageHeaterDevice();
            UserConfig.Execute();
            var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();;
            var movingHomeLandingPage = (await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome()).CurrentPageAs<Step0LandingPage>();

            Sut = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1))
                .CurrentPageAs<Step1InputMoveOutPage>();
        }

        [Test]
        public async Task CanSeeComponents()
        {
	        var registerInfo = UserConfig.ElectricityAccounts().First().Premise.Devices.Single().Registers.Single(x => x.MeterType == MeterType.ElectricityNightStorageHeater);
            Assert.IsNotNull( Sut.GetElecReadingInput(registerInfo.MeterNumber), "Expected to see NSH input");

        }
    }
}