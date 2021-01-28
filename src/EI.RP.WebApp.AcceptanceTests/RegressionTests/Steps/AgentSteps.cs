using EI.RP.WebApp.RegressionTests.PageObjects;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
using System.Diagnostics;
using EI.RP.WebApp.AcceptanceTests;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.UITestAutomation
{
    public class AgentSteps : BaseStep
    {
        private RegressionTests.PageObjects.AgentLoginPage _agentLoginPage => new RegressionTests.PageObjects.AgentLoginPage(shared.Driver.Value);
        private RegressionTests.PageObjects.AgentSearchPage _agentSearchPage => new EI.RP.WebApp.RegressionTests.PageObjects.AgentSearchPage(shared.Driver.Value);
        private MyAccountsPage myAccountsPage => new MyAccountsPage(shared.Driver.Value);
        private NavigationBar _navigationBar => new NavigationBar(shared.Driver.Value);

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
            _agentLoginPage.AssertErrorUsernameEmpty();
        }

        public AgentSteps(SingleTestContext shared) : base(shared)
        {
        }


        private bool OnAgentLoginScreen()
        {
            return _agentLoginPage.OnPage();
        }

        private bool OnAgentSearchScreen()
        {
            return _agentSearchPage.OnPage();
        }

        internal void ThenErrorPasswordEmpty()
        {
            _agentLoginPage.AssertErrorPasswordEmpty();
        }

        public void WhenLoginToTheAgentPortal(IDictionary dict)
        {
            _agentLoginPage.AgentSignIn(dict);
            _agentSearchPage.AssertAgentSearchPage();
        }

        internal void ThenErrorIncorrectCredentials()
        {
            _agentLoginPage.AssertErrorPasswordIncorrect();
        }

        public void WhenEnterUserNameInUsernameField(string username)
        {
            _agentSearchPage.AgentSearchEnterEmailAdress(username);
        }

        public void WhenClickOnFindBusinessPartnerBtn()
        {
            _agentSearchPage.ClickFindBusinessPartnerBtn();
        }

        public void WhenClickOnDeRegistrationBtn()
        {
            _agentSearchPage.ClickDeRegistrationBtn();
            _agentSearchPage.AssertDeRegistrationPopUp();
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
            _agentSearchPage.EnterBP(v);
        }

        public void ThenAccountShouldOpen()
        {

            myAccountsPage.AssertMyAccountsHeaderDisplayed();

        }

        internal void WhenEnterStreet(string v)
        {
            _agentSearchPage.EnterStreet(v);
        }

        internal void WhenEnterHouse(string v)
        {
            _agentSearchPage.EnterHouse(v);
        }

        internal void GivenOnTheAgentSearchScreen(IDictionary dict)
        {
            shared.Driver.Value.Instance.Navigate().GoToUrl(TestSettings.Default.InternalTargetUrl);
           if (OnAgentLoginScreen())
           {
                Debug.Print("On AGent loin");
                _agentLoginPage.AssertAgentLoginPage();
                WhenLoginToTheAgentPortal(dict);
            }
            else if (!OnAgentSearchScreen())
            {
            }
        }

        internal void WhenEnterCity(string v)
        {
            _agentSearchPage.EnterCity(v);
        }

        internal void ThenErrorEmpty()
        {
            _agentSearchPage.AssertErrorEmpty();
        }

        internal void WhenClickFirstAccount()
        {
            myAccountsPage.ClickViewFullAccountDetailsOnFirstAccount();
        }

        internal void ThenNoMoveHomeTab()
        {
            _navigationBar.AssertNoMoveHouseTab();
        }
    }
}
