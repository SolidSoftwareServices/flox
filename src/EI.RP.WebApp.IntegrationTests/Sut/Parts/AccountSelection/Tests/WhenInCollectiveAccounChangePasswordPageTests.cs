using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Commands.Users.Membership.ChangePassword;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ChangePassword;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Tests
{
	class WhenInCollectiveAccounChangePasswordPageTests : WebAppPageTests<CollectiveAccountErrorPage>
    {
	    private AppUserConfigurator _userConfig;
	    private ChangePasswordPage _sut;

	    protected override async Task TestScenarioArrangement()
	    {
		    _userConfig = App.ConfigureUser("a@A.com", "test");
		    await App.WithValidSessionFor(_userConfig.UserName, _userConfig.Role);
		    var errorPage = App.CurrentPageAs<CollectiveAccountErrorPage>();
		    _sut = (await errorPage.ClickOnElement(errorPage.ChangePasswordMenuItemLink)).CurrentPageAs<ChangePasswordPage>();
	    }

	    [Test]
	    public async Task CanChangePassword()
	    {
		    _sut.CurrentPassword.Value = _userConfig.Password;
		    _sut.NewPassword.Value = "Test3333";
		    var changePasswordCommand = new ChangePasswordCommand(
			    _userConfig.UserName,
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
