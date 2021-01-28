using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using NUnit.Framework;
using OpenQA.Selenium;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;

namespace EI.RP.WebApp.AcceptanceTests.Infrastructure
{
	public class SingleTestContext:IDisposable
	{
		public SingleTestContext()
		{
			Driver=new Lazy<ResidentialPortalWebDriver>(() =>
			{
				var driver = DriversPool.Default.Value.GetOneReleasedOrCreateNew();
				driver.Instance.Navigate().GoToUrl(TestSettings.Default.PublicTargetUrl);
				driver.Instance.Manage().Window.Maximize();
				return driver;
			});
		}

		public Lazy<ResidentialPortalWebDriver> Driver { get; private set; }
		private DashBoardPage DashBoardPage => new DashBoardPage(Driver.Value.Instance);
        internal SharedPageFunctions Page => new SharedPageFunctions(Driver.Value.Instance);
        internal AgentLoginPage AgentLoginPage => new AgentLoginPage(Driver.Value.Instance);
        internal AgentSearchPage AgentSearchPage => new AgentSearchPage(Driver.Value.Instance);
        public bool IsPublicTarget => Driver.Value.IsPublicTarget;
		public bool IsInternalTarget => Driver.Value.IsInternalTarget;


		public void WhenClickMyAccountsNavButton()
		{
			DashBoardPage.ClickMyAccountsNavBtn();
			
		}

		public void WhenClickAccountOverviewNavButton()
        {
            DashBoardPage.ClickUsageNavBtn();

		}        

        public void WhenClickBillsPaymentsNavButton()
        {
            DashBoardPage.ClickBillsAndPaymentsNavBtn();
		}
        public void WhenClickMoveHouseNavBtn()
        {
            DashBoardPage.ClickMoveHouseNavBtn();
        }

        public void WhenClickMeterReadingNavButton()
        {
            DashBoardPage.ClickMeterReadingNavBtn();
        }

        public void WhenClickContactUsInTopNavigationBar()
        {
            DashBoardPage.ClickContactUsForm();
        }

        public void WhenClickHelpNavButton()
		{
			DashBoardPage.ClickHelpNavBtn();
		}

        public void WhenClickPlanNavButton()
        {
	        DashBoardPage.ClickPlanNavBtn();
        }

        internal bool InsideAccount(string accountNumber)
		{

            var el = Driver.Value.Instance.FindElements(By.CssSelector("[data-testid='portal-header-account-number']"));

			if (el.Any() &&  el.First().Text.Contains(accountNumber))
			{
				return true;
			}
			return false;
        }
        internal async Task WhenCheckBillResponse(IDictionary dict)
        {
            if (Driver.Value.Instance.FindElements(Page.ByDataAttribute(val: $"account-card-view-latest-bill-{dict["Account"]}")).Count>0)
            {
                var el = Driver.Value.Instance.FindElementEx(Page.ByDataAttribute(val: $"account-card-view-latest-bill-{dict["Account"]}"));
                string href = el.GetAttribute("href");
                var stream = await Driver.Value.RequestFileInSession(href);
                Assert.IsTrue(stream.Length > 51200);
                stream.Close();             
            }
        }

        internal async Task NavigateToTestStart(IDictionary acc)
		{
			var u = new UsageSteps(this);
			//this seems wrong, just refactored as-is
            if (Driver.Value.Instance.Url.Contains(TestSettings.Default.InternalTargetUrl))
            {
	            Driver.Value.Instance.Navigate().GoToUrl(TestSettings.Default.PublicTargetUrl);
            }

			if (InsideAccount(acc["Account"]))
			{
				WhenClickMyAccountsNavButton();
                await WhenCheckBillResponse(acc);
                u.GivenOnUsagePageOfAccount(acc);
				Thread.Sleep(TimeSpan.FromSeconds(2));
			}
			else
			{
				var signInPage = new SignInPage(Driver.Value.Instance);
				signInPage.SignIn(acc, TestSettings.Default.PublicTargetUrl);
                await WhenCheckBillResponse(acc);
                u.GivenOnUsagePageOfAccount(acc);
            }
		}
        public void GivenOnTheAgentLoginScreen()
        {
            Driver.Value.Instance.Navigate().GoToUrl(TestSettings.Default.InternalTargetUrl);

            AgentLoginPage.AssertAgentLoginPage();
        }
        internal void GivenOnTheAgentSearchScreen(IDictionary dict)
        {
            Driver.Value.Instance.Navigate().GoToUrl(TestSettings.Default.InternalTargetUrl);
            
            if (OnAgentLoginScreen())
            {
                Debug.Print("On AGent loin");
                AgentLoginPage.AssertAgentLoginPage();
                WhenLoginToTheAgentPortal(dict);
            }
        }
        private bool OnAgentLoginScreen()
        {
            return AgentLoginPage.OnPage();
        }
        public void WhenLoginToTheAgentPortal(IDictionary dict)
        {
            AgentLoginPage.AgentSignIn(dict);
            AgentSearchPage.AssertAgentSearchPage();
        }

        internal void NavigateToTestStartStayOnMyAccounts(IDictionary acc)
		{
			//this seems wrong, just refactored as-is
			if (Driver.Value.Instance.Url.Contains(TestSettings.Default.InternalTargetUrl))
			{
				Driver.Value.Instance.Navigate().GoToUrl(TestSettings.Default.PublicTargetUrl);
			}

			if (InsideAccount(acc["Account"]))
			{
				WhenClickMyAccountsNavButton();
				Thread.Sleep(TimeSpan.FromSeconds(2));
			}
			else
			{
				var signInPage = new SignInPage(Driver.Value.Instance);
				signInPage.SignIn(acc, TestSettings.Default.PublicTargetUrl);
			}
		}

		

		public void Dispose()
		{
			Dispose(true);
		}
		private readonly object _syncLock=new object();
		private bool _disposed = false;
		private bool _failed;

		protected void Dispose(bool disposing)
		{
			if (!_disposed)
				lock (_syncLock)
					if (!_disposed)
					{
						if (Driver.IsValueCreated)
						{
							if (!_failed)
							{
								DriversPool.Default.Value.Release(Driver.Value);
							}
							else
							{
								Driver.Value.Dispose();
							}

							Driver = null;
						}

						_disposed = true;
					}
		}

		~SingleTestContext()
		{
			Dispose(false);
		}

		public void NotifyFailure()
		{
			//don't recycle failed back to app pool... other valid will be there
			this._failed = true;
		}
	}
}
