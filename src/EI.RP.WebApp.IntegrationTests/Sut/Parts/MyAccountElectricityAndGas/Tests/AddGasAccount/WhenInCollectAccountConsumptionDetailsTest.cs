using System.Linq;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AddGasAccount;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AddGasAccount
{
    [TestFixture]
    class WhenInCollectAccountConsumptionDetailsTest : MyAccountCommonTests<CollectAccountConsumptionDetailsPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(canAddNewAccount: true);
            UserConfig.Execute();

            var app = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMeterReading();
            app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToPlanPage();

            Sut = (await app.ClickOnElement(app.CurrentPageAs<PlanPage>().AddGasFlow)).CurrentPageAs<CollectAccountConsumptionDetailsPage>();
        }

        [Test]
        public async Task CanSeeComponentItems()
        {
            Assert.IsNotNull(Sut.ParagraphContent);
            Assert.IsTrue(Sut.ParagraphContent.TextContent.Trim().Contains("By setting up a gas account you will save 3% on your bills"));
            Assert.IsNotNull(Sut.GPRNLabel);
            Assert.AreEqual("GPRN *", Sut.GPRNLabel.TextContent);
            Assert.IsNotNull(Sut.GPRN);
            Assert.IsNotNull(Sut.MeterReadLabel);
            Assert.IsTrue(Sut.MeterReadLabel.TextContent.Contains("Gas meter reading"));
            Assert.IsNotNull(Sut.GasMeterReading);
            Assert.IsNotNull(Sut.AuthorizationCheck);
            Assert.IsNotNull(Sut.PricePlanTermsAndConditions);
            Assert.IsNotNull(Sut.DebtFlagAndArrearsTermsAndConditions);
            Assert.IsNotNull(Sut.TermsAndConditionsAccepted);
        }

        [Test]
        public async Task PrivacyLinkGoesToTermsInfoPage()
        {
            (await Sut.ToPrivacyNoticeViaComponent()).CurrentPageAs<TermsInfoPage>();
        }
    }
}