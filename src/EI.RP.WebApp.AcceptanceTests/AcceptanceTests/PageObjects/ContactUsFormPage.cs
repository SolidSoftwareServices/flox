﻿using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using static EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.SharedVariables;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class ContactUsFormPage
    {
        protected IWebDriver driver { get; set; }
        WebDriverWait wait;

        protected SharedPageFunctions page => new SharedPageFunctions(driver);
        internal ContactUsFormPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal ContactUsFormPage(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal void AssertContactUsFormBeforeSelectQueryType()
        {
            AssertContactUsFormAccountField();
            AssertContactUsFormTypeOfQueryField();
            AssertContactUsFormSubjectField();
            AssertContactUsFormQueryField();
            AssertContactUsFormSubmitBtn();
        }

        internal void AssertContactUsFormAfterSelectQueryType()
        {
            AssertContactUsFormAccountNumberField();
            AssertContactUsFormMPRNField();
        }

        internal void AssertContactUs()
        {
            page.IsElementPresent(
                page.ByDataAttribute("page", "contact-us"));
        }

        internal void AssertContactUsConfirmationScreen()
        {
            page.IsElementPresent(
                 page.ByDataAttribute("page", IdentifierSelector.ContactUsPage.confirmation));
        }

        internal void AssertContactUsFormAccountField()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.ContactUsPage.accountFieldID));
        }

        internal void errorAccountNumberAlpha()
        {
            page.IsElementTextPresent((string) TextMatch.ContactUsPage.errorAccountNumberAlpha);
        }

        internal void errorAccountNumberEmpty()
        {
            page.IsElementTextPresent((string) TextMatch.ContactUsPage.errorAccountNumberAlpha);
        }

        internal void errorAccountNumberShort()
        {
            page.IsElementTextPresent((string) TextMatch.ContactUsPage.errorAccountNumberShort);
        }

        internal void errorMPRNorGPRNAlpha()
        {
            page.IsElementTextPresent((string) TextMatch.ContactUsPage.errorMPRNorGPRNAlpha);
        }

        internal void errorMPRNorGPRNShort()
        {
            page.IsElementTextPresent((string) TextMatch.ContactUsPage.errorMPRNorGPRNShort);
        }

        internal void errorMPRNorGPRNEmpty()
        {
            page.IsElementTextPresent((string) TextMatch.ContactUsPage.errorMPRNorGPRNEmpty);
        }

        internal void errorSubjectEmpty()
        {
            page.IsElementTextPresent((string) TextMatch.ContactUsPage.errorSubjectEmpty);
        }

        internal void errorQueryEmpty()
        {
            page.IsElementTextPresent((string) TextMatch.ContactUsPage.errorQueryEmpty);
        }

        internal void errorQueryLonger1900()
        {
            page.IsElementTextPresent((string) TextMatch.ContactUsPage.errorQueryLonger1900);
        }

        internal void AssertContactUsFormTypeOfQueryField()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.ContactUsPage.typeOfQueryFieldID));
        }

        internal void AssertContactUsFormAccountNumberField()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.ContactUsPage.accountFieldID));
        }

        internal void AssertContactUsFormMPRNField()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.ContactUsPage.MPRNFieldID));
        }

        internal void AssertContactUsFormSubjectField()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.ContactUsPage.subjectFieldID));
        }

        internal void AssertContactUsFormQueryField()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.ContactUsPage.queryFieldID));
        }

        internal void AssertContactUsFormSubmitBtn()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.ContactUsPage.submitBtnID));
        }

        internal void ClickContactUsFormSubmitBtn()
        {
            page.ClickElement(By.Id(IdentifierSelector.ContactUsPage.submitBtnID));
        }

        internal void SelectAccountFromDrpDown()
        {
            page.ClickElement(By.Id(IdentifierSelector.ContactUsPage.accountFieldID));
            Thread.Sleep(1000);
            new Actions(driver).SendKeys(Keys.ArrowDown).Perform();
            new Actions(driver).SendKeys(Keys.Return).Perform();
        }

        internal void SelectQueryFromDrpDown(queryTypes type)
        {
            page.ClickElement(By.Id(IdentifierSelector.ContactUsPage.typeOfQueryFieldID));
            Thread.Sleep(1000);
            switch (type) {
                case queryTypes.other:
                    new Actions(driver).SendKeys(Keys.ArrowDown).Perform();
                    new Actions(driver).SendKeys(Keys.ArrowDown).Perform();
                    new Actions(driver).SendKeys(Keys.ArrowDown).Perform();
                    new Actions(driver).SendKeys(Keys.ArrowDown).Perform();
                    break;
                case queryTypes.meterReadQuery:           
                    new Actions(driver).SendKeys(Keys.ArrowDown).Perform();
                    new Actions(driver).SendKeys(Keys.ArrowDown).Perform();
                    new Actions(driver).SendKeys(Keys.ArrowDown).Perform();
                    break;
                case queryTypes.billOrPaymentQuery:
                    new Actions(driver).SendKeys(Keys.ArrowDown).Perform();
                    new Actions(driver).SendKeys(Keys.ArrowDown).Perform();
                    break;
                case queryTypes.additionalAccount:
                    new Actions(driver).SendKeys(Keys.ArrowDown).Perform();
                    break;
            }
            new Actions(driver).SendKeys(Keys.Return).Perform();
        }

        internal void EnterAccountNumber(string s)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.ContactUsPage.accountNumberFieldID), s);
        }

        internal void EnterMPRN(string s)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.ContactUsPage.MPRNFieldID), s);
        }

        internal void EnterSubject(string s)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.ContactUsPage.subjectFieldID), s);
        }

        internal void EnterQueryText(string s)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.ContactUsPage.queryFieldID), s);
        }
    }
}