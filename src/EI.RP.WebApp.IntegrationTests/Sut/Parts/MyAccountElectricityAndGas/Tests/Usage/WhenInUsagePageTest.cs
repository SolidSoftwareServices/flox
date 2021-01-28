using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ContactUs;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Usage;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Usage
{
    [TestFixture]
    class WhenInUsagePageTest: MyAccountCommonTests<UsagePage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount()
                .WithInvoices(3);
            UserConfig.Execute();

            var app = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMeterReading();
            var tabButton = (IHtmlElement)App.CurrentPage.Document.QuerySelector("[data-testid='nav-usage-link']");
            Sut = ((ResidentialPortalApp)await App.ClickOnElement(tabButton)).CurrentPageAs<UsagePage>();
        }

        [Test]
        public async Task CanSeeComponentItems()
        {
	        Assert.NotNull(Sut.UsageChartComponent);
	        Assert.NotNull(Sut.ComparisonModal);
		}
    }
}