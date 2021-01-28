using OpenQA.Selenium;
using System;
using EI.RP.WebApp.RegressionTests.PageObjects;
using System.Threading;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
using static EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.SharedVariables;
using System.Collections;
using EI.RP.WebApp.AcceptanceTests;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.UITestAutomation
{
    public class RegisterSteps : BaseStep
    {

        public RegisterSteps(SingleTestContext shared) : base(shared)
        { }

        private RegisterPage registerPage => new RegisterPage(shared.Driver.Value);

        internal void GivenNavigateToTheRegisterPage()
        {
            SignInPage signInPage = new SignInPage(shared.Driver.Value);
            signInPage.ClickCreateAccount();
            Thread.Sleep(2000);
        }


        public void ThenMPRNPopupModalShouldAppear()
        {
            registerPage.AssertMPRNInformationModalPresent();
        }

        public void ThenAccountNumberPopupModalShouldAppear()
        {
            registerPage.AssertAccountNumberInformationModalPresent();
        }

        public void WhenClickAccountNumberFieldInformationButton()
        {
            registerPage.ClickAccountNumberInformationButton();
        }

        internal void GivenNavigateToTheSmartHubPage()
        {
            shared.Driver.Value.Instance.Navigate().GoToUrl(TestSettings.Default.SmartHubUrl());
        }

        public void WhenEnterFutureDOB()
        {
            var today = DateTime.Now;
            string dob = today.AddDays(1).ToString();
            string charactersToReplace = dob.Substring(dob.Length - 11);
            dob.Replace(charactersToReplace, "");
            string[] dateFigures = dob.Split('/');
            string month = dateFigures[0].ToString();
            string day = dateFigures[1].ToString();
            string year = dateFigures[2].ToString();
            registerPage.EnterDOB(day, month, year);
        }

        internal void WhenClickSignUpForSmartServices()
        {
            SmartHubPage smartHubPage = new SmartHubPage(shared.Driver.Value);
            smartHubPage.ClickSignUpForSmartServices();
        }

        internal void WhenEnterFirstName(string firstName)
        {
            registerPage.EnterFirstName(firstName);
        }

        internal void WhenEnterLastName(string lastName)
        {
            registerPage.EnterLastName(lastName);
        }

        internal void WhenEnterAccountNumber(string accountNumber)
        {
            registerPage.EnterAccountNumber(accountNumber);
        }

        internal void WhenEnterLastSixDigitsOfMPRN(string mprn)
        {
            registerPage.EnterLastSixDigitsOfMPRN(mprn);
        }

        internal void WhenEnterPhoneNumber(string phoneNumber)
        {
            registerPage.EnterPhoneNumber(phoneNumber);
        }

        internal void ThenPrivacyNoticeShouldSay(string privacyNoticeText)
        {
            registerPage.AssertPrivacyNoticeText(privacyNoticeText);
        }

        internal void WhenEnterEmail(string email)
        {
            registerPage.EnterRegisterEmail(email);
        }

        internal void WhenEnterDOB(string dob)
        {
            string[] dateFigures = dob.Split('/');
            string day = dateFigures[0].ToString();
            string month = dateFigures[1].ToString();
            string year = dateFigures[2].ToString();
            registerPage.EnterDOB(day, month, year);
        }

        internal void WhenConfirmThatIAmAnAccountHolder()
        {
            registerPage.ConfirmThatIAmAnAccountHolder();
        }

        public void ThenFirstNameFieldPopulatedAs(string firstName)
        {
            registerPage.AssertFirstNameFieldIsPopulated(firstName);
        }

        public void ThenLastNameFieldPopulatedAsBloggs(string lastName)
        {
            registerPage.AssertLastNameFieldIsPopulated(lastName);
        }

        public void ThenLastSixDigitsOfMPRNFieldPopulatedAs(string mprn)
        {
            registerPage.AssertMPRNFieldIsPopulated(mprn);
        }

        public void ThenEmailFieldPopulatedAs(string email)
        {
            registerPage.AssertEmailFieldIsPopulated(email);
        }

        public void ThenPhoneNumberFieldPopulatedAs(string phoneNumber)
        {
            registerPage.AssertPhoneNumberFieldIsPopulated(phoneNumber);
        }

        public void ThenDOBFieldPopulatedAs(string dob)
        {
            registerPage.AssertDOBFieldIsPopulated(dob);
        }

        public void ThenTermsAndConditions(string permission)
        {
            registerPage.AssertTermsAndConditionsCheckboxIsPopulated(permission);
        }

        public void ThenIAmAnAccountHolder(string permission)
        {
            registerPage.AssertAccountHolderCheckboxIsPopulated(permission);
        }

        public void ThenFirstNameInputFieldShouldBePresent()
        {
            registerPage.AssertFirstNameFieldPresent();
        }

        public void ThenLastNameInputFieldShouldBePresent()
        {
            registerPage.AssertLastNameFieldPresent();
        }

        public void ThenAccountNumberInputFieldShouldBePresent()
        {
            registerPage.AssertAccountNumberFieldPresent();
        }

        public void ThenThereShouldBeAnInformationButtonPresentInTheAccountNumberField()
        {
            registerPage.AssertAccountNumberInformationButtonPresent();
        }

        public void ThenLastDigitsOfMPRNInputFieldShouldBePresent(int p0)
        {
            registerPage.AssertMPRNFieldPresent();
        }

        public void ThenThereShouldBeAnInformationButtonPresentInTheMPRNField()
        {
            registerPage.AssertMPRNInformationButtonPresent();
        }

        public void ThenEmailInputFieldShouldBePresent()
        {
            registerPage.AssertEmailFieldPresent();
        }

        public void ThenPhoneNumberInputFieldShouldBePresent()
        {
            registerPage.AssertPhoneNumberFieldPresent();
        }

        public void ThenDOBTextBoxesArePresent()
        {
            registerPage.AssertDOBFieldsPresent();
        }


        public void ThenTheCheckboxIAcceptTheTermsAndConditionsShouldBePresent()
        {
            registerPage.AssertTermsAndConditionsCheckboxPresent();
        }




        public void WhenClickMPRNFieldInformationButton()
        {
            registerPage.ClickMPRNInformationButton();
        }

        public void WhenClickTheGoBackButton()
        {
            registerPage.ClickGoBackButton();
        }

        public void WhenClickTheModalXButton()
        {
            registerPage.ClickModalCloseButton();
        }

        public void ThenTheCreateAccountButtonShouldBePresent()
        {
            registerPage.AssertRegisterButtonPresent();
        }

        public void ThenThereShouldBeTextBesideTheLoginButtonForExistingCustomers()
        {
            registerPage.AssertLoginButtonTextPresentForExistingUser();
        }

        public void ThenTheLoginButtonShouldBePresentForExistingUser()
        {
            registerPage.AssertLoginButtonPresentForExistingUser();
        }

        public void ThenThereShouldBeAnImageOnTheRightHandSideOfTheScreen()
        {
            registerPage.AssertImageOnRightSideOfScreen();
        }

        public void ThenTheBackgroundImageContainsTheEILogo()
        {
            registerPage.AssertEILogo();
        }

        public void ThenModalHeaderShouldSay(string headerText)
        {
            registerPage.AssertModalHeaderPresent(headerText);
        }

        public void ThenModalTextShouldSay(string modalText)
        {
            registerPage.AssertModalTextPresent(modalText);
        }

        public void ThenThereShouldBeAnXInTheTopRightCorner()
        {
            registerPage.AssertCloseButtonPresent();
        }

        internal void WhenAcceptTheTermsAndConditions()
        {
            registerPage.AcceptTheTermsAndConditions();
        }

        internal void WhenClickRegister()
        {
            registerPage.ClickRegister();
        }

        internal void ThenRegisterButtonShouldBeGreyedOut()
        {
            registerPage.AssertRegisterBtnGreyedOut();
            registerPage.AssertRegisterPage();
        }

        internal void WhenClickTheTermsAndConditionsLink()
        {
            registerPage.NavigateToTermsAndConditionsPage();
        }

        internal void ThenShouldBeSentToTermsAndConditionsPage()
        {
            registerPage.AssertTermsAndConditionsPage();
        }

        internal void ThenErrorAsEmailInUse()
        {
            registerPage.AssertErrorEmailInUse();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorEmptyFirstName()
        {
            registerPage.AssertErrorEmptyFirstName();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorEmptyLastName()
        {
            registerPage.AssertErrorEmptyLastName();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorInvalidFirstName()
        {
            registerPage.AssertErrorInvalidFirstName();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorInvalidLastName()
        {
            registerPage.AssertErrorInvalidLastName();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorInvalidAccountNumber()
        {
            registerPage.AssertErrorInvalidAccountNumber();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorEmptyAccountNumber()
        {
            registerPage.AssertErrorEmptyAccountNumber();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorInvalidMPRN()
        {
            registerPage.AssertErrorInvalidMPRN();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorEmptyMPRN()
        {
            registerPage.AssertErrorEmptyMPRN();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorInvalidEmail()
        {
            registerPage.AssertErrorInvalidEmail();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorEmptyEmail()
        {
            registerPage.AssertErrorEmptyEmail();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorInvalidPhoneNumber()
        {
            registerPage.AssertErrorInvalidPhoneNumber();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorEmptyPhoneNumber()
        {
            registerPage.AssertErrorEmptyPhoneNumber();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorInvalidDOB()
        {
            registerPage.AssertErrorInvalidDOB();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorEmptyDOB()
        {
            registerPage.AssertErrorEmptyDOB();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorLongMPRN()
        {
            registerPage.AssertErrorLongMPRN();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorShortMPRN()
        {
            registerPage.AssertErrorShortMPRN();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorLongEmail()
        {
            registerPage.AssertErrorLongEmail();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorLongPhoneNumber()
        {
            registerPage.AssertErrorLongPhoneNumber();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorShortPhoneNumber()
        {
            registerPage.AssertErrorShortPhoneNumber();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorLongAccountNumber()
        {
            registerPage.AssertErrorLongAccountNumber();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorShortAccountNumber()
        {
            registerPage.AssertErrorShortAccountNumber();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorEmptyTermsAndConditions()
        {
            registerPage.AssertErrorEmptyTermsAndConditions();
            registerPage.AssertRegisterPage();
        }

        internal void ThenErrorEmptyAccountHolder()
        {
            registerPage.AssertErrorEmptyAccountHolder();
            registerPage.AssertRegisterPage();
        }

        internal void ThenSentToCheckInboxPage()
        {
            registerPage.AssertAccountCreated();
        }

        internal void ThenShouldBeSentToRegisterPage()
        {
            registerPage.AssertRegisterPage();
        }

        internal void WhenClickLogIntoSmartServices()
        {
            SmartHubPage smartHubPage = new SmartHubPage(shared.Driver.Value);
            smartHubPage.ClickLogIntoSmartServices();
        }

        internal void ThenRegisterPageText(string message)
        {
            registerPage.AssertRegisterPageText(message);
        }

        public void WhenClickPrivacyNoticeLink()
        {
            registerPage.ClickPrivacyNoticeLink();
        }
    }
}
