using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step0
{
    [TestFixture]
    class WhenInMovingHomeLandingPageDuelFuelGasInstalmentPlanTest : WhenInMovingHomePageTest<Step0LandingPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
                UserConfig.AddElectricityAccount();
                UserConfig.AddGasAccount(hasInstalmentPlan: true,
                    duelFuelSisterAccount: UserConfig.ElectricityAndGasAccountConfigurators.Single());
            UserConfig.Execute();

            await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
            await App.CurrentPageAs<AccountSelectionPage>()
	            .SelectAccount(UserConfig.Accounts.First().AccountNumber);
            await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();

            Sut = App.CurrentPageAs<Step0LandingPage>();
        }

        [Test]
        public override async Task CanSeeComponents()
        {
            Assert.IsNotNull(Sut.MoveGasAndElec);
            Assert.IsNotNull("Move Electricity & Gas", Sut.MoveGasAndElec.TextContent);

            Assert.IsNotNull(Sut.MoveElectricityAndCloseGas);
            Assert.IsNotNull("Move Electricity & Close Gas", Sut.MoveElectricityAndCloseGas.TextContent);

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
        public async Task CheckHasInstalmentPlanNotice()
        {
            Assert.IsNotNull(Sut.InstalmentPlanDetail);
            const string infoText = "Your instalment plan will be cancelled and any amount due on your final bill will need to be paid in full.";
            Assert.IsTrue(Sut.InstalmentPlanDetail.TextContent.Contains(infoText));
        }
    }
}