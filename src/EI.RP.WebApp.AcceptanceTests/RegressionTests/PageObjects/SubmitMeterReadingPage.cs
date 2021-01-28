using OpenQA.Selenium;
using System;
using System.Net.Mime;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using types = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.AccountTypes.types;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
	class SubmitMeterReadingPage
	{
		protected IWebDriver driver { get; set; }
		internal SubmitMeterReadingPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal SubmitMeterReadingPage(IWebDriver driver0)
		{
			driver = driver0;
		}
		SharedPageFunctions func => new SharedPageFunctions(driver);

		internal void AssertMeterReadingHistoryHeaders()
		{
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.histMeterType), TextMatch.SubmitMeterReadingPage.histMeterType);
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.histFromDate), TextMatch.SubmitMeterReadingPage.histFromDate);
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.histToDate), TextMatch.SubmitMeterReadingPage.histToDate);
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.histReading), TextMatch.SubmitMeterReadingPage.histReading);
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.histType), TextMatch.SubmitMeterReadingPage.histType);
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.histConsumption), TextMatch.SubmitMeterReadingPage.histConsumption);
		}

		internal void AssertElecText()
		{
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.kWh), TextMatch.SubmitMeterReadingPage.kWh);
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.elecMeterInputLabel), TextMatch.SubmitMeterReadingPage.elecMeterInputLabel);
		}

		internal void AssertDayNightInputFields()
		{
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.kWh), TextMatch.SubmitMeterReadingPage.kWh);
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.dayMeterLabel), TextMatch.SubmitMeterReadingPage.dayMeterLabel);
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.nightMeterLabel), TextMatch.SubmitMeterReadingPage.nightMeterLabel);
		}

		internal void AssertGasText()
		{
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.kWh), TextMatch.SubmitMeterReadingPage.m3);
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.gasMeterInputLabel), TextMatch.SubmitMeterReadingPage.gasMeterInputLabel);
		}

		internal void AssertMPRNAndMeterInfo(string mprn)
		{
			func.IsElementTextPresent(mprn);
		}

		internal void AssertSubmitYourMeterReadingText()
		{
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.submitHeader), TextMatch.SubmitMeterReadingPage.submitHeader);
		}

        internal void AssertDayNightNoNetworkRejectReasonDisplayed()
        {
            func.IsElementTextPresent(By.XPath(XPathSelectors.MeterReadingResultPage.rejectReason), TextMatch.MeterReadingResultPage.rejectReasonLessThan);
        }

        internal void EnterMeterReadingInput(string input, Enum type)
		{
			if (type.Equals(types.DayNight))
			{
				func.SendElementKeys(By.Id(IdentifierSelector.MeterReadingPage.meterReadingInputDayID), input);
				func.SendElementKeys(By.Id(IdentifierSelector.MeterReadingPage.meterReadingInputNightID), input);

			}
			else {
				func.SendElementKeys(By.Id(IdentifierSelector.MeterReadingPage.meterReadingInputID), input);
			}
		}
        
		internal void ClickSubmitMeterReadingBtn()
		{
			func.ClickElement(func.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.ButtonSubmitMeterReadingTestId));
		}       

		internal void AssertModalDisplayed(Enum type)
		{			
			func.ClickElement(func.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.LinkHowDoIRead));

			switch (type){
				case types.DayNight:
					AssertDayNightModalText();
					break;
				case types.Electricity:
					AssertElecModalText();
					break;
				case types.Gas:
					AssertGasModalText();
					break;
			}
        }

		internal void AssertError(Enum type)
		{
            switch (type)
            {
                case types.DayNight:
                    func.IsElementTextPresent(TextMatch.SubmitMeterReadingPage.emptyError);
                    break;
                case types.Electricity:
                    func.IsElementTextPresent(TextMatch.SubmitMeterReadingPage.emptyError);
                    break;
                case types.Gas:
                    func.IsElementTextPresent(TextMatch.SubmitMeterReadingPage.emptyError);
                    break;
            }   
        }

        internal void AssertElecModalText()
		{
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.modalElecMeterHeader), TextMatch.SubmitMeterReadingPage.modalElecMeterHeader);
		}
		internal void AssertGasModalText()
		{
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.modalGasReadingHeader), TextMatch.SubmitMeterReadingPage.modalGasReadingHeader);
		}
		internal void AssertDayNightModalText()
		{
			func.IsElementTextPresent(By.XPath(XPathSelectors.SubmitMeterReadingPage.modalDayNightMeterHeader), TextMatch.SubmitMeterReadingPage.modalDayNightMeterHeader);
		}


		internal void AssertMeterReadingHeader()
		{
			func.IsElementTextPresent(func.ByDataAttribute("testid", "meter-reading-title"), "Meter Reading");
		}

		internal void AssertMeterReadingPage(Enum type)
		{			
			func.IsElementPresent(func.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.ButtonSubmitMeterReadingTestId));
			AssertModalDisplayed(type);
			AssertMeterReadingHeader();
			AssertMeterReadingHistoryDisplayed();
		}


		private void AssertMeterReadingHistoryDisplayed()
		{
			AssertMeterReadingHistoryHeaders();   
		}
        
		internal void AssertDayNightRejectReasonDisplayed()
		{
			
			func.IsElementTextPresent(By.XPath(XPathSelectors.MeterReadingResultPage.rejectReason), TextMatch.MeterReadingResultPage.rejectReason2);
		}
		internal void AssertElecRejectReasonDisplayed()
		{

			func.IsElementTextPresent(By.XPath(XPathSelectors.MeterReadingResultPage.rejectReason), TextMatch.MeterReadingResultPage.rejectReason);
		}

		internal void AssertDayNightAcceptReasonDisplayed()
		{

			func.IsElementTextPresent(By.XPath(XPathSelectors.MeterReadingResultPage.acceptReason), TextMatch.MeterReadingResultPage.acceptReason);
		}
		internal void AssertElecAcceptReasonDisplayed()
		{

			func.IsElementTextPresent(By.XPath(XPathSelectors.MeterReadingResultPage.acceptReason), TextMatch.MeterReadingResultPage.acceptReason);
		}
		internal void AssertGasAcceptReasonDisplayed()
		{

			func.IsElementTextPresent(By.XPath(XPathSelectors.MeterReadingResultPage.acceptReason), TextMatch.MeterReadingResultPage.acceptReason);
		}
	}
}
