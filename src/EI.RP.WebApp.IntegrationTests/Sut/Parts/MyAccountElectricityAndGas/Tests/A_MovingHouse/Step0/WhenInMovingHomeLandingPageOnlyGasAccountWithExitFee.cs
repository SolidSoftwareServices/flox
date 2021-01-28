using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step0
{
    [TestFixture]
    class WhenInMovingHomeLandingPageOnlyGasAccountWithExitFee : WhenInMovingHomePageTest<Step0LandingPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddGasAccount(hasExitFee: true);
            UserConfig.Execute();
            var withValidSessionFor = await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount(); ;
            var a = await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();
            Sut = a.CurrentPageAs<Step0LandingPage>();
        }

        [Test]
        public override async Task CanSeeComponents()
        {
            var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
            Assert.IsTrue(Sut.AddressHeader.TextContent.Contains("You have account for Gas at this address"));
            Assert.NotNull(Sut.AddressContent.TextContent);
            Assert.IsTrue(Sut.AccountName.TextContent.Contains(accountInfo.ClientAccountType));
            Assert.IsTrue(Sut.AccountNumberDetail.TextContent.Contains(accountInfo.AccountNumber));
            Assert.IsNotNull(Sut.MoveGas);
            Assert.IsNotNull("Move Gas", Sut.MoveGas.TextContent);

            Assert.IsNotNull(Sut.MoveGasAndAddElectricity);
            Assert.IsNotNull("Move Gas & Add Electricity", Sut.MoveGasAndAddElectricity.TextContent);

            Assert.IsNotNull(Sut.CloseGas);
            Assert.AreEqual("Close Gas", Sut.CloseGas.TextContent);
        }

        [Test]
        public async Task CheckHasExitFeeNotice()
        {
            var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
            Assert.IsNotNull(Sut.ExitFeeCloseBothDetail);
            var feeInfoText = $"PLEASE NOTE: The Gas Account ({accountInfo.AccountNumber}) is still in contract. By closing this account you will incur an early exit fee of €50. This will be added to your final bill.";
            Assert.IsTrue(Sut.ExitFeeCloseBothDetail.TextContent.Contains(feeInfoText));
            Assert.IsNull(Sut.ExitFeeMoveAndCloseDetail);
        }

        [Test]
        public async Task CheckHasNoInstalmentPlanNotice()
        {
            Assert.IsNull(Sut.InstalmentPlanDetail);
        }
    }
}
