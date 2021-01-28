using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
    public class AgentLoginSteps : BaseStep
    {
        private AgentLoginPage _agentLoginPage => new AgentLoginPage(shared.Driver.Value);
        private AgentSearchPage _agentSearchPage => new AgentSearchPage(shared.Driver.Value);
		private MyAccountsPage myAccountsPage => new MyAccountsPage(shared.Driver.Value);

        private string latestBillHeaderID = "latestBillHeader";

        internal void WhenEnterLoginUsername(string v)
        {
            _agentLoginPage.EnterUsername(v);
        }

        internal void WhenClickLogin()
        {
            _agentLoginPage.ClickLogin();
        }

        internal void WhenEnterLoginPassword(string v)
        {
            _agentLoginPage.EnterPassword(v);
        }

        internal void ThenErrorUsernameEmpty()
        {
            _agentLoginPage.ErrorUsernameEmpty();
        }

        public AgentLoginSteps(SingleTestContext shared) : base(shared)
        {
        }
        
        internal void ThenErrorPasswordEmpty()
        {
            throw new NotImplementedException();
        }

        public void WhenLoginToTheAgentPortal(IDictionary dict)
        {
            _agentLoginPage.AgentSignIn(dict);
            _agentSearchPage.AssertAgentSearchPage();
        }

        internal void ThenErrorPasswordIncorrect()
        {
            throw new NotImplementedException();
        }

        public void WhenEnterUserNameInUsernameField(string username)
        {
            _agentSearchPage.AgentSearchEnterEmailAdress(username);
        }
        
        public void WhenClickOnFindBusinessPartnerBtn()
        {
            _agentSearchPage.ClickFindBusinessPartnerBtn();
            _agentSearchPage.AssertBusinessPartnerTable();
        }
        
        public void WhenClickOnDeRegistrationBtn()
        {
            _agentSearchPage.ClickDeRegistrationBtn();
            _agentSearchPage.AssertDeRegistrationPopUp();
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
        
        public void WhenClickOnCancelBtnOnDeRegistrationPopup()
        {
            _agentSearchPage.ClickDeRegistrationPopUpCancelBtn();
        }
        
        public void WhenClickOnViewBusinessPartnerBtn()
        {
            _agentSearchPage.ClickViewBusinessPartnerBtn();
        }

        internal void WhenEnterBP(string v)
        {
            throw new NotImplementedException();
        }

        public void ThenAccountShouldOpen()
        {

			myAccountsPage.AssertMyAccountsHeaderDisplayed();

        }

        internal void WhenEnterStreet(string v)
        {
            throw new NotImplementedException();
        }

        internal void WhenEnterHouse(string v)
        {
            throw new NotImplementedException();
        }

        internal void WhenEnterCity(string v)
        {
            throw new NotImplementedException();
        }

        internal void ThenErrorEmpty()
        {
            throw new NotImplementedException();
        }
    }
}
