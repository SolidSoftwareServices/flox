using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Tests
{
	[TestFixture]
	class MeterReadingWithMultipleAccountsLoginPageTests : WebAppPageTests<LoginPage>
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
				_userConfig.AddElectricityAccount();
				_userConfig.Execute();

				Sut = (await App.ToLoginPage("?meterread")).CurrentPageAs <LoginPage>();
			}
		}
		[Test]
        public async Task TheViewShowsTheExpectedInformation()
        {
            Assert.AreEqual("Meter Reading Login", Sut.LoginPageHeader.TextContent);
            Sut.EmailElement.Value = _userConfig.UserName;
            Sut.PasswordElement.Value = _userConfig.Password;
            var actual = (await Sut.ClickOnLoginButton()).CurrentPageAs<SubmitMeterReadingsPage>();
        }
        [Test]
        public async Task CanGoToMeterReadingLogin()
        {
            Sut = (await App.ToLoginPage("?meterReading")).CurrentPageAs<LoginPage>();

        }
    }
}