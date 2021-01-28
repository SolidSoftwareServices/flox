using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.DomainServices.Commands.Users.SmartActivationNotification.DismissSmartActivationNotification;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Tests
{
	[TestFixture]
	class WhenInAccountSelectionPage_SmartEligiblityTests : WhenInAccountSelectionPageTests
	{
		protected override async Task TestScenarioArrangement()
		{
			_userConfig = App.ConfigureUser("a@A.com", "test");
			_userConfig.AddElectricityAccount().WithElectricity24HrsDevices(RegisterConfigType.MCC01, CommsTechnicallyFeasibleValue.CTF3);
			_userConfig.AddElectricityAccount();
			_userConfig.AddElectricityAccount(opened: false);
			_userConfig.AddElectricityAccount().WithElectricity24HrsDevices(RegisterConfigType.MCC16, CommsTechnicallyFeasibleValue.CTF4);
			_userConfig.Execute();
			await App.WithValidSessionFor(_userConfig.UserName, _userConfig.Role);
			_sut = App.CurrentPageAs<AccountSelectionPage>();
		}

		[Test]
		public async Task SmartActivationNotificationShown()
		{
			Assert.NotNull(_sut.SmartActivationNotification);
			Assert.NotNull(_sut.SmartActivationNotificationClose);
			var electricityAccount = _userConfig.Accounts.First();
			
			Assert.AreEqual($"MPRN {electricityAccount.PointReferenceNumber}", _sut.SmartActivationNotificationMprn?.TextContent); 
		}

		[Test]
		public async Task DismissSmartActivationNotificationForSession()
		{
			Assert.NotNull(_sut.SmartActivationNotification);
			Assert.NotNull(_sut.SmartActivationNotificationClose);
			var electricityAccount = _userConfig.Accounts.First();
			Assert.AreEqual($"MPRN {electricityAccount.PointReferenceNumber}", _sut.SmartActivationNotificationMprn?.TextContent);

			electricityAccount.SwitchToSmartPlanDismissed = true;
			var smartActivationPage = (await App.ClickOnElement(_sut.SmartActivationNotificationClose))
				.CurrentPageAs<AccountSelectionPage>();

			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(new DismissSmartActivationNotificationCommand(electricityAccount.AccountNumber));

			
			electricityAccount = _userConfig.Accounts.First(x=>x.AccountNumber!=electricityAccount.AccountNumber &&
			                                                   x.SmartActivationStatus == SmartActivationStatus.SmartAndEligible);
			Assert.NotNull(smartActivationPage.SmartActivationNotification);
			Assert.AreEqual($"MPRN {electricityAccount.PointReferenceNumber}", smartActivationPage.SmartActivationNotificationMprn?.TextContent);
			electricityAccount.SwitchToSmartPlanDismissed = true;
			smartActivationPage = (await App.ClickOnElement(smartActivationPage.SmartActivationNotificationClose))
				.CurrentPageAs<AccountSelectionPage>();

			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(new DismissSmartActivationNotificationCommand(electricityAccount.AccountNumber));
			Assert.Null(smartActivationPage.SmartActivationNotification);
		}

		[Test]
		public async Task CanDismissSmartActivationNotification()
		{
			Assert.NotNull(_sut.SmartActivationNotification);
			Assert.NotNull(_sut.SmartActivationNotificationClose);
			var electricityAccount = _userConfig.Accounts.First();
			Assert.AreEqual($"MPRN {electricityAccount.PointReferenceNumber}", _sut.SmartActivationNotificationMprn?.TextContent);

			electricityAccount.SwitchToSmartPlanDismissed = true;
			var smartActivationPage = (await _sut.ClickOnElement(_sut.SmartActivationNotificationClose)).CurrentPageAs<AccountSelectionPage>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(new DismissSmartActivationNotificationCommand(electricityAccount.AccountNumber));
			electricityAccount = _userConfig.Accounts.First(x => x.AccountNumber != electricityAccount.AccountNumber &&
			                                                     x.SmartActivationStatus == SmartActivationStatus.SmartAndEligible);
			Assert.NotNull(smartActivationPage.SmartActivationNotification);
			Assert.AreEqual($"MPRN {electricityAccount.PointReferenceNumber}", smartActivationPage.SmartActivationNotificationMprn?.TextContent);
		}

		[Test]
		public async Task CanNavigateToSmartActivation()
		{
			Assert.NotNull(_sut.SmartActivationNotificationJourneyLink);
			var page = (await _sut.ClickOnElement(_sut.SmartActivationNotificationJourneyLink)).CurrentPageAs<Step1EnableSmartFeaturesPage>();
			(await page.ClickOnElement(page.CancelButton)).CurrentPageAs<AccountSelectionPage>();
		}
	}
}
