using System;
using EI.RP.DomainServices.Commands.Users.Membership.ResetPassword;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.ForgotPassword.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using NUnit.Framework;
using System.Threading.Tasks;
using EI.RP.CoreServices.ErrorHandling;
using Moq;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.ForgotPassword.Tests
{
    [TestFixture]
    class WhenInRequestForgotPasswordPageTest : WebAppPageTests<RequestForgotPasswordPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            var loginPage = (await App.ToLoginPage("")).CurrentPageAs<LoginPage>();
            Sut = (await loginPage.ClickOnForgotPasswordLink()).CurrentPageAs<RequestForgotPasswordPage>();
        }

        [Test]
        public async Task CanRequestResetPassword()
        {
            Sut.EmailInput.Value = "forgotPass@test.test";

            var expectedCommand = new ResetPasswordDomainCommand(Sut.EmailInput.Value);

            var page = (await Sut.ClickOnResetMyPasswordButton()).CurrentPageAs<RequestForgotPasswordLinkSentPage>();
			Assert.IsNotNull(page.Heading);
			Assert.IsNotNull(page.Message);

            App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(expectedCommand);

        }

        [Test]
        public async Task ValidationErrorIsCorrectWhenDomainCommandErrors()
        {
	        Sut.EmailInput.Value = "forgotPass@test.test"; ;
	        App.DomainFacade.CommandDispatcher.Current
		        .Setup(x => x.ExecuteAsync<ResetPasswordDomainCommand>(It.IsAny<ResetPasswordDomainCommand>(),
			        It.IsAny<bool>())).Throws(new AggregateException(new DomainException(DomainError.Undefined)));
			
	        var page = (await Sut.ClickOnResetMyPasswordButton()).CurrentPageAs<RequestForgotPasswordPage>();
			Assert.IsTrue(page.ValidationErrors.InnerHtml.Contains("Sorry, an error occurred while processing your request."));
        }
    }
}