using NUnit.Framework;
using OpenQA.Selenium;
using System;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace EI.RP.WebApp.AcceptanceTests.PageObjects
{
    class SubmitMeterReadingPage
    {
        protected IWebDriver driver { get; set; }

        internal SubmitMeterReadingPage(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal SharedPageFunctions page => new SharedPageFunctions(driver);

        class Selectors
        {
            public string PageId { get; set; }
            public string TitleTestId { get; set; }
            public string TitleSuccessTestId { get; set; }
            public string TitleErrorTestId { get; set; }
            public string MessageErrorTestId { get; set; }
            public string InputValueATestId { get; set; }
            public string InputValueBTestId { get; set; }
            public string MessageHowDoIRead { get; set; }
            public string LinkHowDoIRead { get; set; }
            public string ModalHowDoIReadGas { get; set; }
            public string ModalHowDoIReadDayNight { get; set; }
            public string ModalHowDoIRead24H { get; set; }
            public string ButtonSubmitMeterReadingTestId { get; set; }
            public string TitleHistoryTestId { get; set; }
            public string TableHistoryTestId { get; set; }
            public string TableHistoryMobileTestId { get; set; }
            public string ButtonBackTestId { get; set; }
        }

        readonly Selectors selectors = new Selectors()
        {
            TitleTestId = "meter-reading-title",
            TitleSuccessTestId = "meter-reading-success-title",
            TitleErrorTestId = "meter-reading-error-title",
            MessageErrorTestId = "meter-reading-error-message",
            InputValueATestId = "reading-value-a-input",
            InputValueBTestId = "reading-value-a-input",
            MessageHowDoIRead = "meter-reading-how-do-i-read-message",
            LinkHowDoIRead = "meter-reading-how-do-i-read-message-link",
            ModalHowDoIReadGas = "meter-reading-how-do-i-read-gas-modal",
            ModalHowDoIReadDayNight = "meter-reading-how-do-i-read-day-night-modal",
            ModalHowDoIRead24H = "meter-reading-how-do-i-read-24h-modal",
            ButtonSubmitMeterReadingTestId = "submit-meter-reading-button",
            TitleHistoryTestId = "meter-reading-history-title",
            TableHistoryTestId = "meter-reading-history-table",
            TableHistoryMobileTestId = "meter-reading-history-table-mobile",
            ButtonBackTestId = "meter-reading-back"
        };

        internal void AssertBackToAccountOverviewBtnDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.ButtonBackTestId));
        }

        internal void ClickBackToAccountOverviewBtn()
        {
            page.ClickElement(page.ByDataAttribute(val: selectors.ButtonBackTestId));
        }

        internal void EnterMeterReadingInput(string input)
        {
            var el = new WebDriverWait(driver, TimeSpan.FromSeconds(60))
                .Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                        page.ByDataAttribute(val: selectors.InputValueATestId)));

            page.SendElementKeys(el, input);

			if (el.GetAttribute("value") != input)
			{
				EnterValidMeter(input);
			}
			else
			{
				return;
			}
		}

        internal void CheckMeterReadingInputValue(string input)
        {
            var el = new WebDriverWait(driver, TimeSpan.FromSeconds(60))
                .Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                        page.ByDataAttribute(val: selectors.InputValueATestId)));

            Assert.IsTrue(el.GetAttribute("value") == input);
        }

        internal void AssertConfirmationScreenHeader(string header)
        {
            var by = page.ByDataAttribute(val: selectors.TitleTestId);
            page.IsElementPresent(by);
            Assert.IsTrue(driver.FindElement(by).Text == header);
        }

        internal void EnterValidMeter(string validMeter)
        {
            page.SendElementKeys(page.ByDataAttribute(val: selectors.InputValueATestId), validMeter);

			var el = new WebDriverWait(driver, TimeSpan.FromSeconds(15))
				.Until(
					SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
						page.ByDataAttribute(val: selectors.InputValueATestId)));

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
            page.ClickElement(page.ByDataAttribute(val: selectors.LinkHowDoIRead));
        }

        internal void AssertHowDoIReadMyMeterModalDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.ModalHowDoIReadGas));
            page.IsElementPresent(page.ByDataAttribute(val: selectors.ModalHowDoIReadDayNight));
            page.IsElementPresent(page.ByDataAttribute(val: selectors.ModalHowDoIRead24H));
        }

        internal void AssertMeterReadingInputFeildPopulatedAs(string p0)
        {
            page.IsElementPopulated(page.ByDataAttribute(val: selectors.InputValueATestId), p0);
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
            page.ClickElement(page.ByDataAttribute(val: selectors.ButtonSubmitMeterReadingTestId));
        }

        internal void AssertErrorMeterReadingError()
        {
            throw new NotImplementedException();
        }

        internal void AssertSubmitMeterReadingBtn()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.ButtonSubmitMeterReadingTestId));
        }

        internal void AssertMeterReadingHeader()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.TitleTestId));
        }

        internal void AssertAcceptedPageDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute("page", "meter-reading-success"));
        }

        internal void AssertAcceptedScreenHeaderDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.TitleSuccessTestId));
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
            page.IsElementPresent(page.ByDataAttribute(val: selectors.TableHistoryTestId));
            page.IsElementPresent(page.ByDataAttribute(val: selectors.TableHistoryMobileTestId));
        }

        internal void AssertRejectPageDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute("page", "meter-reading-error"));
        }

        internal void AssertRejectedScreenHeaderDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.TitleErrorTestId));
        }

        internal void AssertRejectReasonDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.MessageErrorTestId));
        }
    }
}
