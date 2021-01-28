
using NUnit.Framework;
using System.Threading.Tasks;
using EI.RP.WebApp.AcceptanceTests;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using types = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.AccountTypes.types;

namespace EI.RP.WebApp.UITestAutomation
{
	class MeterReading : ResidentialPortalBrowserFixture
	{
		AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests t => new AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests();
		MeterReadingSteps mr => new MeterReadingSteps(Context);

		[Test]
		[Category("regression")]
		public void ElecMeterReadingPageText()
		{
            var account = EnvironmentSet.Inputs.MeterReadingElec;
            Context.NavigateToTestStart(account);
			Context.WhenClickMeterReadingNavButton();

			mr.ThenAssertMeterReadingPageText(account["Mprn"]);
			mr.ThenAssertElecText();
			mr.ThenThereShouldBeAPanelContainingTextWithinTheScreen();
        }

		[Test]
		[Category("regression")]
		public void GasMeterReadingPageText() {
            var account = EnvironmentSet.Inputs.MeterReadingGas;

            Context.NavigateToTestStart(account);
			Context.WhenClickMeterReadingNavButton();

			mr.ThenAssertMeterReadingPageText(account["Mprn"]);
			mr.ThenAssertGasText();
			mr.ThenThereShouldBeAPanelContainingTextWithinTheScreen();
        }

		[Test]
		[Category("regression")]
		public void DayNightMeterReadingPageText()
		{
            var account = EnvironmentSet.Inputs.MeterReadingDayNight;
            Context.NavigateToTestStart(account);
			Context.WhenClickMeterReadingNavButton();

			mr.ThenAssertMeterReadingPageText(account["Mprn"]);
			mr.ThenAssertDayNightInputText();
			mr.ThenThereShouldBeAPanelContainingTextWithinTheScreen();
        }

		[Test]
		[Category("regression")]
		public void ElecMeterReadResultPageText()
		{
			Context.NavigateToTestStart(EnvironmentSet.Inputs.MeterReadingElec);
			Context.WhenClickMeterReadingNavButton();

			mr.WhenEnterIntoTheMeterReadingInputField("1234",types.Electricity);
			mr.WhenClickSubmitMeterReadingButton();
			mr.ThenAMeterReadingResultConfirmationMessageShouldBeDisplayed(types.Electricity);
		}

		[Test]
		[Category("regression")]
		public void DayNightMeterReadResultPageText()
		{
			Context.NavigateToTestStart(EnvironmentSet.Inputs.MeterReadingDayNight);
			Context.WhenClickMeterReadingNavButton();

			mr.WhenEnterIntoTheMeterReadingInputField("1234", types.DayNight);
			mr.WhenClickSubmitMeterReadingButton();
			mr.ThenAMeterReadingResultConfirmationMessageShouldBeDisplayed(types.DayNight);
		}
        //34248
        [Test]
        [Category("regression")]
        public void DayNightMeterReadResultPageTextNoNetworkRead()
        {
            Context.NavigateToTestStart(EnvironmentSet.Inputs.MeterReadingDayNight);
            Context.WhenClickMeterReadingNavButton();

            mr.WhenEnterIntoTheMeterReadingInputField("1234", types.DayNight);
            mr.WhenClickSubmitMeterReadingButton();
            mr.ThenAMeterReadingNoNetworkReadResultConfirmationMessageShouldBeDisplayed(types.DayNight);
        }

        [Test]
		[Category("regression")]
		public void ElecMeterReadingPageModal()
		{
			Context.NavigateToTestStart(EnvironmentSet.Inputs.MeterReadingElec);
			Context.WhenClickMeterReadingNavButton();
			mr.ThenAssertHowDoIReadMyMeterModalText(types.Electricity);
            mr.Refresh(TestSettings.Default.PublicTargetUrl);
        }

		[Test]
		[Category("regression")]
		public void GasMeterReadingPageModal()
		{
			Context.NavigateToTestStart(EnvironmentSet.Inputs.MeterReadingGas);
			Context.WhenClickMeterReadingNavButton();
			mr.ThenAssertHowDoIReadMyMeterModalText(types.Gas);
            mr.Refresh(TestSettings.Default.PublicTargetUrl);
        }

		[Test]
		[Category("regression")]
		public void DayNightMeterReadingPageModal()
		{
			Context.NavigateToTestStart(EnvironmentSet.Inputs.MeterReadingDayNight);
			Context.WhenClickMeterReadingNavButton();
			mr.ThenAssertHowDoIReadMyMeterModalText(types.DayNight);
            mr.Refresh(TestSettings.Default.PublicTargetUrl);
        }

		[Test]
		[Category("regression")]
		public void GasMeterReadResultPage()
		{
			Context.NavigateToTestStart(EnvironmentSet.Inputs.MeterReadingGas);
			Context.WhenClickMeterReadingNavButton();

			mr.WhenEnterIntoTheMeterReadingInputField("1234", types.Gas);
			mr.WhenClickSubmitMeterReadingButton();
			mr.ThenAMeterReadingResultConfirmationMessageShouldBeDisplayed(types.Gas);
		}

		[Test]
		[Category("regression")]
		public void ElecMeterReadingEmptyField()
		{
			Context.NavigateToTestStart(EnvironmentSet.Inputs.MeterReadingElec);
			Context.WhenClickMeterReadingNavButton();

			mr.WhenEnterIntoTheMeterReadingInputField("", types.Electricity);
			mr.WhenClickSubmitMeterReadingButton();
			mr.ThenErrorEmptyField(types.Electricity);
		}


		[Test]
		[Category("regression")]
		public void ElecMeterReadingErrorAlphabetical()
		{
			Context.NavigateToTestStart(EnvironmentSet.Inputs.MeterReadingElec);
			Context.WhenClickMeterReadingNavButton();

			mr.WhenEnterIntoTheMeterReadingInputField("123a", types.Electricity);
			mr.WhenClickSubmitMeterReadingButton();
			mr.ThenErrorAlphabetical(types.Electricity);
		}


		[Test]
		[Category("regression")]
		public void DayNightMeterReadingEmptyField()
		{
			Context.NavigateToTestStart(EnvironmentSet.Inputs.MeterReadingDayNight);
			Context.WhenClickMeterReadingNavButton();

			mr.WhenEnterIntoTheMeterReadingInputField("", types.DayNight);
			mr.WhenClickSubmitMeterReadingButton();
			mr.ThenErrorEmptyField(types.DayNight);
		}


	}
}
