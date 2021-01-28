using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Commands.BusinessPartner.ActivateAsEBiller;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.CompetitionEntry;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ContactUs;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Help;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.PromotionEntry;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Usage;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Tests
{
	[TestFixture]
	class WhenInAccountSelectionPageTests : WebAppPageTests<AccountSelectionPage>
    {
		protected AppUserConfigurator _userConfig;
		protected AccountSelectionPage _sut;

		protected override async Task TestScenarioArrangement()
		{
			_userConfig = App.ConfigureUser("a@A.com", "test");
			_userConfig.AddElectricityAccount();
			_userConfig.AddElectricityAccount(isEbiller:true);
            _userConfig.AddElectricityAccount(opened: false);
			_userConfig.AddElectricityAccount(opened: false);
			_userConfig.AddElectricityAccount(opened: false);
			_userConfig.AddElectricityAccount(opened: false);
			_userConfig.AddElectricityAccount(opened: false);
			_userConfig.AddElectricityAccount(canEstimateLatestBill: true);
			_userConfig.Execute();
			await App.WithValidSessionFor(_userConfig.UserName, _userConfig.Role);
            _sut = App.CurrentPageAs<AccountSelectionPage>();
		}

		[Test]
		public void CanSeeUserAccounts()
		{
			CollectionAssert.AreEquivalent(_userConfig.Accounts.Where(x => x.IsOpen)
                .Select(x => x.AccountNumber), _sut.ReadAccountNumbers());
		}       

        [Test]
        public async Task WhenSelectingAccount_ThenShowsUsagePage()
        {
	        var accountInfo = _userConfig.Accounts.First();
	        (await _sut.SelectAccount(accountInfo.AccountNumber)).CurrentPageAs<UsagePage>();
        }

		[Test]
		public async Task CanLogout_NavigatesToLoginPage()
		{
			var loginPage = (await _sut.Logout()).CurrentPageAs<LoginPage>();
			Assert.NotNull(loginPage);
		}
        [Test]
        public async Task MyAccountTabClick()
        {
            var myAccountPage = (await _sut.MyAccountTabClick()).CurrentPageAs<AccountSelectionPage>();
            Assert.NotNull(myAccountPage);
        }
        [Test]
        public async Task CompetitionBannerAndSwitchToCompetitionEntryPage()
        {
            Assert.NotNull(_sut.Banners);
            Assert.NotNull(_sut.CompetitionBannerClose);
            Assert.NotNull(_sut.CompetitionEntry);
            var competitionEntryPage = (await _sut.ClickOnElement(_sut.CompetitionEntry)).CurrentPageAs<CompetitionEntryPage>();
            Assert.NotNull(competitionEntryPage);
        }
        [Test]
        public async Task PromotionBannerAndSwitchToPromotionEntryPage()
        {
            Assert.NotNull(_sut.Banners);
            Assert.NotNull(_sut.PromotionBannerClose);
            Assert.NotNull(_sut.PromotionEntry);
            var promotionEntryPage = (await _sut.ClickOnElement(_sut.PromotionEntry)).CurrentPageAs<PromotionEntryPage>();
            Assert.NotNull(promotionEntryPage);
        }
        [Test]
        public async Task CanSwitchToClosedAccount()
        {
            (await _sut.ClickOnElement(_sut.ClosedAccountLink)).CurrentPageAs<AccountSelectionPage>();
        }
        [Test]
        public async Task CanSwitchToOpenAccount()
        {
            _sut = (await _sut.ClickOnElement(_sut.ClosedAccountLink)).CurrentPageAs<AccountSelectionPage>();
            (await _sut.ClickOnElement(_sut.OpenAccountLink)).CurrentPageAs<AccountSelectionPage>();
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
		[Test]
		public void EBillerFlagForUserContractsChecked()
		{
			foreach (var userConfigurator in _userConfig.ElectricityAndGasAccountConfigurators.Where(c=> !c.Model.BusinessAgreement.IsEBiller))
			{
				var activateAsEBillerCommand = new ActivateBusinessAgreementAsEBillerCommand(userConfigurator.Model.BusinessAgreement.BusinessAgreementId);
				App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(activateAsEBillerCommand);
			}

			App.DomainFacade.CommandDispatcher.Current.VerifyNoOtherCalls();
		}

		[Test, Ignore("TODO")]
		public void CanSwitchElectricityAndGas()
		{
			Assert.Fail("test all workflows");
		}

		[Test, Ignore("TODO")]
		public void CanPageMultipleResults()
		{
			Assert.Fail("test all workflows");
		}

		[Test, Ignore("TODO")]
		public void CanStart_ChangePassword()
		{
			Assert.Fail("test all workflows");
		}


	}
}