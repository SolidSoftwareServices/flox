using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
    public class SubmitMeterReadingSteps : BaseStep
    {
        const string validMeter = "1234";
        private SubmitMeterReadingPage Page => new SubmitMeterReadingPage(shared.Driver.Value);

        public SubmitMeterReadingSteps(SingleTestContext shared) : base(shared)
        {
        }

        public void WhenClickSubmitMeterReadingButton()
        {
            Page.ClickSubmitMeterReadingBtn();
        }


        public void WhenEnterIntoTheMeterReadingInputField(string input)
        {
            Page.EnterMeterReadingInput(input);
        }

        public void CheckValueForMeterReadingInputField(string input)
        {
            Page.EnterMeterReadingInput(input);
        }

 
        public void ThenAMeterReadingResultConfirmationMessageShouldBeDisplayed()
        {
            try
            {
                Page.AssertRejectPageDisplayed();
                Page.AssertRejectReasonDisplayed();
                Page.AssertRejectedScreenHeaderDisplayed();
                Page.AssertBackToAccountOverviewBtnDisplayed();
            }
            catch
            {
                Page.AssertAcceptedPageDisplayed();
                Page.AssertAcceptedScreenHeaderDisplayed();
                Page.AssertBackToAccountOverviewBtnDisplayed();
            }
        }

	}
}
