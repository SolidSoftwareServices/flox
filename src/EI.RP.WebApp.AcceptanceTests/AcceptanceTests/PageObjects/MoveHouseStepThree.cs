using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using OpenQA.Selenium;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class MoveHouseStepThree
    {
        protected IWebDriver driver { get; set; }

        private SharedPageFunctions _sharedPageFunctions => new SharedPageFunctions(driver);
        internal MoveHouseStepThree(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal MoveHouseStepThree(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal string
            moveInDateCalendarID = "move_out",
            contactPhoneFieldLabelID = "lblContactNumber",
            contactPhoneFieldID = "step3ContactNumber",
            oldMPRNTextID = "pElectricityMeterReadingDescription",
            oldMPRNnumberID = "MprnHeader",
            twentyFourHrMeterReadID = "24Hr Meter reading",
            dayMeterReadID = "Day Meter reading",
            nightMeterReadID = "Night Meter reading",
            nshMeterReadID = "NSH Meter reading",
            gasHeaderID = "gasMeterReadingHeader",
            oldGPRNTextID = "pGasMeterReadingDescription",
            oldGPRNNumberID = "GprnHeader",
            gasMeterTypeID = "lblGasMeterType",
            nextPaymentOptionsBtnId = "btnNextPaymentOptions",
            cancelBtnID = "btnCancel";


        internal void PickDateFromCalendar()
        {
            _sharedPageFunctions.ClickElement(By.Id(moveInDateCalendarID));
            _sharedPageFunctions.ClickElement(By.CssSelector(".active.day"));
        }

        internal void MoveHouseStepThreeEnterData(IDictionary dict, SharedVariables sharedVariables)
        {

            string elecMeterRead1 = dict["ElecMeterRead1"];
            string elecMeterRead2 = dict["ElecMeterRead2"];
            string gasMeterRead = dict["GasMeterRead"];
            string contactPhone = dict["ContactPhone"];

            if (sharedVariables.ElecMeterMoveIn)
            {
                AssertMoveHouseStepThreeElecInfo();

                if (dict["24HrMeter"] == "Y")
                {
                    AssertMoveHouseStepThree24HrMeterReadFieldLabel();
                    _sharedPageFunctions.SendElementKeys(_sharedPageFunctions.ByDataAttribute(val: "24hr-meter-reading-input"), elecMeterRead1);
                }
                if (dict["DayMeter"] == "Y")
                {
                    AssertMoveHouseStepThreeDayMeterReadFieldLabel();
                    _sharedPageFunctions.SendElementKeys(_sharedPageFunctions.ByDataAttribute(val: "day-meter-reading-input"), elecMeterRead1);
                }
                if (dict["NightMeter"] == "Y")
                {
                    AssertMoveHouseStepThreeNightMeterReadFieldLabel();
                    _sharedPageFunctions.SendElementKeys(_sharedPageFunctions.ByDataAttribute(val: "night-meter-reading-input"), elecMeterRead2);
                }
                if (dict["NSHMeter"] == "Y")
                {
                    AssertMoveHouseStepThreeNSHMeterReadFieldLabel();
                    _sharedPageFunctions.SendElementKeys(_sharedPageFunctions.ByDataAttribute(val: "nsh-meter-reading-input"), elecMeterRead2);
                }
            }

            if (sharedVariables.GasMeterMoveIn)
            {
                AssertMoveHouseStepThreeGasInfo();
                _sharedPageFunctions.SendElementKeys(_sharedPageFunctions.ByDataAttribute(val: "gas-meter-reading-input"), gasMeterRead);
            }

            EnterContactPhone(contactPhone);
        }

        internal void EnterContactPhone(string contactPhone)
        {
            _sharedPageFunctions.SendElementKeys(By.Id(contactPhoneFieldID), contactPhone);
        }

        internal void ClickConfirmDetailsCheckbox()
        {
            IWebElement detailsCheckbox = driver.FindElementEx(By.XPath("//*[@id='details'][@type='checkbox']"));
            if (!detailsCheckbox.Selected)
            {
                IJavaScriptExecutor ex = (IJavaScriptExecutor)driver;
                ex.ExecuteScript("arguments[0].click();", detailsCheckbox);
            }
        }

        internal void ClickTermsAndConditionsCheckbox()
        {
            IWebElement termsAndConditionsCheckbox = driver.FindElementEx(By.XPath("//*[@id='terms'][@type='checkbox']"));
            if (!termsAndConditionsCheckbox.Selected)
            {
                IJavaScriptExecutor ex = (IJavaScriptExecutor)driver;
                ex.ExecuteScript("arguments[0].click();", termsAndConditionsCheckbox);
            }
        }

        internal void ClickNextPaymentOptionsBtn()
        {
			driver.ClickElementEx(By.Id(nextPaymentOptionsBtnId));
        }

        internal void AssertMoveHouseStepThreePage(SharedVariables sharedVariables)
        {
            AssertMoveHouseStepThreeFlowHeader();
            AssertMoveHouseStep3ActiveTab(sharedVariables);
            AssertMoveHouseStepThreeHeader();
            AssertMoveHouseStepThreeDenoteFields();
            AssertMoveHouseStepThreeDateMoveInGreenText();
            AssertMoveHouseStepThreeDateMoveInToolTip();
            AssertMoveHouseStepThreeDateMoveInText();
            AssertMoveHouseStepThreeContactPhoneFieldLabel();
            AssertMoveHouseStepThreeContactPhoneFieldToolTip();
            AssertMoveHouseStepThreeContactPhoneField();
            //AssertMoveHouseStepThreeContactPhoneFieldSubtext(); Might be added in new UI currently only displays in old app
            AssertMoveHouseStepThreeCancelBtn();
        }

        internal void AssertMoveHouseStepThreeElecInfo()
        {
            AssertMoveHouseStepThreeNewPropertyElecHeader();
            AssertMoveHouseStepThreeNewPropertyMPRN();
            AssertMoveHouseStepThreeNewPropertyElecText();
        }

        internal void AssertMoveHouseStepThreeGasInfo()
        {
            AssertMoveHouseStepThreeNewPropertyGasHeader();
            AssertMoveHouseStepThreeNewPropertyGPRN();
            AssertMoveHouseStepThreeNewPropertyGasText();
            AssertMoveHouseStepThreeNewPropertyGasMeterReadFieldLabel();
        }

        internal void AssertMoveHouseStep3ActiveTab(SharedVariables sharedVariables)
        {
	        _sharedPageFunctions.IsElementPresent(By.CssSelector("#step3.active"));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step1TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step2TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step4TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step5TabID));
        }

        internal void AssertMoveHouseStepThreeFlowHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id("step3Header"));
        }

        internal void AssertMoveHouseStepThreeHeader()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-3-title"));
        }

        internal void AssertMoveHouseStepThreeDenoteFields()
        {
	        _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-3-info"));
        }

        internal void AssertMoveHouseStepThreeDateMoveInGreenText()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#moveOutDate > label"));
        }

        internal void AssertMoveHouseStepThreeDateMoveInToolTip()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#moveOutDate > label > [data-toggle='tooltip']"));
        }

        internal void AssertMoveHouseStepThreeDateMoveInText()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#moveOutDate > p"));
        }

        internal void AssertMoveHouseStepThreeContactPhoneFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(contactPhoneFieldLabelID));
        }

        internal void AssertMoveHouseStepThreeContactPhoneFieldToolTip()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector($"#{contactPhoneFieldLabelID}  > [data-toggle='tooltip']"));
        }

        internal void AssertMoveHouseStepThreeContactPhoneField()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(contactPhoneFieldID));
        }

        internal void AssertMoveHouseStepThreeContactPhoneFieldSubtext()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#new_property_details > div:nth-child(4) > span"));
        }

        internal void AssertMoveHouseStepThreeNewPropertyElecHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(), 'Electricity')]"));
        }

        internal void AssertMoveHouseStepThreeNewPropertyMPRN()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(oldMPRNnumberID));
        }

        internal void AssertMoveHouseStepThreeNewPropertyElecText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(oldMPRNTextID));
        }

        internal void AssertMoveHouseStepThree24HrMeterReadFieldLabel()
        {
	        _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "24hr-meter-reading-label"));
        }

        internal void AssertMoveHouseStepThreeDayMeterReadFieldLabel()
        {
	        _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "day-meter-reading-label"));
        }

        internal void AssertMoveHouseStepThreeNightMeterReadFieldLabel()
        {
	        _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "night-meter-reading-label"));
        }

        internal void AssertMoveHouseStepThreeNSHMeterReadFieldLabel()
        {
	        _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "nsh-meter-reading-label"));
        }

        internal void AssertMoveHouseStepThreeNewPropertyGasHeader()
        {
	        _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "gas-meter-reading-label"));
        }

        internal void AssertMoveHouseStepThreeNewPropertyGPRN()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(oldGPRNNumberID));
        }

        internal void AssertMoveHouseStepThreeNewPropertyGasText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(oldGPRNTextID));
        }

        internal void AssertMoveHouseStepThreeNewPropertyGasMeterReadFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(gasMeterTypeID));
        }

        internal void AssertMoveHouseStepThreeCancelBtn()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(cancelBtnID));
        }
    }
}
