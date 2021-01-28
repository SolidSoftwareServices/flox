using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Help;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using NUnit.Framework;
using System.Threading.Tasks;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ContactUs;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Tests
{
    [TestFixture]
    class WhenInAgentBusinessPartnerFooterTests : WebAppPageTests<AgentBusinessPartnerPage>
    {
        public WhenInAgentBusinessPartnerFooterTests() : base(ResidentialPortalDeploymentType.Internal)
        {
        }

        private AgentBusinessPartnerPage _sut;
        private AppUserConfigurator _esbAgentUser;
        private AppUserConfigurator _esbBusinessPartner;

        protected override async Task TestScenarioArrangement()
        {
            _esbAgentUser = App.ConfigureUser("testagent@esb.ie", "Password$1", ResidentialPortalUserRole.AgentUser);
            _esbBusinessPartner = App.ConfigureUser("testBusinessPartner@esb.ie", "Password$1", ResidentialPortalUserRole.OnlineUser);
            _esbBusinessPartner.AddElectricityAccount();
            _esbBusinessPartner.Execute();
            
            App.DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
            {
                Opened = null
            },  _esbBusinessPartner.Accounts);

            var app = (ResidentialPortalApp)await App.WithValidSessionFor(_esbAgentUser.UserName, _esbAgentUser.Role);

            await app.ToAgentBusinessPartnerSearch();
            _sut = App.CurrentPageAs<AgentBusinessPartnerPage>();
        }

        [Test]
        public async Task CanNavigatesToHelp()
        {
            (await _sut.ToHelpViaFooter()).CurrentPageAs<HelpPage>();
        }

        [Test]
        public async Task CanNavigatesToContactUs()
        {
            (await _sut.ToContactUsViaFooter()).CurrentPageAs<ContactUsPage>();
        }

        [Test]
        public async Task CanNavigatesToDisclaimer()
        {
            (await _sut.ToDisclaimerViaFooter()).CurrentPageAs<TermsInfoPage>();
        }

        [Test]
        public async Task CanNavigatesToPrivacy()
        {
            (await _sut.ToPrivacyNoticeViaFooter()).CurrentPageAs<TermsInfoPage>();
        }

        [Test]
        public async Task CanNavigatesToTermsAndConditions()
        {
            (await _sut.ToTermsAndConditionsViaFooter()).CurrentPageAs<TermsInfoPage>();
        }
    }
}
