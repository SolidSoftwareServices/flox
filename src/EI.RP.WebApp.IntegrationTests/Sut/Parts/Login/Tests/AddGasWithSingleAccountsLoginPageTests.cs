using System.Threading.Tasks;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Tests
{
	[TestFixture]
	class AddGasWithSingleAccountsLoginPageTests : WebAppPageTests<LoginPage>
	{
		private AppUserConfigurator _userConfig;

		protected override async Task TestScenarioArrangement()
		{
			await AUserIsInTheLoginPage();

			async Task AUserIsInTheLoginPage()
			{
				var userName = "a@A.com";
				var userPassword = "test";

				_userConfig = App.ConfigureUser(userName, userPassword);
				_userConfig.AddElectricityAccount();
				_userConfig.Execute();

				Sut = (await App.ToLoginPage("?addgas")).CurrentPageAs <LoginPage>();
			}
		}
		[Test]
        public async Task TheViewShowsTheExpectedInformation()
        {
            Assert.AreEqual("Login to Add Gas", Sut.LoginPageHeader.TextContent);
            Sut.EmailElement.Value = _userConfig.UserName;
            Sut.PasswordElement.Value = _userConfig.Password;
            (await Sut.ClickOnLoginButton()).CurrentPageAs<AccountSelectionPage>();

        }
    }
}