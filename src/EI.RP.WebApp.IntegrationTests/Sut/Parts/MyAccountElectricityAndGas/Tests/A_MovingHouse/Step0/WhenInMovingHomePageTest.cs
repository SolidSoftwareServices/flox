using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step0
{
    [TestFixture]
    abstract class WhenInMovingHomePageTest<TPage> : MyAccountCommonTests<TPage>
        where TPage : Step0LandingPage
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount();
            UserConfig.Execute();
            var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            var a = await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();
            Sut = a.CurrentPageAs<TPage>();
        }

        [Test]
        public abstract Task CanSeeComponents();
    }
}
