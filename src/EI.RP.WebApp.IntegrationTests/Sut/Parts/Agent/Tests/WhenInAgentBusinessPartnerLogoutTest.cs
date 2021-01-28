using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Commands.Users.Membership.ChangePassword;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ChangePassword;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Tests
{
    [TestFixture]
    class WhenInAgentBusinessPartnerLogoutTest : WebAppPageTests<AgentBusinessPartnerPage>
    {
        public WhenInAgentBusinessPartnerLogoutTest() : base(ResidentialPortalDeploymentType.Internal)
        {

        }

        private AppUserConfigurator _esbAgentUser;

        protected override async Task TestScenarioArrangement()
        {
            _esbAgentUser = App.ConfigureUser("testagent@esb.ie", "Password$1", ResidentialPortalUserRole.AgentUser);

            var app = (ResidentialPortalApp)await App.WithValidSessionFor(_esbAgentUser.UserName, _esbAgentUser.Role);
            Sut = (await app.ToAgentBusinessPartnerSearch()).CurrentPageAs<AgentBusinessPartnerPage>();
            
        }

        [Test]
        public async Task Logout()
        {
            Assert.IsNotNull(Sut.LogoutMenuItemLink);
            var page = (await App.ClickOnElement(Sut.LogoutMenuItemLink)).CurrentPageAs<LoginPage>();
            Assert.IsNotNull(page.LoginPageHeader);
            Assert.IsNotNull(page.UserNameElement);
            Assert.IsNotNull(page.PasswordElement);
            Assert.IsNotNull(page.LoginButton);
        }
    }
}
