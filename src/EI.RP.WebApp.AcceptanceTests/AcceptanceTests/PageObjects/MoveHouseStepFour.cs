using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class MoveHouseStepFour
    {
        WebDriverWait wait;

        protected IWebDriver driver { get; set; }

        private SharedPageFunctions _sharedPageFunctions => new SharedPageFunctions(driver);
        internal MoveHouseStepFour(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal MoveHouseStepFour(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal string
            stepFourHeaderID = "step4Header",
            choosePaymentHeaderID = "choosePaymentOptions",
            manualBtnID = "btnManualPayments",
            canUseDDCheckboxID = "lblUseSameSetupForAllAccounts",
            setUpNewDDBtnID = "setUpNewDirectDebit",
            choosePaymentScreenCancelBtnID = "btnCancel",
            directDebitSubtextID = "directDebitContent",
            ibanFieldID = "iban",
            nameFieldID = "customer-name",
            whatIsMyIBANID = "btnDirectDebitIban",
            skipDDBtnID = "skip_dd_btn",
            confirmPopUpBtnID = "manualPaymentConfirmButton";

        internal void ClickUseDDForBothCheckbox()
        {
            //For Future Regression Pack
            if (true)
            {
                _sharedPageFunctions.ClickElement(By.Id("lblUseSameSetupForAllAccounts"));
            }
        }

        internal void ClickSetUpNewDDBtn()
        {
            _sharedPageFunctions.ClickElement(By.Id(setUpNewDDBtnID));
        }

        internal void EnterDDElecDetails(IDictionary dict)
        {
            string iban = dict["IBAN"];
            string name = dict["Name"];
            EnterIBAN(iban);
            EnterName(name);
        }

        internal void EnterIBAN(string iban)
        {
            _sharedPageFunctions.SendElementKeys(By.Id(ibanFieldID), iban);
        }

        internal void EnterName(string name)
        {
            _sharedPageFunctions.SendElementKeys(By.Id(nameFieldID), name);
        }

        internal void ClickAuthorizeCheckboxElec()
        {
            _sharedPageFunctions.ClickElement(By.Id("terms-label"));
        }

        internal void ContinueToGasDDSetupBtn()
        {
            _sharedPageFunctions.ClickElement(By.CssSelector("#directdebit > button"));
        }

        internal void ClickSkipSetupGasDDBtn()
        {
            driver.ClickElementEx(By.Id(skipDDBtnID));
        }

        internal void ClickYesImSurePopUpBtn()
        {
            driver.ClickElementEx(By.Id(confirmPopUpBtnID));
        }

        internal void AssertMoveHouseStepFourChoosePaymentOptionPage(SharedVariables sharedVariables)
        {
            AssertMoveHouseStepFourFlowHeader();
            AssertMoveHouseStep4ActiveTab(sharedVariables);
            AssertMoveHouseStepFourChoosePaymentHeader();
            AssertMoveHouseStepFourSetUpNewDDHeader();
            AssertMoveHouseStepFourSetUpNewDDText();
            AssertMoveHouseStepFourManualHeader();
            AssertMoveHouseStepFourManualText();
            AssertMoveHouseStepFourManualBtn();
            AssertMoveHouseStepFourCancelBtn();
        }

        internal void AssertMoveHouseStepFourElecDDSetupPage()
        {
            AssertMoveHouseStepFourElecDDSetupPageHeader();
            AssertMoveHouseStepFourElecDDSetupPageText();
            AssertMoveHouseStepFourElecDDSetupPageIbanFieldLabel();
            AssertMoveHouseStepFourElecDDSetupPageNameFieldLabel();
            AssertMoveHouseStepFourDisclaimerTextElec();
            AssertMoveHouseStepFourSkipElecDDSetUpBtn();
        }

        internal void AssertMoveHouseStepFourGasDDSetupPage()
        {
            AssertMoveHouseStepFourSetupGasDDHeader();
            AssertMoveHouseStepFourSetupGasDDText();
            AssertMoveHouseStepFourGasDDSetupPageIBANFieldLabel();
            AssertMoveHouseStepFourGasDDSetupPageIBANField();
            AssertMoveHouseStepFourGasDDSetupPageNameFieldLabel();
            AssertMoveHouseStepFourGasDDSetupPageNameField();
            AssertMoveHouseStepFourDisclaimerTextGas();
            AssertMoveHouseStepFourComeplteSetupGasDDBtn();
        }

        internal void AssertMoveHouseStep4ActiveTab(SharedVariables sharedVariables)
        {
	        _sharedPageFunctions.IsElementPresent(By.CssSelector("#step4.active"));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step1TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step2TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step3TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step5TabID));
        }

        internal void AssertMoveHouseStepFourFlowHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(stepFourHeaderID));
        }

        internal void AssertMoveHouseStepFourChoosePaymentHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(choosePaymentHeaderID));
        }

        internal void AssertMoveHouseStepFourSetUpNewDDHeader()
        {
            _sharedPageFunctions
                .IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "payment-option-new-dd-title"));
        }

        internal void AssertMoveHouseStepFourSetUpNewDDText()
        {
	        _sharedPageFunctions
		        .IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "payment-option-new-dd-text"));
        }

        internal void AssertMoveHouseStepFourManualHeader()
        {
	        _sharedPageFunctions
		        .IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "payment-option-manual-title"));
        }

        internal void AssertMoveHouseStepFourManualText()
        {
	        _sharedPageFunctions
		        .IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "payment-option-manual-text"));
        }

        internal void AssertMoveHouseStepFourManualBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(manualBtnID));
        }

        internal void AssertMoveHouseStepFourCancelBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(choosePaymentScreenCancelBtnID));
        }

        internal void AssertMoveHouseStepFourElecDDSetupPageHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#content > h1"));
        }

        internal void AssertMoveHouseStepFourElecDDSetupPageText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(directDebitSubtextID));
        }

        internal void AssertMoveHouseStepFourElecDDSetupPageIbanFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#inputIban > label"));
        }

        internal void AssertMoveHouseStepFourElecDDSetupPageNameFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#inputName > label"));
        }

        internal void AssertMoveHouseStepFourDisclaimerTextElec()
        {
            _sharedPageFunctions
                .IsElementPresent(By.CssSelector("#dds_mandate > small"));
            _sharedPageFunctions
                .IsElementPresent(By.CssSelector("#content > p:nth-child(5) > small"));
        }

        internal void AssertMoveHouseStepFourSkipElecDDSetUpBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(skipDDBtnID));
        }

        internal void AssertMoveHouseStepFourSetupGasDDHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id("#content > h1"));
        }

        internal void AssertMoveHouseStepFourSetupGasDDText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(directDebitSubtextID));
        }

        internal void AssertMoveHouseStepFourGasDDSetupPageIBANFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#inputIban > label"));
        }

        internal void AssertMoveHouseStepFourGasDDSetupPageIBANField()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(ibanFieldID));
        }

        internal void AssertMoveHouseStepFourGasDDSetupPageNameFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#inputName > label"));
        }

        internal void AssertMoveHouseStepFourGasDDSetupPageNameField()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(nameFieldID));
        }

        internal void AssertMoveHouseStepFourDisclaimerTextGas()
        {
            _sharedPageFunctions
                .IsElementPresent(By.CssSelector("#dds_mandate > small"));
            _sharedPageFunctions
                .IsElementPresent(By.CssSelector("#content > p:nth-child(5) > small"));
        }

        internal void AssertMoveHouseStepFourComeplteSetupGasDDBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#directdebit > button"));
        }
    }
}
