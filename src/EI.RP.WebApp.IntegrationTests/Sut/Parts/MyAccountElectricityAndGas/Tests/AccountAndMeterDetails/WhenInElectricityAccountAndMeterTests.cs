using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountAndMeterDetails;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountAndMeterDetails
{
    [TestFixture]
    internal class WhenInElectricityAccountAndMeterTests : WhenAccountAndMeterDetailsOnTests
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount();
            UserConfig.Execute();

            await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            Sut = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToAccountAndMeterDetails())
                .CurrentPageAs<AccountAndMeterDetailsPage>();
        }
    }
}