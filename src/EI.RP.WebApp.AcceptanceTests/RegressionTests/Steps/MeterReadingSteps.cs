using System;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using EI.RP.WebApp.RegressionTests.PageObjects;
using types = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.AccountTypes.types;
namespace EI.RP.WebApp.UITestAutomation
{
	public class MeterReadingSteps : BaseStep
	{
		string validMeter = "1234";

		public MeterReadingSteps(SingleTestContext shared) : base(shared)
		{}

		SubmitMeterReadingPage submitMeterReadingPage => new SubmitMeterReadingPage(shared.Driver.Value);


        public void WhenEnterIntoTheMeterReadingInputField(string input, Enum type)
        {
            submitMeterReadingPage.EnterMeterReadingInput(input, type);
        }

        public void WhenClickSubmitMeterReadingButton()
		{
			submitMeterReadingPage.ClickSubmitMeterReadingBtn();
		}

		internal void ThenAssertElecText()
		{
			submitMeterReadingPage.AssertElecText();
		}

		internal void ThenAssertGasText()
		{
			submitMeterReadingPage.AssertGasText();
		}

		public void ThenAssertMeterReadingPageText(string mprn)
		{
			submitMeterReadingPage.AssertMeterReadingHeader();
			submitMeterReadingPage.AssertMPRNAndMeterInfo(mprn);
		}


		public void ThenThereShouldBeAPanelContainingTextWithinTheScreen()
		{
			submitMeterReadingPage.AssertSubmitYourMeterReadingText();
		}

		public void ThenAssertHowDoIReadMyMeterModalText(Enum type)
		{
			submitMeterReadingPage.AssertModalDisplayed(type);
		}
        internal void ThenAssertDayNightInputText()
        {
            submitMeterReadingPage.AssertDayNightInputFields();
        }
        internal void Refresh(string Url)
        {
            shared.Driver.Value.Instance.Navigate().GoToUrl(Url);
        }

        public void ThenAMeterReadingResultConfirmationMessageShouldBeDisplayed(Enum type)
		{
			switch (type)
			{
				case types.DayNight:
					try
					{
						submitMeterReadingPage.AssertDayNightRejectReasonDisplayed();
					}
					catch
					{
						submitMeterReadingPage.AssertDayNightAcceptReasonDisplayed();

					}
					break;
				case types.Electricity:
					try
					{
						submitMeterReadingPage.AssertElecRejectReasonDisplayed();
					}
					catch
					{
						submitMeterReadingPage.AssertElecAcceptReasonDisplayed();

					}
					break;
				case types.Gas:
						submitMeterReadingPage.AssertGasAcceptReasonDisplayed();

					break;
			}			
		}
        public void ThenAMeterReadingNoNetworkReadResultConfirmationMessageShouldBeDisplayed(Enum type)
        {
            switch (type)
            {
                case types.DayNight:
                    try
                    {
                        submitMeterReadingPage.AssertDayNightNoNetworkRejectReasonDisplayed();
                    }
                    catch
                    {
                        submitMeterReadingPage.AssertDayNightAcceptReasonDisplayed();

                    }
                    break;
                case types.Electricity:
                    try
                    {
                        submitMeterReadingPage.AssertElecRejectReasonDisplayed();
                    }
                    catch
                    {
                        submitMeterReadingPage.AssertElecAcceptReasonDisplayed();

                    }
                    break;
                case types.Gas:
                    submitMeterReadingPage.AssertGasAcceptReasonDisplayed();

                    break;
            }
        }


        internal void ThenErrorEmptyField(Enum type)
		{
			submitMeterReadingPage.AssertError(type);
		}

		internal void ThenErrorAlphabetical(Enum type)
		{
			submitMeterReadingPage.AssertError(type);
		}

		public void WhenClickSubmitMeterReadingButtonOnAcccountOverviewPage()
		{
		}
	}
}
