using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
    class SignInSteps : BaseStep
    {
        SignInSteps(SingleTestContext shared) : base(shared) 
        {
        }
		private SignInPage signInPage => new SignInPage(driver);

		internal void GivenNavigateToTheSignInPage()
        {
			shared.Driver.Value.Instance.Navigate().GoToUrl(TestSettings.Default.PublicTargetUrl);
			signInPage.AssertLoginPage();
        }
        
        internal void GivenNavigateToTheSignInPageFromSmartHubPage()
        {
            shared.Driver.Value.Instance.Navigate().GoToUrl(TestSettings.Default.SmartHubUrl());
        }
        internal void WhenEnterPassword(string password)
        {
            signInPage.EnterPassword(password);
        }
        
        internal void WhenClickForgotPassword()
        {
            signInPage.ClickForgotPassword();
        }
        
        internal void WhenClickPAYGButton()
        {
            signInPage.ClickPAYG();
        }
        
        internal void WhenClickCreateAccountLink()
        {
            signInPage.ClickCreateAccount();
        }
        
        internal void WhenClickLogin()
        {
            signInPage.ClickLogin();
        }
        
        internal void ThenShouldBeSentToMyAccounts()
        {
            MyAccountsPage myAccountsPage = new MyAccountsPage(shared.Driver.Value);
            myAccountsPage.AssertMyAccountsHeaderDisplayed();
        }
        
        internal void ThenLoginButtonShouldBeGreyedOut()
        {
            signInPage.ClickLogin();
            signInPage.AssertSignPageHeaderDisplayed();
        }
        
        internal void ThenErrorAsInvalidEmail()
        {
            signInPage.AssertErrorEmail();
        }
        
        internal void ThenShouldBeSentToForgotPasswordPage()
        {
            signInPage.AssertForgotPasswordPage();
        }
        
        internal void ThenShouldBeSentToPAYGPage()
        {
            signInPage.AssertPAYGPage();
        }
        internal void WhenEnterSignInEmail(string email)
        {
            signInPage.EnterSignInEmail(email);
        }
    }
}
