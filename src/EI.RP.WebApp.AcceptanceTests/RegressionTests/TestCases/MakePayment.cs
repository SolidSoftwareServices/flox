
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using types = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.AccountTypes.types;

namespace EI.RP.WebApp.UITestAutomation
{
	class MakeAPayment : ResidentialPortalBrowserFixture
	{
		AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests t => new AcceptanceTests.AcceptanceTests.TestCases.AcceptanceTests();
		MakePaymentSteps map => new MakePaymentSteps(Context);
        BillsAndPaymentsSteps bp => new BillsAndPaymentsSteps(Context);
        [Test]
        [Category("regression")]
        public void VerifyElecPaymentNewAmount()
        {
            string payAmount = "1.50";
            var account = EnvironmentSet.Inputs.MakeAPayment;

            Context.NavigateToTestStart(account);
            Context.WhenClickBillsPaymentsNavButton();
            bp.WhenClickPayNow();
            map.WhenClickPayADifferentAmount();
            map.WhenEnterHowMuchWouldYouLikeToPayNowAs(payAmount);
            map.WhenClickSubmitOnPayDifferentAmount();
            map.ThenPayAmountShouldBeSet(payAmount);
        }

        [Test]
        [Category("regression")]
        public void VerifyElecPaymentErrorAlphabetical()
        {
            string payAmount = "1a";
            var account = EnvironmentSet.Inputs.MakeAPayment;

            Context.NavigateToTestStart(account);
            Context.WhenClickBillsPaymentsNavButton();
            bp.WhenClickPayNow();
            map.WhenClickPayADifferentAmount();
            map.WhenEnterHowMuchWouldYouLikeToPayNowAs(payAmount);
            map.WhenClickSubmitOnPayDifferentAmount();
            map.ThenErrorAlphabetical();
        }
        [Test]
        [Category("regression")]
        public void VerifyElecPaymentErrorMinus()
        {
            string payAmount = "-1";
            var account = EnvironmentSet.Inputs.MakeAPayment;

            Context.NavigateToTestStart(account);
            Context.WhenClickBillsPaymentsNavButton();
            bp.WhenClickPayNow();
            map.WhenClickPayADifferentAmount();
            map.WhenEnterHowMuchWouldYouLikeToPayNowAs(payAmount);
            map.WhenClickSubmitOnPayDifferentAmount();
            map.ThenErrorMinus();
        }
        [Test]
        [Category("regression")]
        public void VerifyElecPaymentErrorLimitHigh()
        {
            string payAmount = "10000";
            var account = EnvironmentSet.Inputs.MakeAPayment;

            Context.NavigateToTestStart(account);
            Context.WhenClickBillsPaymentsNavButton();
            bp.WhenClickPayNow();
            map.WhenClickPayADifferentAmount();
            map.WhenEnterHowMuchWouldYouLikeToPayNowAs(payAmount);
            map.WhenClickSubmitOnPayDifferentAmount();
            map.ThenErrorHigh();
        }
        [Test]
        [Category("regression")]
        public void VerifyNegativeToPositiveValue()
        {
            string payAmount = "1";
            var account = EnvironmentSet.Inputs.MakeAPayment;

            Context.NavigateToTestStart(account);
            Context.WhenClickBillsPaymentsNavButton();
            bp.WhenClickPayNow();
            map.WhenAmountDueIsTooLow();
            map.WhenClickPayADifferentAmount();
            map.WhenEnterHowMuchWouldYouLikeToPayNowAs(payAmount);
            map.WhenClickSubmitOnPayDifferentAmount();
            map.ThenElevonPanelShouldAppear();

        }

        //TODO
        //Bill Issue Date
        //Payment due date
        //Next Bill date

       
	}
}
