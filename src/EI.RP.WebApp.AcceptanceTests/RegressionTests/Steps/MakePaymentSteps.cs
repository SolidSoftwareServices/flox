using OpenQA.Selenium;
using System;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using EI.RP.WebApp.RegressionTests.PageObjects;
namespace EI.RP.WebApp.UITestAutomation
{
    public class MakePaymentSteps : BaseStep
    {

        public MakePaymentSteps(SingleTestContext shared) : base(shared)
        { }
    
		private MakeAPaymentPage makeAPaymentPage => new MakeAPaymentPage(shared.Driver.Value);

        public void WhenClickSubmitOnPayDifferentAmount()
        {
            makeAPaymentPage.ClickSubmitBtnPayDiffAmount();
        }
        
        public void WhenClickPayADifferentAmount()
        {
            makeAPaymentPage.ClickPayDifferentAmount();
        }
        
        public void WhenEnterHowMuchWouldYouLikeToPayNowAs(string p0)
        {
            makeAPaymentPage.EnterNewPayAmount(p0);
        }
        
        internal void ThenPayAmountShouldBeSet(string payAmount)
        {
            makeAPaymentPage.AssertPaymentAmountChanged(payAmount);
        }

        internal void ThenErrorAlphabetical()
        {
            makeAPaymentPage.AssertErrorAlphabetical();
        }

        internal void ThenErrorMinus()
        {
            makeAPaymentPage.AssertErrorMinus();
        }

        internal void ThenErrorHigh()
        {
            makeAPaymentPage.AssertErrorHigh();
        }

        internal void WhenAmountDueIsTooLow()
        {
            makeAPaymentPage.AssertTooLowNotifier();
        }

        internal void ThenElevonPanelShouldAppear()
        {
            makeAPaymentPage.AsserElevonPanel();
        }
    }
}
