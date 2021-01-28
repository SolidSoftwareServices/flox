using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step0
{
    [TestFixture]
    class WhenInMovingHomeLandingPageDuelFuelGasExitFeeTest : WhenInMovingHomePageTest<Step0LandingPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
                UserConfig.AddElectricityAccount();
                UserConfig.AddGasAccount(
                    hasExitFee: true,
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
            Assert.IsNotNull(Sut.MoveGasAndElec);
            Assert.IsNotNull("Move Electricity & Gas", Sut.MoveGasAndElec.TextContent);

            Assert.IsNotNull(Sut.MoveElectricityAndCloseGas);
            Assert.IsNotNull("Move Electricity & Close Gas", Sut.MoveElectricityAndCloseGas.TextContent);

            Assert.IsNotNull(Sut.CloseGasAndEle);
            Assert.AreEqual("Close Electricity & Gas", Sut.CloseGasAndEle.TextContent);
        }

        [Test]
        public async Task CheckHasExitFeeNotice()
        {
            var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Last().Model;
            Assert.IsNotNull(Sut.ExitFeeCloseBothDetail);
            var feeInfoTextCloseBoth = $"PLEASE NOTE: The Gas Account ({accountInfo.AccountNumber}) is still in contract. By closing this account you will incur an early exit fee of €50. This will be added to your final bill.";
            Assert.IsTrue(Sut.ExitFeeCloseBothDetail.TextContent.Contains(feeInfoTextCloseBoth));
            Assert.IsNotNull(Sut.ExitFeeMoveAndCloseDetail);
            var feeInfoTextMoveAndClose = $"PLEASE NOTE: The Gas Account ({accountInfo.AccountNumber}) is still in contract. By closing this account you will incur an early exit fee of €50. This will be added to your final bill.";
            Assert.IsTrue(Sut.ExitFeeMoveAndCloseDetail.TextContent.Contains(feeInfoTextMoveAndClose));
        }

        [Test]
        public async Task CheckHasNoInstalmentPlanNotice()
        {
            Assert.IsNull(Sut.InstalmentPlanDetail);
        }
    }
}