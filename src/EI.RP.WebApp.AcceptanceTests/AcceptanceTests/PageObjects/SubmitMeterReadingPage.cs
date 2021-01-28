using System;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class SubmitMeterReadingPage
    {
        protected IWebDriver driver { get; set; }
        internal SubmitMeterReadingPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }

		internal SubmitMeterReadingPage(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal SharedPageFunctions page => new SharedPageFunctions(driver);

        internal void AssertBackToAccountOverviewBtnDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.ButtonBackTestId));
        }

        internal void ClickBackToAccountOverviewBtn()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.ButtonBackTestId));
        }

        internal void EnterMeterReadingInput(string input)
        {
            var el = new WebDriverWait(driver, TimeSpan.FromSeconds(180))
                .Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                        page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.InputValueATestId)));

            page.SendElementKeys(el, input);

			if (el.GetAttribute("value") != input)
			{
				EnterValidMeter(input);
			}
			
		}

        internal void CheckMeterReadingInputValue(string input)
        {
            var el = new WebDriverWait(driver, TimeSpan.FromSeconds(180))
                .Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                        page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.InputValueATestId)));

            Assert.IsTrue(el.GetAttribute("value") == input);
        }

        internal void AssertConfirmationScreenHeader(string header)
        {
            var by = page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.TitleTestId);
            page.IsElementPresent(by);
            Assert.IsTrue(driver.FindElementEx(by).Text == header);
        }

        internal void EnterValidMeter(string validMeter)
        {
            page.SendElementKeys(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.InputValueATestId), validMeter);

			var el = new WebDriverWait(driver, TimeSpan.FromSeconds(15))
				.Until(
					SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.InputValueATestId)));

			if(el.GetAttribute("value") != validMeter)
			{
				EnterValidMeter(validMeter);
			}
			else
			{
				return;
			}		
		}

        internal void ClickHowDoIReadMyMeterBtn()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.LinkHowDoIRead));
        }

        internal void AssertHowDoIReadMyMeterModalDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.ModalHowDoIReadGas));
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.ModalHowDoIReadDayNight));
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.ModalHowDoIRead24H));
        }

        internal void AssertMeterReadingInputFeildPopulatedAs(string p0)
        {
            page.IsElementPopulated(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.InputValueATestId), p0);
        }

        internal void AssertErrorSpecialCharacters()
        {
            throw new NotImplementedException();
        }

        internal void AssertErrorAlphabeticalCharacters()
        {
            throw new NotImplementedException();
        }

        internal void ClickSubmitMeterReadingBtn()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.ButtonSubmitMeterReadingTestId));
        }

        internal void AssertErrorMeterReadingError()
        {
            throw new NotImplementedException();
        }

        internal void AssertSubmitMeterReadingBtn()
        {
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.ButtonSubmitMeterReadingTestId));
        }

        internal void AssertMeterReadingHeader()
        {
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.TitleTestId));
        }

        internal void AssertAcceptedPageDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute("page", "meter-reading-success"));
        }

        internal void AssertAcceptedScreenHeaderDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.TitleSuccessTestId));
        }

        internal void AssertMeterReadingPage()
        {
            page.IsElementPresent(page.ByDataAttribute("page", "meter-reading"));
            AssertMeterReadingHeader();
            AssertSubmitMeterReadingBtn();
            AssertMeterReadingHistoryDisplayed();
        }

        private void AssertMeterReadingHistoryDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.TableHistoryTestId));
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.TableHistoryMobileTestId));
        }

        internal void AssertRejectPageDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute("page", "meter-reading-error"));
        }

        internal void AssertRejectedScreenHeaderDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.TitleErrorTestId));
        }

        internal void AssertRejectReasonDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.MeterReadingPage.MessageErrorTestId));
        }
    }
}
