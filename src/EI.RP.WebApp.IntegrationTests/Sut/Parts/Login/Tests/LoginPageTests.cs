using System.Collections;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Tests
{
	[TestFixture]
	internal class LoginPageTests : WebAppPageTests<LoginPage>
	{
		private AppUserConfigurator _userConfig;

		protected override async Task TestScenarioArrangement()
		{
			var userName = "a@A.com";
			var userPassword = "test";

			_userConfig = App.ConfigureUser(userName, userPassword);
			_userConfig.AddElectricityAccount();
			_userConfig.AddElectricityAccount();
			_userConfig.Execute();


			Sut = (await App.ToLoginPage(string.Empty)).CurrentPageAs<LoginPage>();
		}

		[Test]
		public async Task CanLogin()
		{
			Sut.EmailElement.Value = _userConfig.UserName;
			Sut.PasswordElement.Value = _userConfig.Password;
			await Sut.ClickOnLoginButton();
			App.CurrentPageAs<AccountSelectionPage>();
		}

		[Test]
		public async Task CanLoginAdminUser()
		{
			Assert.IsNull(Sut.UserNameElement);
			var agentUserConfig = App.ConfigureUser("anAdmin", "apwd", ResidentialPortalUserRole.AgentUser).Execute();

			Sut.EmailElement.Value = agentUserConfig.UserName;
			Sut.PasswordElement.Value = agentUserConfig.Password;
			Assert.IsFalse(Sut.LoginButton.IsDisabled);
		}

		[Test]
		public async Task WhenQueryStringIsWrongShowsDefaultLogin()
		{
			Sut = (await App.ToLoginPage("?asdasd")).CurrentPageAs<LoginPage>();
		}

		[Test]
		public async Task CanLoginAndRedirectedToCorrectPage()
		{
			Sut = (await App.ToLoginPage("?ReturnUrl=/accounts/init")).CurrentPageAs<LoginPage>();
			Sut.EmailElement.Value = _userConfig.UserName;
			Sut.PasswordElement.Value = _userConfig.Password;
			await Sut.ClickOnLoginButton();
			App.CurrentPageAs<AccountSelectionPage>();
		}
	}
}