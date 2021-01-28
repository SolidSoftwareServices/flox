using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using OpenQA.Selenium;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class MoveHouseStepOne
    {
        protected IWebDriver driver { get; set; }

        private SharedPageFunctions _sharedPageFunctions => new SharedPageFunctions(driver);
        internal MoveHouseStepOne(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal MoveHouseStepOne(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal string
            stepOneFlowHeaderID = "step1Header",
            moveOutCalendarFieldID = "move_out",
            oldMPRNnumberID = "MprnHeader",
            oldMPRNTextID = "pElectricityMeterReadingDescription",
            oldGPRNNumberID = "GprnHeader",
            oldGPRNTextID = "pGasMeterReadingDescription",
            radioBtnYesID = "incomingOccupantYes",
            radioBtnNoID = "incomingOccupantNo",
            gasMeterReadID = "lblGasMeterType",
            howToReadMyMeterBtnID = "btnMeterreadingHowToRead",
            lettingAgentNameFieldID = "LettingAgentName",
            lettingAgentPhoneNumberFieldID = "LettingPhoneNumber",
            permissionsCheckBoxID = "incomingOccupantCheckedBox",
            nextMPRNGPRNBtnID = "btnNextPRN";


        internal void PickDateFromCalendar()
        {
            _sharedPageFunctions.ClickElement(By.Id(moveOutCalendarFieldID));
            _sharedPageFunctions.ClickElement(By.CssSelector(".today.day"));
        }

        internal void MoveHouseStepOneEnterMPRNReads(IDictionary dict, SharedVariables sharedVariables)
        {
            string elecMeterRead1 = dict["ElecMeterRead1"];
            string elecMeterRead2 = dict["ElecMeterRead2"];
            string gasMeterRead = dict["GasMeterRead"];
            string name = dict["Name"];
            string phone = dict["Phone"];

            if (sharedVariables.ElecMeterMoveOut)
            {
                AssertMoveHouseStepOneElecInfo();

                if (dict["24HrMeter"] == "Y")
                {
                    AssertMoveHouseStepOnePreviousProperty24HrMeterReadFieldLabel();
                    _sharedPageFunctions.SendElementKeys(_sharedPageFunctions.ByDataAttribute(val: "24hr-meter-reading-input"), elecMeterRead1);
                }
                if (dict["DayMeter"] == "Y")
                {
                    AssertMoveHouseStepOnePreviousPropertyDayMeterReadFieldLabel();
                    _sharedPageFunctions.SendElementKeys(_sharedPageFunctions.ByDataAttribute(val: "day-meter-reading-input"), elecMeterRead1);
                }
                if (dict["NightMeter"] == "Y")
                {
                    AssertMoveHouseStepOnePreviousPropertyNightMeterReadFieldLabel();
                    _sharedPageFunctions.SendElementKeys(_sharedPageFunctions.ByDataAttribute(val: "night-meter-reading-input"), elecMeterRead2);
                }
                if (dict["NSHMeter"] == "Y")
                {
                    AssertMoveHouseStepOnePreviousPropertyNSHMeterReadFieldLabel();
                    _sharedPageFunctions.SendElementKeys(_sharedPageFunctions.ByDataAttribute(val: "nsh-meter-reading-input"), elecMeterRead2);
                }
            }

            if (sharedVariables.GasMeterMoveOut)
            {
                AssertMoveHouseStepOneGasInfo();
                _sharedPageFunctions.SendElementKeys(_sharedPageFunctions.ByDataAttribute(val: "gas-meter-reading-input"), gasMeterRead);
            }
        
            EnterName(name);
            EnterPhone(phone);
        }

        internal void EnterName(string name)
        {
            _sharedPageFunctions.SendElementKeys(By.Id(lettingAgentNameFieldID), name);
        }

        internal void EnterPhone(string phone)
        {
            _sharedPageFunctions.SendElementKeys(By.Id(lettingAgentPhoneNumberFieldID), phone);
        }

        internal void ClickRadioYesBtn()
        {
            Thread.Sleep(1000);
            var radioBtnYes = driver.FindElementEx(By.XPath("//*[@id='incomingOccupantYes'][@type='radio']"));
            if (!radioBtnYes.Selected)
            {
				//jack you should make the following generic
	            try
	            {
		        
		            radioBtnYes.Click();
	            }
	            catch(ElementClickInterceptedException ex)
	            {
		            ((IJavaScriptExecutor)driver).ExecuteScript($"document.getElementById('incomingOccupantYes').click();");
	            }

	            
            }
        }

        internal void ClickConfirmPermissionCheckbox()
        {
            IWebElement permissionsCheckbox = driver.FindElementEx(By.XPath("//*[@id='occupier'][@type='checkbox']"));
            if (!permissionsCheckbox.Selected)
            {
                IJavaScriptExecutor ex = (IJavaScriptExecutor)driver;
                ex.ExecuteScript("arguments[0].click();", permissionsCheckbox);
            }
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

        internal void ClickNextNewMPRNGPRNBtn()
        {
            _sharedPageFunctions.ClickElement(By.Id(nextMPRNGPRNBtnID));
        }

        internal void AssertMoveHouseStepOnePage(SharedVariables sharedVariables)
        {
            AssertMoveHouseStep1ActiveTab(sharedVariables);
            AssertMoveHouseStepOneFlowHeader();
            AssertMoveHouseStepOneHeader();
            AssertMoveHouseStepOneDenoteFields();
            AssertMoveHouseStepOneDateMoveOut();
            AssertMoveHouseStepOneDateMoveOutToolTip();
            AssertMoveHouseStepOneDateMoveOutText();
            AssertMoveHouseStepOnePreviousPropertyHowToReadMeterBtn();
            AssertMoveHouseStepOnePreviousPropertyRadioBtnTopText();
            AssertMoveHouseStepOnePreviousOccupantRadioBtnYes();
            AssertMoveHouseStepOnePreviousOccupantRadioBtnNo();
            AssertMoveHouseStepOneCancelBtn();
        }

        internal void AssertMoveHouseStepOneNewOccupantContent()
        {
            AssertMoveHouseStepOneNewOccupantNameFieldHeaderText();
            AssertMoveHouseStepOneNewOccupantNameFieldToolTip();
            AssertMoveHouseStepOneNewOccupantNameField();
            AssertMoveHouseStepOneNewOccupantPhoneNumberFieldHeaderText();
            AssertMoveHouseStepOneNewOccupantPhoneNumberFieldToolTip();
            AssertMoveHouseStepOneNewOccupantPhoneNumberField();
        }

        internal void AssertMoveHouseStepOneElecInfo()
        {
            AssertMoveHouseStepOnePreviousPropertyElecHeader();
            AssertMoveHouseStepOnePreviousPropertyMPRNText();
            AssertMoveHouseStepOnePreviousPropertyMPRN();
        }

        internal void AssertMoveHouseStepOneGasInfo()
        {
            AssertMoveHouseStepOnePreviousPropertyGasHeader();
            AssertMoveHouseStepOnePreviousPropertyGPRNText();
            AssertMoveHouseStepOnePreviousPropertyGPRN();
            AssertMoveHouseStepOnePreviousPropertyGasMeterReadFieldLabel();
        }

        internal void AssertMoveHouseStep1ActiveTab(SharedVariables sharedVariables)
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#step1.active"));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step2TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step3TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step4TabID));
            _sharedPageFunctions.IsElementPresent(By.Id(sharedVariables.step5TabID));
        }

        internal void AssertMoveHouseStepOneFlowHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(stepOneFlowHeaderID));
        }

        internal void AssertMoveHouseStepOneHeader()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-1-title"));
        }

        internal void AssertMoveHouseStepOneDenoteFields()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-1-info"));
        }

        internal void AssertMoveHouseStepOneDateMoveOut()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#moveOutDate > label"));
        }

        internal void AssertMoveHouseStepOneDateMoveOutToolTip()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#moveOutDate > label > [data-toggle='tooltip']"));
        }

        internal void AssertMoveHouseStepOneDateMoveOutText()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#moveOutDate > p"));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyElecHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(), 'Electricity')]"));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyMPRNText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(oldMPRNTextID));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyMPRN()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(oldMPRNnumberID));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyGasHeader()
        {
            _sharedPageFunctions.IsElementPresent(By.XPath("//h2[contains(text(), 'Gas')]"));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyGPRNText()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(oldGPRNTextID));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyGPRN()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(oldMPRNnumberID));
        }

        internal void AssertMoveHouseStepOnePreviousProperty24HrMeterReadFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "24hr-meter-reading-label"));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyDayMeterReadFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "day-meter-reading-label"));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyNightMeterReadFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "night-meter-reading-label"));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyNSHMeterReadFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "nsh-meter-reading-label"));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyGasMeterReadFieldLabel()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "gas-meter-reading-label"));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyHowToReadMeterBtn()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-how-do-i-read-my-meter"));
        }

        internal void AssertMoveHouseStepOnePreviousPropertyRadioBtnTopText()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute(val: "mimo-do-you-know-incoming-occupant"));
        }

        internal void AssertMoveHouseStepOnePreviousOccupantRadioBtnYes()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(radioBtnYesID));
        }

        internal void AssertMoveHouseStepOnePreviousOccupantRadioBtnNo()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(radioBtnNoID));
        }

        internal void AssertMoveHouseStepOneCancelBtn()
        {
            _sharedPageFunctions.IsElementPresent(_sharedPageFunctions.ByDataAttribute("target", "#modal-cancel-mimo"));
        }

        internal void AssertMoveHouseStepOneNewOccupantNameFieldHeaderText()
        {
            _sharedPageFunctions.IsElementPresent(By.XPath("//*[@id='incomingOccupantName']/label"));
        }

        internal void AssertMoveHouseStepOneNewOccupantNameFieldToolTip()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#incomingOccupantName > label > [data-toggle='tooltip']"));
        }

        internal void AssertMoveHouseStepOneNewOccupantNameField()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(lettingAgentNameFieldID));
        }

        internal void AssertMoveHouseStepOneNewOccupantPhoneNumberFieldHeaderText()
        {
	        _sharedPageFunctions.IsElementPresent(By.XPath("//*[@id='incomingOccupantNumber']/label"));
        }

        internal void AssertMoveHouseStepOneNewOccupantPhoneNumberFieldToolTip()
        {
            _sharedPageFunctions.IsElementPresent(By.CssSelector("#incomingOccupantNumber > label > [data-toggle='tooltip']"));
        }

        internal void AssertMoveHouseStepOneNewOccupantPhoneNumberField()
        {
            _sharedPageFunctions.IsElementPresent(By.Id(lettingAgentPhoneNumberFieldID));
        }
    }
}
