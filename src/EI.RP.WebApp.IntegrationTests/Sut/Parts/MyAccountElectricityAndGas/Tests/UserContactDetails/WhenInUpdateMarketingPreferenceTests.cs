using AutoFixture;
using EI.RP.DomainServices.Commands.Users.MarketingPreferences;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.UserContactDetails;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.UserContactDetails
{
	internal class WhenInUpdateMarketingPreferenceTests : MyAccountCommonTests<MarketingPreferencePage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount();
			UserConfig.Execute();
			var accountInfo = UserConfig.Accounts.First();

			var app = await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			Sut = (await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMarketingPreference(accountInfo.AccountNumber))
				.CurrentPageAs<MarketingPreferencePage>();
		}
		[Test]
		public async Task VisualItemsArePresented()
		{
			Assert.IsNotNull(Sut.SmsPreference);
			Assert.IsNotNull(Sut.MobilePreference);
			Assert.IsNotNull(Sut.LandLinePreference);
			Assert.IsNotNull(Sut.DoorToDoorPreference);
			Assert.IsNotNull(Sut.EmailPreference);
			Assert.IsNotNull(Sut.PostPreference);

			Assert.IsNotNull(Sut.SaveChangesButton);
			Assert.AreEqual("Save Changes", Sut.SaveChangesButton.TextContent);
		}

		[Test]
		public async Task CanUpdateMarketingPreferences()
		{
			var smsMarketingActive = Fixture.Create<bool>();
			var landLineMarketingActive = Fixture.Create<bool>();
			var mobileMarketingActive = Fixture.Create<bool>();
			var postMarketingActive = Fixture.Create<bool>();
			var doorToDoorMarketingActive = Fixture.Create<bool>();
			var emailMarketingActive = Fixture.Create<bool>();

			Sut.SmsPreference.IsChecked = smsMarketingActive;
			Sut.LandLinePreference.IsChecked = landLineMarketingActive;
			Sut.MobilePreference.IsChecked = mobileMarketingActive;
			Sut.PostPreference.IsChecked = postMarketingActive;
			Sut.DoorToDoorPreference.IsChecked = doorToDoorMarketingActive;
			Sut.EmailPreference.IsChecked = emailMarketingActive;

			var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;

			var updateUserContactCommand = new UpdateMarketingPreferencesCommand(accountInfo.AccountNumber,
				smsMarketingActive,
				landLineMarketingActive, mobileMarketingActive,
				postMarketingActive, doorToDoorMarketingActive,
				emailMarketingActive);
			App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(updateUserContactCommand);

			var page =
				(await Sut.ClickOnElement(Sut.SaveChangesButton))
				.CurrentPageAs<MarketingPreferencePage>();

			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(updateUserContactCommand);

			Assert.IsNotNull(page.SuccessMessage);
			Assert.AreEqual("Your Marketing Preferences have been successfully changed.", page.SuccessMessage.TextContent);
		}

		[Test]
		public async Task GoToMarketingPreferenceUsingQueryParam()
		{
			var page = (await App.ToUrl("?Marketing")).CurrentPageAs<MarketingPreferencePage>();
			Assert.IsNotNull(page.SmsPreference);
			Assert.IsNotNull(page.MobilePreference);
			Assert.IsNotNull(page.LandLinePreference);
			Assert.IsNotNull(page.DoorToDoorPreference);
			Assert.IsNotNull(page.EmailPreference);
			Assert.IsNotNull(page.PostPreference);

			Assert.IsNotNull(page.SaveChangesButton);
			Assert.AreEqual("Save Changes", page.SaveChangesButton.TextContent);
		}
	}
}
