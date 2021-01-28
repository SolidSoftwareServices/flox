using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Commands.Users.MarketingPreferences;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.UserContactDetails;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Tests
{
	internal class MarketingPreferencesWithMultipleAccountsLoginPageTests : WebAppPageTests<LoginPage>
    {
	    private const string MarketingQueryParam = "?marketing";
	    private AppUserConfigurator _userConfig;
	    protected static readonly IFixture Fixture = new Fixture().CustomizeDomainTypeBuilders();

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
			    _userConfig.AddGasAccount();
			    _userConfig.Execute();

			    Sut = (await App.ToLoginPage(MarketingQueryParam)).CurrentPageAs<LoginPage>();
		    }
	    }

	    [Test]
	    public async Task TheViewShowsTheExpectedInformation()
	    {
			Assert.AreEqual("Account Login", Sut.LoginPageHeader.TextContent);
			Sut.EmailElement.Value = _userConfig.UserName;
		    Sut.PasswordElement.Value = _userConfig.Password;
		    var page = (await Sut.ClickOnLoginButton()).CurrentPageAs<MarketingPreferencePage>();
			Assert.IsNotNull(page.SmsPreference);
			Assert.IsNotNull(page.MobilePreference);
			Assert.IsNotNull(page.LandLinePreference);
			Assert.IsNotNull(page.DoorToDoorPreference);
			Assert.IsNotNull(page.EmailPreference);
			Assert.IsNotNull(page.PostPreference);

			Assert.IsNotNull(page.SaveChangesButton);
			Assert.AreEqual("Save Changes", page.SaveChangesButton.TextContent);
	    }

	    [Test]
	    public async Task MarketingPageWorkingAsExpected()
	    {
			Assert.AreEqual("Account Login", Sut.LoginPageHeader.TextContent);
			Sut.EmailElement.Value = _userConfig.UserName;
		    Sut.PasswordElement.Value = _userConfig.Password;
		    var marketingPage = (await Sut.ClickOnLoginButton()).CurrentPageAs<MarketingPreferencePage>();

		    var smsMarketingActive = Fixture.Create<bool>();
		    var landLineMarketingActive = Fixture.Create<bool>();
		    var mobileMarketingActive = Fixture.Create<bool>();
		    var postMarketingActive = Fixture.Create<bool>();
		    var doorToDoorMarketingActive = Fixture.Create<bool>();
		    var emailMarketingActive = Fixture.Create<bool>();

		    marketingPage.SmsPreference.IsChecked = smsMarketingActive;
		    marketingPage.LandLinePreference.IsChecked = landLineMarketingActive;
		    marketingPage.MobilePreference.IsChecked = mobileMarketingActive;
		    marketingPage.PostPreference.IsChecked = postMarketingActive;
		    marketingPage.DoorToDoorPreference.IsChecked = doorToDoorMarketingActive;
			marketingPage.EmailPreference.IsChecked = emailMarketingActive;

		    var accountInfo = _userConfig.ElectricityAndGasAccountConfigurators.First().Model;

		    var updateUserContactCommand = new UpdateMarketingPreferencesCommand(accountInfo.AccountNumber,
			    smsMarketingActive,
			    landLineMarketingActive, mobileMarketingActive,
			    postMarketingActive, doorToDoorMarketingActive,
			    emailMarketingActive);
		    App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(updateUserContactCommand);

		    var page =
			    (await marketingPage.ClickOnElement(marketingPage.SaveChangesButton))
			    .CurrentPageAs<MarketingPreferencePage>();

		    App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(updateUserContactCommand);

		    Assert.IsNotNull(page.SuccessMessage);
		    Assert.AreEqual("Your Marketing Preferences have been successfully changed.", page.SuccessMessage.TextContent);
		}


	    [Test]
	    public async Task CanGoToMarketingPreferenceLogin()
	    {
		    Sut = (await App.ToLoginPage(MarketingQueryParam)).CurrentPageAs<LoginPage>();
	    }
    }
}