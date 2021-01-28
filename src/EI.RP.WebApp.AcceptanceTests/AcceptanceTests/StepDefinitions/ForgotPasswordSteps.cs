using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
    class ForgotPasswordSteps : BaseStep
    {
        ForgotPasswordSteps(SingleTestContext shared) : base(shared) { }
		private PasswordResetPage passwordResetPage => new PasswordResetPage(shared.Driver.Value);
		private SignInPage signInPage => new SignInPage(shared.Driver.Value);

		internal void GivenNavigateToThePasswordResetPage()
        {
            signInPage.ClickForgotPassword();
        }
        
        internal void ThenResetMyPasswordButtonShouldBeGreyedOut()
        {
            passwordResetPage.AssertPasswordResetBtnGreyedOut();
            passwordResetPage.AssertPasswordResetPage();
        }
        
        public void ThenErrorAsInvalidEmailAddress()
        {
            passwordResetPage.AssertErrorInvalidEmailAddress();
        }
        internal void WhenEnterResetEmailAsJoe_BloggsTest_Com(string email)
        {
            passwordResetPage.EnterEmail(email);
        }
        
        internal void WhenClickResetPassword()
        {
            passwordResetPage.ClickResetPassword();
        }
    }
}
