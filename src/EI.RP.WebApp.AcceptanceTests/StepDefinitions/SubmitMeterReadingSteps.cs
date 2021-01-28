using System;
using TechTalk.SpecFlow;
using EI.RP.WebApp.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Utils;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
using System.Threading;

namespace EI.RP.WebApp.AcceptanceTests.StepDefinitions
{
    [Binding]
    public class SubmitMeterReadingSteps : BaseStep
    {
        const string validMeter = "1234";
        private SubmitMeterReadingPage Page => new SubmitMeterReadingPage(shared.driver);
        private AccountOverviewPage AccountOverviewPage => new AccountOverviewPage(driver);

        public SubmitMeterReadingSteps(SharedThings shared) : base(shared)
        {
        }

        public void GivenNavigateToTheMeterReadingConfirmationScreen()
        {
            Page.EnterValidMeter(validMeter);
            Page.ClickSubmitMeterReadingBtn();
        }

        public void WhenClickSubmitMeterReadingButton()
        {
            Page.ClickSubmitMeterReadingBtn();
        }

        public void WhenClickHowDoIReadMyMeterButton()
        {
            Page.ClickHowDoIReadMyMeterBtn();
        }

        public void WhenEnterIntoTheMeterReadingInputField(string input)
        {
            Page.EnterMeterReadingInput(input);
        }

        public void CheckValueForMeterReadingInputField(string input)
        {
            Page.EnterMeterReadingInput(input);
        }

        public void WhenClickOnTheBackToAccountOverviewButton()
        {
            Page.ClickBackToAccountOverviewBtn();
        }

        public void ThenShouldBeSentToTheMeterReadingScreen()
        {
            Page.AssertMeterReadingPage();
        }

        public void ThenTheHeaderOfTheScreenDisplayedShouldBeMeterReading()
        {
            Page.AssertMeterReadingHeader();
        }

        public void ThenThereShouldBeAButtonSayingSubmitMeterReading()
        {
            Page.AssertSubmitMeterReadingBtn();
        }

        public void ThenErrorAsEmptyMeterReadingError()
        {
            Page.AssertErrorMeterReadingError();
        }

        public void ThenErrorAsAlphabeticalCharactersError()
        {
            Page.AssertErrorAlphabeticalCharacters();
        }

        public void ThenErrorAsSpecialCharactersError()
        {
            Page.AssertErrorSpecialCharacters();
        }

        public void ThenInputShouldBeTruncatedAfterTheThDigit(string p0)
        {
            Page.AssertMeterReadingInputFeildPopulatedAs(p0);
        }

        public void ThenTheTextOfTheConfirmationScreenShouldSay(string header)
        {
            Page.AssertConfirmationScreenHeader(header);
        }

        public void ThenThereShouldBeAButtonSayingBackToAccountOverviewDisplayed()
        {
            Page.AssertBackToAccountOverviewBtnDisplayed();
        }

        public void ThenAMeterReadingResultConfirmationMessageShouldBeDisplayed()
        {
	        try
	        {
		        Page.AssertAcceptedPageDisplayed();
		        Page.AssertAcceptedScreenHeaderDisplayed();
	        }
	        catch
	        {
		        Page.AssertRejectPageDisplayed();
		        Page.AssertRejectReasonDisplayed();
		        Page.AssertRejectedScreenHeaderDisplayed();
		        
	        }
	        finally
	        {
		        Page.AssertBackToAccountOverviewBtnDisplayed();
			}
        }

        public void WhenClickMeterBackToAccountOverviewButton()
        {
            Page.ClickBackToAccountOverviewBtn();
        }

        public void ThenTheHeaderOfTheRejectionMessageShouldRead(string p0)
        {
            Page.AssertRejectedScreenHeaderDisplayed();
        }

        public void WhenClickSubmitMeterReadingButtonOnAcccountOverviewPage()
        {
            AccountOverviewPage.ClickSubmitMeterReadingBtn();
        }
    }
}
