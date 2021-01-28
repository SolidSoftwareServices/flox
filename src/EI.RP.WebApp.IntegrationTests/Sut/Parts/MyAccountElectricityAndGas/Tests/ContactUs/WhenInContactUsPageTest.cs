using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ContactUs;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.ContactUs
{
    [TestFixture]
    class WhenInContactUsPageTest: MyAccountCommonTests<ContactUsPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount()
                .WithInvoices(3);
            UserConfig.Execute();

            var app = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToContactUs();
            Sut = app.CurrentPageAs<ContactUsPage>();
        }

        [Test]
        public async Task CanSeeComponentItems()
        {
	        Assert.NotNull(Sut.AccountDropDown);
	        Assert.NotNull(Sut.QueryTypeDropDown);
            Assert.NotNull(Sut.SubjectInput);
            Assert.NotNull(Sut.QueryInput);
            Assert.NotNull(Sut.SubmitQueryButton);
        }

        [Test]
        public async Task CanSeeAllowedMenuItemsInContactUsPage()
        {
            Assert.NotNull(Sut.ChangePasswordProfileMenuItem);
            Assert.NotNull(Sut.LogoutProfileMenuItem);            
        }

        [Test]
        public async Task CannotSeeForbiddenMenuItemsInContactUsPage()
        {
            Assert.Null(Sut.MyDetailsProfileMenuItem);
            Assert.Null(Sut.MarketingProfileMenuItem);
        }

        [Test]
        public async Task PrivacyLinkGoesToTermsInfoPage()
        {
            (await Sut.ToPrivacyNoticeViaComponent()).CurrentPageAs<TermsInfoPage>();
        }
    }
}