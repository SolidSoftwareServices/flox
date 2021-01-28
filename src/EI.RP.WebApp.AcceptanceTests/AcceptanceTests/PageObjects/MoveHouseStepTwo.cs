using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using OpenQA.Selenium;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class MoveHouseStepTwo
    {
        protected IWebDriver driver { get; set; }

        private SharedPageFunctions _sharedPageFunctions => new SharedPageFunctions(driver);

		//SharedVariables _sharedVariables => new SharedVariables();
		internal MoveHouseStepTwo(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal MoveHouseStepTwo(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal string
            stepTwoHeaderID = "step2Header",
            stepTwoSubHeader = "inputTitle",
            mprnFieldLabelID = "lblMRPN",
            gprnFieldLabelID = "lblGPRN",
            newMPRNFieldID = "mprn",
            newGPRNFieldID = "gprn",
            enterPRNCancelBtnId = "btnCancel",
            confirmAddressCancelBtnID = "new_mprn_gprn",
            confirmAdressHeaderID = "headerConfirm",
            confirmAddressSubHeaderID = "addressTitle",
            confirmAddressContinueBtnID = "btnContinue";


        internal void MoveHouseStepTwoEnterMPRNGPRN(IDictionary dict, SharedVariables sharedVariables)
        {
            sharedVariables.NewMPRN = dict["MPRN"];
            sharedVariables.NewGPRN = dict["GPRN"];
            EnterMPRN(sharedVariables);
            EnterGPRN(sharedVariables);
        }

        internal void EnterMPRN(SharedVariables sharedVariables)
        {
            _sharedPageFunctions.SendElementKeys(By.Id(newMPRNFieldID), sharedVariables.NewMPRN);
        }

        internal void EnterGPRN(SharedVariables sharedVariables)
        {
            _sharedPageFunctions.SendElementKeys(By.Id(newGPRNFieldID), sharedVariables.NewGPRN);
        }

        internal void ClickSubmitNewPRN(SharedVariables sharedVariables)
        {
            if (sharedVariables.ElecMeterMoveIn && sharedVariables.GasMeterMoveIn)
            {
                driver.ClickElementEx(By.XPath("//button[@id='submitNewPRNs' and contains(text(), 'Submit New MPRN/GPRN')]"));
            }
            if (sharedVariables.ElecMeterMoveIn && !(sharedVariables.GasMeterMoveIn))
            {
	            driver.ClickElementEx(By.XPath("//button[@id='submitNewPRNs' and contains(text(), 'Submit New MPRN')]"));
            }
            if (sharedVariables.GasMeterMoveIn && !(sharedVariables.ElecMeterMoveIn))
            {
	            driver.ClickElementEx(By.XPath("//button[@id='submitNewPRNs' and contains(text(), 'Submit New GPRN')]"));
            }
        }

        internal void ClickContinueOnConfirmAddressBtn()
        {
            driver.ClickElementEx(By.Id(confirmAddressContinueBtnID));
        }

        internal void AssertMoveHouseStepTwoEnterMPRNGPRNPage(SharedVariables sharedVariables)
        {
            AssertMoveHouseStep2ActiveTab(sharedVariables);
            AssertMoveHouseStepTwoFlowHeader(sharedVariables);
            AssertMoveHouseStepTwoSubHeader(sharedVariables);
            AssertMoveHouseStepTwoSubHeaderText();
            AssertMoveHouseStepTwoCancelBtn();
        }

        internal void AssertMoveHouseStepTwoConfirmNewAddressPage(SharedVariables sharedVariables)
        {
            AssertMoveHouseStepTwoConfirmAddressHeader();
            AssertMoveHouseStepTwoConfirmAddressSubHeader(sharedVariables);

            if (sharedVariables.ElecMeterMoveIn && sharedVariables.GasMeterMoveIn)
            {
                AssertMoveHouseStepTwoConfirmAddressMPRN(sharedVariables);
                AssertMoveHouseStepTwoConfirmAddressGPRN(sharedVariables);
            }
            if (sharedVariables.ElecMeterMoveIn && !(sharedVariables.GasMeterMoveIn))
            {
                AssertMoveHouseStepTwoConfirmAddressMPRN(sharedVariables);
            }
            if (sharedVariables.GasMeterMoveIn && !(sharedVariables.ElecMeterMoveIn))
            {
                AssertMoveHouseStepTwoConfirmAddressGPRN(sharedVariables);
            }

            AssertMoveHouseStepTwoConfirmAddressTextOfAddressPresent(sharedVariables);
            AssertMoveHouseStepTwoConfirmAddressReEnterBtn();
            AssertMoveHouseStepTwoConfirmAddressNeedHelp();
        }

        internal void AssertMoveHouseStep2ActiveTab(SharedVariables sharedVariables)
        {
	        _sharedPageFunctions.IsElementPresent(By.CssSelector("#step2.active"));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step1TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step3TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step4TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step5TabID));
        }

        internal void AssertMPRN()
        {
            AssertMoveHouseStepTwoMPRNFieldLabel();
            AssertMoveHouseStepTwoMPRNFieldToolTip();
            AssertMoveHouseStepTwoMPRNField();
        }

        internal void AssertGPRN()
        {
            AssertMoveHouseStepTwoGPRNFieldLabel();
            AssertMoveHouseStepTwoGPRNFieldToolTip();
            AssertMoveHouseStepTwoGPRNField();
        }

        internal void AssertMoveHouseStepTwoFlowHeader(SharedVariables sharedVariables)
        {
            if (sharedVariables.MoveElecOnly)
            {
                _sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(), 'Moving Electricity Account')]"));
            }
            if (sharedVariables.MoveGasOnly)
            {
                _sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(), 'Moving Gas Account')]"));
            }
            if (sharedVariables.MoveElecAndGas)
            {
                _sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(), 'Moving Electricity & Gas Accounts')]"));
            }
            if (sharedVariables.MoveElecAddGas)
            {
                _sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(), 'Moving Electricity & Adding Gas Account')]"));
            }
            if (sharedVariables.MoveGasAddElec)
            {
                _sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(), 'Moving Gas & Adding Electricity Account')]"));
            }
        }

        internal void AssertMoveHouseStepTwoSubHeader(SharedVariables sharedVariables)
        {
            if (sharedVariables.ElecMeterMoveIn && sharedVariables.GasMeterMoveIn)
            {
                _sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(), 'Enter your new MPRN and GPRN')]"));
                AssertMPRN();
                AssertGPRN();
            }
            if (sharedVariables.ElecMeterMoveIn && !(sharedVariables.GasMeterMoveIn))
            {
                _sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(), 'Enter your new MPRN')]"));
                AssertMPRN();
            }
            if (sharedVariables.GasMeterMoveIn && !(sharedVariables.ElecMeterMoveIn))
            {
                _sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(), 'Enter your new GPRN')]"));
                AssertGPRN();
            }
        }

        internal void AssertMoveHouseStepTwoSubHeaderText()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-2-input-subheader"));
        }

        internal void AssertMoveHouseStepTwoMPRNFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(mprnFieldLabelID));
        }

        internal void AssertMoveHouseStepTwoMPRNFieldToolTip()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#inputMPRN [data-toggle='tooltip']"));
        }

        internal void AssertMoveHouseStepTwoMPRNField()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(newMPRNFieldID));
        }

        internal void AssertMoveHouseStepTwoGPRNFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(gprnFieldLabelID));
        }

        internal void AssertMoveHouseStepTwoGPRNFieldToolTip()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#inputGPRN [data-toggle='tooltip']"));
        }

        internal void AssertMoveHouseStepTwoGPRNField()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(newGPRNFieldID));
        }

        internal void AssertMoveHouseStepTwoCancelBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(enterPRNCancelBtnId));
        }

        // Confirm Address PAge
        internal void AssertMoveHouseStepTwoConfirmAddressHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(confirmAdressHeaderID));
        }

        internal void AssertMoveHouseStepTwoConfirmAddressSubHeader(SharedVariables sharedVariables)
        {
            if (sharedVariables.ElecMeterMoveIn && sharedVariables.GasMeterMoveIn)
            {
                _sharedPageFunctions.IsElementPresent(By.XPath("//span[@id='addressTitle' and contains(text(), 'The following address matches the MPRN/GPRN you entered:')]"));
            }
            if (sharedVariables.ElecMeterMoveIn && !(sharedVariables.GasMeterMoveIn))
            {
                _sharedPageFunctions.IsElementPresent(By.XPath("//span[@id='addressTitle' and contains(text(), 'The following address matches the MPRN you entered:')]"));
            }
            if (sharedVariables.GasMeterMoveIn && !(sharedVariables.ElecMeterMoveIn))
            {
                _sharedPageFunctions.IsElementPresent(By.XPath("//span[@id='addressTitle' and contains(text(), 'The following address matches the GPRN you entered:')]"));
            }
        }

        internal void AssertMoveHouseStepTwoConfirmAddressMPRN(SharedVariables sharedVariables)
        {
            _sharedPageFunctions.IsElementPresent(By.XPath("//p[@data-testid='address-container-title-and-number']/strong[contains(text(), 'MPRN: " + sharedVariables.NewMPRN + "')]"));
        }

        internal void AssertMoveHouseStepTwoConfirmAddressGPRN(SharedVariables sharedVariables)
        {
            _sharedPageFunctions.IsElementPresent(By.XPath("//p[@data-testid='address-container-title-and-number']/strong[contains(text(), 'GPRN: " + sharedVariables.NewGPRN + "')]"));
        }

        internal void AssertMoveHouseStepTwoConfirmAddressTextOfAddressPresent(SharedVariables sharedVariables)
        {
            if (sharedVariables.ElecMeterMoveIn && sharedVariables.GasMeterMoveIn)
            {
	            _sharedPageFunctions.IsElementPresent(By.CssSelector("[data-testid='address-container']:nth-child(1) > [data-testid='address-container-address']"));
	            _sharedPageFunctions.IsElementPresent(By.CssSelector("[data-testid='address-container']:nth-child(2) > [data-testid='address-container-address']"));
            }
            else
            {
	            _sharedPageFunctions.IsElementPresent(By.CssSelector("[data-testid='address-container'] > [data-testid='address-container-address']"));
            }
        }

        internal void AssertMoveHouseStepTwoConfirmAddressReEnterBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(confirmAddressCancelBtnID));
        }

        internal void AssertMoveHouseStepTwoConfirmAddressNeedHelp()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-2-help-message"));
        }
    }
}
