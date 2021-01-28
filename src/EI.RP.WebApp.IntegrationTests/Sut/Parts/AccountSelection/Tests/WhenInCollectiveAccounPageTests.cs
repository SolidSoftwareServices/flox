using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Help;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Tests
{
	class WhenInCollectiveAccounPageTests : WebAppPageTests<CollectiveAccountErrorPage>
    {
	    private AppUserConfigurator _userConfig;
	    private CollectiveAccountErrorPage _sut;

	    protected override async Task TestScenarioArrangement()
	    {
		    _userConfig = App.ConfigureUser("a@A.com", "test");
		    await App.WithValidSessionFor(_userConfig.UserName, _userConfig.Role);
		    _sut = App.CurrentPageAs<CollectiveAccountErrorPage>();
	    }

	    [Test]
	    public void CanSeeErrorPage()
	    {
		    Assert.IsNotNull(_sut.ErrorHeading);
		    Assert.IsNotNull(_sut.ErrorText);
		    Assert.IsNotNull(_sut.ChangePasswordMenuItemLink);
		    Assert.IsNotNull(_sut.LogoutMenuItemLink);
	    }

	    [Test]
	    public async Task CanNavigatesToHelp()
	    {
		    (await _sut.ToHelpViaFooter()).CurrentPageAs<HelpPage>();
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
    }
}
