using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainErrors;
using EI.RP.DomainServices.Commands.Users.Membership.ChangePassword;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ChangePassword;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using Moq;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.ChangePassword
{
    [TestFixture]
    internal class WhenInChangePasswordTests : MyAccountCommonTests<ChangePasswordPage>
    {

        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount();
            UserConfig.Execute();

            await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
            await App.CurrentPageAs<AccountSelectionPage>().ToChangePassword();
            Sut = App.CurrentPageAs<ChangePasswordPage>();
        }

        [Test]
        public async Task CanChangePassword()
        {
            Sut.CurrentPassword.Value = UserConfig.Password;
            Sut.NewPassword.Value = "Test3333";
            var changePasswordCommand =
                new ChangePasswordCommand("a@A.com", Sut.CurrentPassword.Value, Sut.NewPassword.Value, null, null, null);
            App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(changePasswordCommand);
            Sut = (await Sut.ClickOnElement(Sut.SaveNewPasswordBtn)).CurrentPageAs<ChangePasswordPage>();
            
            App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(changePasswordCommand);
        }


        [Test]
        public async Task WhenWrongCurrentPasswordEnteredShowsError()
        {
	        Sut.CurrentPassword.Value = UserConfig.Password;
	        Sut.NewPassword.Value = "Test3333";

	        App.DomainFacade.CommandDispatcher.Current.Setup(x =>
					x.ExecuteAsync(It.IsAny<ChangePasswordCommand>(), It.IsAny<bool>()))
				.Throws(new DomainException(ResidentialDomainError.IncorrectPasswordError));
			
	        Sut = (await Sut.ClickOnElement(Sut.SaveNewPasswordBtn)).CurrentPageAs<ChangePasswordPage>();
			Assert.IsTrue(Sut.ErrorMessage.TextContent.Contains(ResidentialDomainError.IncorrectPasswordError.ErrorMessage));
        }

		[Test]
        [Ignore("TODO")]
        public void WhenCommandFails_ShowsError()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void WhenPasswordIsNotPolicyCompliant_ShowsError()
        {
            throw new NotImplementedException();
        }
    }
}