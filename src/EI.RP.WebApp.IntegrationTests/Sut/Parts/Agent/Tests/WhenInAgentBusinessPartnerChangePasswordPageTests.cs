using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Commands.Users.Membership.ChangePassword;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ChangePassword;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Tests
{
    [TestFixture]
    class WhenInAgentBusinessPartnerChangePasswordPageTests : WebAppPageTests<AgentBusinessPartnerPage>
    {
        public WhenInAgentBusinessPartnerChangePasswordPageTests() : base(ResidentialPortalDeploymentType.Internal)
        {
            
        }

        private ChangePasswordPage _sut;
        private AppUserConfigurator _esbAgentUser;

        protected override async Task TestScenarioArrangement()
        {
            _esbAgentUser = App.ConfigureUser("testagent@esb.ie", "Password$1", ResidentialPortalUserRole.AgentUser);

            var app = (ResidentialPortalApp) await App.WithValidSessionFor(_esbAgentUser.UserName, _esbAgentUser.Role);

            var agentBusinessSearchPage = (await app.ToAgentBusinessPartnerSearch()).CurrentPageAs<AgentBusinessPartnerPage>();

            _sut = (await agentBusinessSearchPage.ClickOnElement(agentBusinessSearchPage.ChangePasswordMenuItemLink)).CurrentPageAs<ChangePasswordPage>();
        }

        [Test]
        public async Task CanChangePassword()
        {
            _sut.CurrentPassword.Value = _esbAgentUser.Password;
            _sut.NewPassword.Value = "Test3333";
           var changePasswordCommand = new ChangePasswordCommand(
               _esbAgentUser.UserName, 
               _sut.CurrentPassword.Value, 
               _sut.NewPassword.Value, 
               null,
			   null,
               null);

           App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(changePasswordCommand);

           _sut = (await _sut.ClickOnElement(_sut.SaveNewPasswordBtn)).CurrentPageAs<ChangePasswordPage>();

           App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(changePasswordCommand);
        }
    }
}