using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainModels.Membership;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Commands.Users.Membership.ChangePassword;
using EI.RP.DomainServices.Queries.Membership.ForgotPasswordRequestResults;
using EI.RP.TestServices.SpecimenGeneration;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.ForgotPassword.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.ForgotPassword.Tests
{
    [TestFixture]
    class WhenInForgotPasswordChangePageTest : WebAppPageTests<ForgotPasswordChangePage>
    {
        protected override async Task TestScenarioArrangement()
        {
        }

        [Test]
        public async Task Can_CreateNewPassword()
        {
            var fixture = new Fixture().CustomizeFrameworkBuilders();

            var requestId = fixture.Create<string>();
            var activationKey = fixture.Create<string>();

			var email = "forgotPass@test.test";
            var temporalPassword = fixture.Create<string>();
            var newPassword = "forgotPassword1234";

			SetUpForgotPasswordMock(requestId, true, email, temporalPassword);

            var forgotPasswordPage = (await App.ActivateForgotPassword(requestId, activationKey)).CurrentPageAs<ForgotPasswordChangePage>();
            forgotPasswordPage.NewPasswordInput.Value = newPassword;

            var changePasswordCommand = new ChangePasswordCommand(
                email, temporalPassword, newPassword, activationKey, null, requestId);

            App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(changePasswordCommand);
            (await forgotPasswordPage.ClickOnContinueButton()).CurrentPageAs<LoginPage>();
            App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(changePasswordCommand);
        }

        [Test]
        public async Task WhenPasswordIsNotValid_ShowsError()
        {
	        var fixture = new Fixture().CustomizeFrameworkBuilders();

	        var requestId = fixture.Create<string>();
	        var activationKey = fixture.Create<string>();

	        var email = "forgotPass@test.test";
	        var temporalPassword = fixture.Create<string>();

	        var newPassword = "1234";
			SetUpForgotPasswordMock(requestId, true, email, temporalPassword);
			var forgotPasswordPage = (await App.ActivateForgotPassword(requestId, activationKey)).CurrentPageAs<ForgotPasswordChangePage>();
			forgotPasswordPage.NewPasswordInput.Value = newPassword;
			var changePasswordCommand = new ChangePasswordCommand(
				email, temporalPassword, newPassword, activationKey, null, requestId);
			await forgotPasswordPage.ClickOnContinueButton();
	        App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(changePasswordCommand, new DomainException(DomainError.GeneralValidation));
		}

        [Test]
        public async Task WhenPasswordIsEmpty_ShowsError()
        {
	        var fixture = new Fixture().CustomizeFrameworkBuilders();

	        var requestId = fixture.Create<string>();
	        var activationKey = fixture.Create<string>();

	        var email = "forgotPass@test.test";
	        var temporalPassword = fixture.Create<string>();

	        var newPassword = "";
	        SetUpForgotPasswordMock(requestId, true, email, temporalPassword);
	        var forgotPasswordPage = (await App.ActivateForgotPassword(requestId, activationKey)).CurrentPageAs<ForgotPasswordChangePage>();
	        forgotPasswordPage.NewPasswordInput.Value = newPassword;
	        var changePasswordCommand = new ChangePasswordCommand(
		        email, temporalPassword, newPassword, activationKey, null, requestId);
	        await forgotPasswordPage.ClickOnContinueButton();
	        App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(changePasswordCommand, new DomainException(DomainError.GeneralValidation));
        }

		[Test]
        public async Task When_UserRequestIsInvalid_RedirectToLinkExpired()
        {
            var fixture = new Fixture().CustomizeFrameworkBuilders();

            var requestId = fixture.Create<string>();
            var activationKey = fixture.Create<string>();

            var email = fixture.Create<EmailAddress>();
            var temporalPassword = fixture.Create<string>();
            var newPassword = fixture.Create<string>();

            SetUpForgotPasswordMock(requestId, false, email, temporalPassword);

            (await App.ActivateForgotPassword(requestId, activationKey)).CurrentPageAs<ForgotPasswordLinkExpiredPage>();
        }

        [Test]
        public async Task When_UserRequestIsValid_But_UserNavigateToLoginPage()
        {
            var fixture = new Fixture().CustomizeFrameworkBuilders();

            var requestId = fixture.Create<string>();
            var activationKey = fixture.Create<string>();

            var email = fixture.Create<EmailAddress>();
            var temporalPassword = fixture.Create<string>();
            var newPassword = fixture.Create<string>();

            SetUpForgotPasswordMock(requestId, true, email, temporalPassword);
            
            (await App.ActivateForgotPassword(requestId, activationKey)).CurrentPageAs<ForgotPasswordChangePage>();

            (await App.ToLoginPage("")).CurrentPageAs<LoginPage>();
        }

        private void SetUpForgotPasswordMock(string requestId, bool isValid, string email, string temporalPassword)
        {
            App.DomainFacade
                .QueryResolver
                .ExpectQuery(new ForgotPasswordRequestResultQuery
                {
                    RequestId = requestId
                }, new ForgotPasswordRequestResult()
                {
                    IsValid = isValid,
                    Email = email,
                    TemporalPassword = temporalPassword
                }.ToOneItemArray());
        }   
    }
}
