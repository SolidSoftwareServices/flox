using OpenQA.Selenium;
using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class RegisterPage
    {
        protected IWebDriver driver { get; set; }
        protected SharedPageFunctions page => new SharedPageFunctions(driver);
        internal RegisterPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal RegisterPage(IWebDriver driver0)
        {
            driver = driver0;
        }
        WebDriverWait wait;
        internal const string overrideName = "overridelink";

        internal void ClickAccountNumberInformationButton()
        {
            page.ClickElement(By.Id(IdentifierSelector.RegisterPage.accountNumberInfoModalBtn));
        }

        const string loginBtnText = "Already have an account?",
            privacyNoticeText = "Electric Ireland requires the below information to create and administer your account. The data controller is the Electricity Supply Board, trading as Electric Ireland. Please refer to our Privacy Notice.",
            subHeaderText = "Set up your online account";
        internal void EnterFirstName(string firstName)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.RegisterPage.firstNameID), firstName);
        }
        internal void EnterLastName(string lastName)
        {
   
            page.SendElementKeys(By.Id(IdentifierSelector.RegisterPage.lastNameID), lastName);
        }
        internal void EnterAccountNumber(string accountNumber)
        {
   
            page.SendElementKeys(By.Id(IdentifierSelector.RegisterPage.accountNumberID), accountNumber);
        }
        internal void EnterLastSixDigitsOfMPRN(string mprn)
        {
   
            page.SendElementKeys(By.Id(IdentifierSelector.RegisterPage.mprnID), mprn);
        }
        internal void EnterPhoneNumber(string phoneNumber)
        {
   
            page.SendElementKeys(By.Id(IdentifierSelector.RegisterPage.phoneNumberID), phoneNumber);
        }
        internal void AssertPrivacyNoticeText(string privacyNoticeText)
        {
   
            page.IsElementPresent(By.XPath("//p[contains(text(),'" + privacyNoticeText + "')]"));
        }
        internal void EnterRegisterEmail(string email)
        {
            Random rnd = new Random();
            int uniqueEmailGen = rnd.Next(1, 10000);
   
            page.SendElementKeys(By.Id(IdentifierSelector.RegisterPage.emailID), uniqueEmailGen + email);
        }
        internal void EnterDOB(string day, string month, string year)
        {
            EnterDOBDay(day);
            EnterDOBMonth(month);
            EnterDOBYear(year);
        }
        internal void EnterDOBDay(string day)
        {
   
            page.SendElementKeys(By.Id(IdentifierSelector.RegisterPage.dateOfBirthDayID), day);
        }
        internal void AssertFirstNameFieldIsPopulated(string firstName)
        {
   
            page.IsElementPopulated(By.Id(IdentifierSelector.RegisterPage.firstNameID), firstName);
        }
        internal void AssertLastNameFieldIsPopulated(string lastName)
        {
   
            page.IsElementPopulated(By.Id(IdentifierSelector.RegisterPage.lastNameID), lastName);
        }
        internal void EnterDOBMonth(string month)
        {
   
            page.SendElementKeys(By.Id(IdentifierSelector.RegisterPage.dateOfBirthMonthID), month);
        }
        internal void AssertMPRNFieldIsPopulated(string mprn)
        {
   
            page.IsElementPopulated(By.Id(IdentifierSelector.RegisterPage.mprnID), mprn);
        }
        internal void AssertEmailFieldIsPopulated(string email)
        {
   
            page.IsElementPopulated(By.Id(IdentifierSelector.RegisterPage.emailID), email);
        }
        internal void EnterDOBYear(string year)
        {
   
            page.SendElementKeys(By.Id(IdentifierSelector.RegisterPage.dateOfBirthYearID), year);
        }
        internal void AssertPhoneNumberFieldIsPopulated(string phoneNumber)
        {
   
            page.IsElementPopulated(By.Id(IdentifierSelector.RegisterPage.phoneNumberID), phoneNumber);
        }
        internal void AssertDOBFieldIsPopulated(string dob)
        {
            string[] dateFigures = dob.Split('/');
            string day = dateFigures[0].ToString();
            string month = dateFigures[1].ToString();
            string year = dateFigures[2].ToString();
   
            page.IsElementPopulated(By.Id(IdentifierSelector.RegisterPage.dateOfBirthYearID), year);
            page.IsElementPopulated(By.Id(IdentifierSelector.RegisterPage.dateOfBirthMonthID), month);
            page.IsElementPopulated(By.Id(IdentifierSelector.RegisterPage.dateOfBirthDayID), day);
        }
        internal void ConfirmThatIAmAnAccountHolder()
        {
   
            page.ClickElement(By.Id(IdentifierSelector.RegisterPage.ownerID));
        }
        internal void AssertAccountHolderCheckboxIsPopulated(string permission)
        {
            throw new NotImplementedException();
        }
        internal void AssertTermsAndConditionsCheckboxIsPopulated(string permission)
        {
            throw new NotImplementedException();
        }
        internal void AcceptTheTermsAndConditions()
        {
            IWebElement termsAndConditionsField = driver.FindElementEx(By.Id("chkAcceptTC"));//"//em[contains(text(),'I accept')]"));
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            //wait.Until(driver => termsAndConditionsField.Displayed);
            termsAndConditionsField.Click();
        }
        internal void AssertTermsAndConditionsPage()
        {
            driver.SwitchTo().Window(driver.WindowHandles[1]);
   
            page.IsElementPresent(By.XPath("//p[contains(text(),'This website is owned and operated by Electric Ireland')]"));
        }
        internal void AssertFirstNameFieldPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.firstNameID));
        }
        internal void NavigateToTermsAndConditionsPage()
        {
            ClickTheTermsAndConditions();
        }
        internal void AssertLastNameFieldPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.lastNameID));
        }
        internal void ClickTheTermsAndConditions()
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement termsAndConditionsLink = wait.Until<IWebElement>(d => d.FindElementEx(By.XPath("//a[contains(text(),'Terms and Conditions')]")));
            termsAndConditionsLink.Click();
        }
        internal void AssertAccountNumberFieldPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.accountNumberID));
        }
        internal void AssertAccountNumberInformationButtonPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.accountNumberInformationBtnID));
        }
        internal void AssertAccountNumberInformationModalPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.accountNumberInformationModalHeaderID));
        }
        internal void ClickRegister()
        {
   
            page.ClickElement(By.Id(IdentifierSelector.RegisterPage.registerBtnID));
        }
        internal void AssertMPRNInformationButtonPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.mprnInformationBtnID));
        }
        internal void AssertMPRNFieldPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.mprnID));
        }
        internal void AssertMPRNInformationModalPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.mprnID));
        }
        internal void AssertErrorEmptyFirstName()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorFirstNameID));
        }
        internal void AssertEmailFieldPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.emailID));
        }
        internal void AssertPhoneNumberFieldPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.phoneNumberID));
        }
        internal void AssertAccountCreated()
        {
   
            page.IsElementPresent(By.XPath("//h1[contains(text(),'Check your email')]"));
        }
        internal void AssertDOBFieldsPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.dateOfBirthDayID));
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.dateOfBirthMonthID));
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.dateOfBirthYearID));
        }
        internal void AssertErrorEmptyLastName()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorLastNameID));
        }
       
        internal void AssertTermsAndConditionsCheckboxPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.termsAndConditionsID));
        }
        internal void AssertRegisterBtnGreyedOut()
        {
            IWebElement createAccountBtn = driver.FindElementEx(By.Id(IdentifierSelector.RegisterPage.registerBtnID));
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => createAccountBtn.Displayed);
            Assert.IsFalse(createAccountBtn.Enabled);
        }
        internal void AssertRegisterButtonPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.registerBtnID));
        }
        internal void AssertLoginButtonTextPresentForExistingUser()
        {
   
            page.IsElementPresent(By.XPath(loginBtnText));
        }
        internal void AssertErrorInvalidFirstName()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorFirstNameID));
        }
        internal void AssertLoginButtonPresentForExistingUser()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.loginBtnID));
        }
        internal void AssertImageOnRightSideOfScreen()
        {
            throw new NotImplementedException();
        }
        internal void AssertEILogo()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.logoID));
        }
        internal void ClickMPRNInformationButton()
        {
   
            page.ClickElement(By.Id(IdentifierSelector.RegisterPage.mprnInformationBtnID));
        }
        internal void AssertModalHeaderPresent(string headerText)
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.modalMPRNHeaderID));
        }
        internal void AssertModalTextPresent(string modalText)
        {
   
            page.IsElementPresent(By.XPath("//p[contains(text(),'" + modalText + "')]"));
        }
        internal void AssertErrorInvalidLastName()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorLastNameID));
        }
        internal void AssertCloseButtonPresent()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.modalMPRNCloseBtnID));
        }
        internal void ClickGoBackButton()
        {
   
            page.ClickElement(By.XPath(XPathSelectors.RegisterPage.goBackBtn));
        }
        internal void ClickModalCloseButton()
        {
   
            page.ClickElement(By.Id(IdentifierSelector.RegisterPage.modalMPRNCloseBtnID));
        }
        internal void AssertErrorEmailInUse()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorEmailID));
        }
        internal void AssertErrorInvalidAccountNumber()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorAccountNumberID));
        }
        internal void AssertErrorEmptyAccountNumber()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorAccountNumberID));
        }
        internal void AssertErrorAccountNumber()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorAccountNumberID));
        }
        internal void AssertErrorEmptyMPRN()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorMPRNID));
        }
        internal void AssertErrorInvalidMPRN()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorMPRNID));
        }
        internal void AssertErrorInvalidEmail()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorEmailID));
        }
        internal void AssertErrorEmptyEmail()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorEmailID));
        }
        internal void AssertErrorInvalidPhoneNumber()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorPhoneNumberID));
        }
        internal void AssertErrorEmptyPhoneNumber()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorPhoneNumberID));
        }
        internal void AssertErrorInvalidDOB()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorDOBID));
        }
        internal void AssertErrorEmptyDOB()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorDOBID));
        }
        internal void AssertErrorLongMPRN()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorMPRNID));
        }
        internal void AssertErrorLongEmail()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorEmailID));
        }
        internal void AssertErrorShortMPRN()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorMPRNID));
        }
        internal void AssertErrorShortEmail()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorEmailID));
        }
        internal void AssertErrorLongPhoneNumber()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorPhoneNumberID));
        }
        internal void AssertErrorShortPhoneNumber()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorPhoneNumberMinID));
        }
        internal void AssertErrorLongAccountNumber()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorAccountNumberID));
        }
        internal void AssertErrorShortAccountNumber()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorAccountNumberID));
        }
        internal void AssertErrorEmptyTermsAndConditions()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorTandCID));
        }
        internal void AssertErrorEmptyAccountHolder()
        {
   
            page.IsElementPresent(By.Id(IdentifierSelector.RegisterPage.errorOwnerID));
        }
        internal void AssertRegisterPageText(string message)
        {
   
            page.IsElementPresent(By.XPath("//h3[contains(text(),\"" + message.Replace("'", "\'") + "\")]"));
        }
        internal void AssertRegisterPage()
        {
   
            page.IsElementPresent(By.XPath("//h1[contains(text(),'Register')]"));
        }
        internal void ClickPrivacyNoticeLink()
        {
   
            page.ClickElement(By.Id(IdentifierSelector.RegisterPage.privacyNoticeLinkID));
        }
    }
}
