using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
	class AgentSearchPage
	{
		protected IWebDriver driver { get; set; }
		private SharedPageFunctions _sharedPageFunctions => new SharedPageFunctions(driver);
		internal AgentSearchPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal AgentSearchPage(IWebDriver driver0)
		{
			driver = driver0;
		}
		internal string
			myProfileBtnID = "btnMyProfile",
			//logOutBtnID = "", TBD
			businessPartnerFieldID = "Partner",
			emailAdressFieldID = "FirstName",
			streetFieldID = "Street",
			houseNoFieldID = "House",
			cityFieldID = "City",
			maximumRecordsID = "MaxNo",
			findBusinessPartnersBtnID = "btnSubmit";

		internal void AssertAgentSearchPage()
		{
			// AssertSearchPageMyProfileBtnDisplayed(); TBD
			// AssertSearchPageLogOutLinkDisplayed(); TBD
			AssertSearchPageTitle();
			AssertSearchPageBusinessPartnerFieldDisplayed();
			AssertSearchPageUsernameFieldDisplayed();
			AssertSearchPageStreetFieldDisplayed();
			AssertSearchPageHouseNoFieldDisplayed();
			AssertSearchPageCityFieldDisplayed();
			AssertSearchPageMaxRecordsFieldDisplayed();
			AssertSearchPageFindBusinessPartnersBtnDisplayed();
		}
		internal void AgentSearchEnterEmailAdress(string username)
		{
			EnterEmailAddress(username);
		}
		internal void EnterEmailAddress(string username)
		{
			_sharedPageFunctions.SendElementKeys(By.Id(emailAdressFieldID), username);
		}
		internal void ClickFindBusinessPartnerBtn()
		{
			_sharedPageFunctions.ClickElement(By.Id(findBusinessPartnersBtnID));
		}
		internal void AssertBusinessPartnerTable()
		{
			AssertBusinessPartnerTableBPColumnDisplayed();
			AssertBusinessPartnerTableDescColumnDisplayed();
			AssertBusinessPartnerTableUserNameColumnDisplayed();
			AssertBusinessPartnerTableActionsColumnDisplayed();
			AssertViewBusinessPartnerBtnDisplayed();
			AssertDeRegistrationBtnDisplayed();
		}
		internal void ClickDeRegistrationBtn()
		{
			_sharedPageFunctions.ClickElement(By.Id("deregister"));
		}
		internal void AssertDeRegistrationPopUp()
		{
			AssertDeRegistrationPopUpHeader();
			AssertDeRegistrationPopUpSubtitle();
			AssertDeRegistrationPopUpSubConfirmBtn();
			AssertDeRegistrationPopUpSubCancelBtn();
			AssertDeRegistrationPopUpCloseBtn();
		}

		internal void ClickDeRegistrationPopUpCancelBtn()
		{
			_sharedPageFunctions.ClickElement(_sharedPageFunctions.ByDataAttribute(val: "agent-btn-cancel-modal"));
			Thread.Sleep(TimeSpan.FromSeconds(2));
		}
		internal void ClickViewBusinessPartnerBtn()
		{
			_sharedPageFunctions.ClickElement(By.XPath("//*[@id='btnSubmit'][contains(@value,'BusinessPartnerSelected')]"));
		}

		internal void AssertSearchPageMyProfileBtnDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.Id(myProfileBtnID));
		}

		internal void AssertSearchPageLogOutLinkDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.XPath("//a[contains(text(), 'Log out')")); // TBD
		}
		internal void AssertSearchPageTitle()
		{
			_sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(),'Agent Search')]")); // TBD
		}
		internal void AssertSearchPageBusinessPartnerFieldDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.Id(businessPartnerFieldID));
		}
		internal void AssertSearchPageUsernameFieldDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.Id(emailAdressFieldID));
		}
		internal void AssertSearchPageStreetFieldDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.Id(streetFieldID));
		}
		internal void AssertSearchPageHouseNoFieldDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.Id(houseNoFieldID));
		}
		internal void AssertSearchPageCityFieldDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.Id(cityFieldID));
		}
		internal void AssertSearchPageMaxRecordsFieldDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.Id(maximumRecordsID));
		}
		internal void AssertSearchPageFindBusinessPartnersBtnDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.Id(findBusinessPartnersBtnID));
		}

		internal void AssertDeRegistrationPopUpCloseBtn()
		{
			_sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "agent-btn-close-modal"));
		}
		internal void AssertDeRegistrationPopUpHeader()
		{
			_sharedPageFunctions.IsElementPresent(By.XPath("//h1[contains(text(), 'Confirm De-Registration')]"));
		}
		internal void AssertDeRegistrationPopUpSubtitle()
		{
			_sharedPageFunctions.IsElementPresent(By.XPath("//p[contains(text(), 'Are you sure you want to de-register?')]"));
		}
		internal void AssertDeRegistrationPopUpSubConfirmBtn()
		{
			_sharedPageFunctions.IsElementPresent(By.XPath("//*[@id='btnSubmit'][contains(@value,'DeRegistrationRequested')]"));
		}
		internal void AssertDeRegistrationPopUpSubCancelBtn()
		{
			_sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "agent-btn-cancel-modal"));
		}
		internal void AssertBusinessPartnerTableBPColumnDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.XPath("//*[@data-testid='search-result-table']/thead/tr/th[1]"));
		}
		internal void AssertBusinessPartnerTableDescColumnDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.XPath("//*[@data-testid='search-result-table']/thead/tr/th[2]"));
		}
		internal void AssertBusinessPartnerTableUserNameColumnDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.XPath("//*[@data-testid='search-result-table']/thead/tr/th[3]"));
		}

		internal void AssertBusinessPartnerTableActionsColumnDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.XPath("//*[@data-testid='search-result-table']/thead/tr/th[4]"));
		}

		internal void AssertViewBusinessPartnerBtnDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.XPath("//*[@id='btnSubmit'][contains(@value,'BusinessPartnerSelected')]"));
		}

		internal void AssertDeRegistrationBtnDisplayed()
		{
			_sharedPageFunctions.IsElementPresent(By.Id("deregister"));
		}
	}
}
