using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ContactUs;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Help;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.TermsInfo
{
    [TestFixture]
    class WhenInDisclaimerPageTest : MyAccountCommonTests<TermsInfoPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig
                .AddElectricityAccount()
                .WithInvoices(3);
            UserConfig.Execute();
            
            var app = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToDisclaimer();
            Sut = app.CurrentPageAs<TermsInfoPage>();
        }

        [Test]
        public async Task CanSeeComponentItems()
        {
            Assert.IsTrue(Sut.IsDisclaimer());
        }

        [Test]
        public async Task CanNavigateToHelp()
        {
            (await Sut.ToHelpViaFooter()).CurrentPageAs<HelpPage>();
        }

        [Test]
        public async Task CanNavigateToContactUs()
        {
            (await Sut.ToContactUsViaFooter()).CurrentPageAs<ContactUsPage>();
        }

        [Test]
        public async Task CanNavigateToTermsAndConditions()
        {
            Sut = (await Sut.ToTermsAndConditionsViaFooter()).CurrentPageAs<TermsInfoPage>();
            Assert.IsTrue(Sut.IsTermsAndConditions());
        }

        [Test]
        public async Task CanNavigateToDisclaimer()
        {
            Sut = (await Sut.ToDisclaimerViaFooter()).CurrentPageAs<TermsInfoPage>();
            Assert.IsTrue(Sut.IsDisclaimer());
        }

        [Test]
        public async Task CanNavigateToPrivacyNotice()
        {
            Sut = (await Sut.ToPrivacyNoticeViaFooter()).CurrentPageAs<TermsInfoPage>();
            Assert.IsTrue(Sut.IsPrivacyNotice());
        }
    }
}
