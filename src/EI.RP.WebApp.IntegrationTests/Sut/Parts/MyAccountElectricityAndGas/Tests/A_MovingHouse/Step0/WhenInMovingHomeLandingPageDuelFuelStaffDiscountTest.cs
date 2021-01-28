using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step0
{
    [TestFixture]
    class WhenInMovingHomeLandingPageDuelFuelStaffDiscountTest : WhenInMovingHomePageTest<Step0LandingPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
                UserConfig.AddElectricityAccount(hasStaffDiscount: true);
            UserConfig.AddGasAccount(
	            duelFuelSisterAccount: UserConfig.ElectricityAndGasAccountConfigurators.Single());
            UserConfig.Execute();

            await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
            await App.CurrentPageAs<AccountSelectionPage>()
	            .SelectAccount(UserConfig.Accounts.Last().AccountNumber);
            await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();

            Sut = App.CurrentPageAs<Step0LandingPage>();
        }

        [Test]
        public override async Task CanSeeComponents()
        {
            Assert.IsNotNull(Sut.CloseGasAndEle);
            Assert.AreEqual("Close Electricity & Gas", Sut.CloseGasAndEle.TextContent);
        }

        [Test]
        public async Task CheckHasNoExitFeeNotice()
        {
	        Assert.IsNull(Sut.ExitFeeCloseBothDetail);
	        Assert.IsNull(Sut.ExitFeeMoveAndCloseDetail);
        }

        [Test]
        public async Task CheckHasNoInstalmentPlanNotice()
        {
	        Assert.IsNull(Sut.InstalmentPlanDetail);
        }

        [Test]
        public async Task CheckHasMoveOptionForElectricityOrAddGas()
        {
	        Assert.IsNull(Sut.MoveElectricity);
	        Assert.IsNull(Sut.MoveElectricityAndAddGas);
        }
    }
}