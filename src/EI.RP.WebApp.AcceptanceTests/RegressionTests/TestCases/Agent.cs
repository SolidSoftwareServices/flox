
using NUnit.Framework;
using System.Threading.Tasks;
using types = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.AccountTypes.types;
using queryTypes = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.SharedVariables.queryTypes;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.UITestAutomation
{
	class Agent : ResidentialPortalBrowserFixture
	{
		
		AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests t => new AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests();
        AgentSteps al => new AgentSteps(Context);

        [Test]
        [Category("regression")]
        public void LoginErrorUsernameEmpty()
        {
            Context.GivenOnTheAgentLoginScreen();
            al.WhenEnterLoginPassword(EnvironmentSet.Inputs.AgentLogin["Password"]);
            al.WhenClickLogin();
            al.ThenErrorUsernameEmpty();
        }
        [Test]
        [Category("regression")]
        public void LoginErrorPasswordEmpty()
        {
            Context.GivenOnTheAgentLoginScreen();
            al.WhenEnterLoginUsername(EnvironmentSet.Inputs.AgentLogin["Email"]);
            al.WhenClickLogin();
            al.ThenErrorPasswordEmpty();
        }
        [Test]
        [Category("regression")]
        public void LoginErrorIncorrectCredentials()
        {
            Context.GivenOnTheAgentLoginScreen();
            al.WhenEnterLoginUsername("IncorrectUser");
            al.WhenEnterLoginPassword("IncorrectPassword");
            al.WhenClickLogin();
            al.ThenErrorIncorrectCredentials();
        }
        [Test]
		[Category("regression")]
        public void SearchUsername()
        {
            Context.GivenOnTheAgentSearchScreen(EnvironmentSet.Inputs.AgentLogin);
            al.WhenEnterUserNameInUsernameField(EnvironmentSet.Inputs.AdminSearch["Email"]);
            al.WhenClickOnFindBusinessPartnerBtn();
            al.WhenClickOnDeRegistrationBtn();
            al.WhenClickOnCancelBtnOnDeRegistrationPopup();
            al.WhenClickOnViewBusinessPartnerBtn();
            al.ThenAccountShouldOpen();
        }
        [Test]
        [Category("regression")]
        public void SearchBP()
        {
            Context.GivenOnTheAgentSearchScreen(EnvironmentSet.Inputs.AgentLogin);
            al.WhenEnterBP(EnvironmentSet.Inputs.AdminSearch["BP"]);
            al.WhenClickOnFindBusinessPartnerBtn();     
            al.WhenClickOnViewBusinessPartnerBtn();
            al.ThenAccountShouldOpen();
        }
        [Test]
        [Category("regression")]
        public void SearchCity()
        {
            Context.GivenOnTheAgentSearchScreen(EnvironmentSet.Inputs.AgentLogin);
            al.WhenEnterCity(EnvironmentSet.Inputs.AdminSearch["City"]);
            al.WhenClickOnFindBusinessPartnerBtn();
            al.WhenClickOnViewBusinessPartnerBtn();
            al.ThenAccountShouldOpen();
        }
        [Test]
        [Category("regression")]
        public void SearchStreetNo()
        {
            Context.GivenOnTheAgentSearchScreen(EnvironmentSet.Inputs.AgentLogin);
            al.WhenEnterStreet(EnvironmentSet.Inputs.AdminSearch["Street"]);
            al.WhenEnterHouse(EnvironmentSet.Inputs.AdminSearch["House"]);
            al.WhenClickOnFindBusinessPartnerBtn();
            al.WhenClickOnViewBusinessPartnerBtn();
            al.ThenAccountShouldOpen();
        }
        [Test]
        [Category("regression")]
        public void SearchNo1()
        {
            Context.GivenOnTheAgentSearchScreen(EnvironmentSet.Inputs.AgentLogin);
            al.WhenEnterHouse("1");
            al.WhenClickOnFindBusinessPartnerBtn();
            al.WhenClickOnViewBusinessPartnerBtn();
            al.ThenAccountShouldOpen();
        }
        [Test]
        [Category("regression")]
        public void SearchEmpty()
        {
            Context.GivenOnTheAgentSearchScreen(EnvironmentSet.Inputs.AgentLogin);
            al.WhenClickOnFindBusinessPartnerBtn();
            al.ThenErrorEmpty();
        }
        [Test]
        [Category("regression")]
        public void MaximumRecords()
        {
            Context.GivenOnTheAgentSearchScreen(EnvironmentSet.Inputs.AgentLogin);
            al.WhenEnterUserNameInUsernameField(EnvironmentSet.Inputs.AdminSearch["Email"]);
            al.WhenClickOnFindBusinessPartnerBtn();
        }
        [Test]
        [Category("regression")]
        public void NoMoveHomeTab()
        {
            Context.GivenOnTheAgentSearchScreen(EnvironmentSet.Inputs.AgentLogin);
            al.WhenEnterUserNameInUsernameField(EnvironmentSet.Inputs.AdminSearch["Email"]);
            al.WhenClickOnFindBusinessPartnerBtn();
            al.WhenClickOnViewBusinessPartnerBtn();
            al.ThenAccountShouldOpen();
            al.WhenClickFirstAccount();
            al.ThenNoMoveHomeTab();
        }

	}
}
