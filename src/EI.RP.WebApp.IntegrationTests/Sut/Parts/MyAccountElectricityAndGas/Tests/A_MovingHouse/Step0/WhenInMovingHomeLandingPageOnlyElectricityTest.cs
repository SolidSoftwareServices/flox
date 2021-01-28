using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step0
{
    [TestFixture]
    class WhenInMovingHomeLandingPageOnlyElectricityTest : WhenInMovingHomePageTest<Step0LandingPage>
    {
        [Test]
        public override async Task CanSeeComponents()
        {
            var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
            Assert.IsTrue(Sut.OnMovingDay.Text().Contains("On moving day"));
            Assert.IsTrue( Sut.AfterYouMoveIn.Text().Contains("After you move in"));
            Assert.IsTrue(Sut.CompleteMoveOnline.Text().Contains("Complete move online"));
            Assert.IsTrue(Sut.AddressHeader.TextContent.Contains("You have account for Electricity"));
            Assert.NotNull(Sut.AddressContent.TextContent);
            Assert.IsTrue(Sut.AccountName.TextContent.Contains(accountInfo.ClientAccountType));
            Assert.IsTrue(Sut.AccountNumberDetail.TextContent.Contains(accountInfo.AccountNumber));

            Assert.IsNotNull(Sut.MoveElectricity);
            Assert.IsNotNull("Move Electricity", Sut.MoveElectricity.TextContent);

            Assert.IsNotNull(Sut.MoveElectricityAndAddGas);
            Assert.IsNotNull("Move Electricity & Add Gas",Sut.MoveElectricityAndAddGas.TextContent);

            Assert.IsNotNull(Sut.CloseElectricity);
            Assert.AreEqual("Close Electricity",Sut.CloseElectricity.TextContent);
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
    }
}
