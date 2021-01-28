using OpenQA.Selenium;
using System;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
	class AgentSearchPage
	{
		protected IWebDriver driver { get; set; }
		private SharedPageFunctions page => new SharedPageFunctions(driver); 
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

		internal Boolean AssertAgentSearchPage()
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
            return true;
		}
		internal void AgentSearchEnterEmailAdress(string username)
		{
			EnterEmailAddress(username);
		}
		internal void EnterEmailAddress(string username)
		{
			page.SendElementKeys(By.Id(emailAdressFieldID), username);
		}
		internal void ClickFindBusinessPartnerBtn()
		{
			page.ClickElement(By.Id(findBusinessPartnersBtnID));
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
            page.ClickElement(By.Id("deregister"), TimeSpan.FromMinutes(1));
		}

        internal bool OnPage()
        {
            return driver.FindElements(By.Id(IdentifierSelector.AgentSearchPage.BP)).Count>0?true:false;
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
			page.ClickElement(page.ByDataAttribute(val: "agent-btn-cancel-modal"));
		}
		internal void ClickViewBusinessPartnerBtn()
		{
			page.ClickElement(By.XPath("//*[@id='btnSubmit'][contains(@value,'BusinessPartnerSelected')]"),TimeSpan.FromMinutes(1));
		}

		internal void AssertSearchPageMyProfileBtnDisplayed()
		{
			page.IsElementPresent(By.Id(myProfileBtnID));
		}

        internal void EnterBP(string v)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.AgentSearchPage.BP), v);
        }

        internal void AssertSearchPageLogOutLinkDisplayed()
		{
			page.IsElementPresent(By.XPath("//a[contains(text(), 'Log out')")); // TBD
		}

        internal void EnterStreet(string v)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.AgentSearchPage.Street), v);
        }

        internal void EnterHouse(string v)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.AgentSearchPage.HouseNo), v);
        }

        internal void AssertSearchPageTitle()
		{
			page.IsElementPresent(By.XPath("//h2[contains(text(),'Agent Search')]")); // TBD
		}

        internal void EnterCity(string v)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.AgentSearchPage.City), v);
        }

        internal void AssertSearchPageBusinessPartnerFieldDisplayed()
		{
			page.IsElementPresent(By.Id(businessPartnerFieldID));
		}

        internal void AssertErrorEmpty()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.AgentSearchPage.errorEmpty));
        }

        internal void AssertSearchPageUsernameFieldDisplayed()
		{
			page.IsElementPresent(By.Id(emailAdressFieldID));
		}
		internal void AssertSearchPageStreetFieldDisplayed()
		{
			page.IsElementPresent(By.Id(streetFieldID));
		}
		internal void AssertSearchPageHouseNoFieldDisplayed()
		{
			page.IsElementPresent(By.Id(houseNoFieldID));
		}
		internal void AssertSearchPageCityFieldDisplayed()
		{
			page.IsElementPresent(By.Id(cityFieldID));
		}
		internal void AssertSearchPageMaxRecordsFieldDisplayed()
		{
			page.IsElementPresent(By.Id(maximumRecordsID));
		}
		internal void AssertSearchPageFindBusinessPartnersBtnDisplayed()
		{
			page.IsElementPresent(By.Id(findBusinessPartnersBtnID));
		}

		internal void AssertDeRegistrationPopUpCloseBtn()
		{
			page.IsElementPresent(page.ByDataAttribute(val: "agent-btn-close-modal"));
		}
		internal void AssertDeRegistrationPopUpHeader()
		{
			page.IsElementPresent(By.XPath("//h1[contains(text(), 'Confirm De-Registration')]"));
		}
		internal void AssertDeRegistrationPopUpSubtitle()
		{
			page.IsElementPresent(By.XPath("//p[contains(text(), 'Are you sure you want to de-register?')]"));
		}
		internal void AssertDeRegistrationPopUpSubConfirmBtn()
		{
			page.IsElementPresent(By.XPath("//*[@id='btnSubmit'][contains(@value,'DeRegistrationRequested')]"));
		}
		internal void AssertDeRegistrationPopUpSubCancelBtn()
		{
			page.IsElementPresent(page.ByDataAttribute(val: "agent-btn-cancel-modal"));
		}
		internal void AssertBusinessPartnerTableBPColumnDisplayed()
		{
			page.IsElementPresent(By.XPath("//*[@data-testid='search-result-table']/thead/tr/th[1]"));
		}
		internal void AssertBusinessPartnerTableDescColumnDisplayed()
		{
			page.IsElementPresent(By.XPath("//*[@data-testid='search-result-table']/thead/tr/th[2]"));
		}
		internal void AssertBusinessPartnerTableUserNameColumnDisplayed()
		{
			page.IsElementPresent(By.XPath("//*[@data-testid='search-result-table']/thead/tr/th[3]"));
		}

		internal void AssertBusinessPartnerTableActionsColumnDisplayed()
		{
			page.IsElementPresent(By.XPath("//*[@data-testid='search-result-table']/tr/th[4]"));
		}

		internal void AssertViewBusinessPartnerBtnDisplayed()
		{
			page.IsElementPresent(By.XPath("//*[@id='btnSubmit'][contains(@value,'BusinessPartnerSelected')]"));
		}

		internal void AssertDeRegistrationBtnDisplayed()
		{
			page.IsElementPresent(By.Id("deregister"));
		}
	}
}
