using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountAndMeterDetails;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountAndMeterDetails
{
    [TestFixture]
    class WhenInGasAccountAndMeterTests : WhenAccountAndMeterDetailsOnTests
    {
        protected override async Task TestScenarioArrangement()
        {

            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddGasAccount();
            UserConfig.Execute();

            await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            Sut =(await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToAccountAndMeterDetails()).CurrentPageAs<AccountAndMeterDetailsPage>();

        }
    }
}