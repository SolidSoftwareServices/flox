using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Tests
{
    [TestFixture]
    class InternalLoginPageTests : WebAppPageTests<LoginPage>
    {
	    public InternalLoginPageTests() : base(ResidentialPortalDeploymentType.Internal)
	    {
	    }

	    private AppUserConfigurator _userConfig;

	    protected override async Task TestScenarioArrangement()
        {
	        var userName = "aUserName";
	        var userPassword = "test";

	        _userConfig = App.ConfigureUser(userName, userPassword,ResidentialPortalUserRole.AgentUser).Execute();

			Sut = (await App.ToLoginPage("")).CurrentPageAs<LoginPage>();
        }

        [Test]
        public async Task CanLogin()
        {
            Sut.UserNameElement.Value = _userConfig.UserName;
            Sut.PasswordElement.Value = _userConfig.Password;
            await Sut.ClickOnLoginButton();
            App.CurrentPageAs<AgentBusinessPartnerPage>();
        }


        [Test]
        public async Task CanLoginOnlineUser()
        {
	        Assert.IsNull(Sut.EmailElement);

	        var onlineUser = App.ConfigureUser("a@A.com", "asdfasdf");
	        onlineUser.AddElectricityAccount();
	        onlineUser.AddElectricityAccount();
	        onlineUser.Execute();
	        Sut.UserNameElement.Value = onlineUser.UserName;
	        Sut.PasswordElement.Value = onlineUser.Password;
	        Assert.IsFalse(Sut.LoginButton.IsDisabled);
        }

        [Test]
        public async Task WhenQueryStringIsWrongShowsDefaultLogin()
        {
            Sut = (await App.ToLoginPage("?asdasd")).CurrentPageAs<LoginPage>();
        }
    }
}