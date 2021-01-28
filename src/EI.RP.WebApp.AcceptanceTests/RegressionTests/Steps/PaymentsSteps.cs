using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using EI.RP.WebApp.RegressionTests.PageObjects;
using EI.RP.WebApp.UITestAutomation;
using types = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.AccountTypes.types;
namespace EI.RP.WebApp.UITestAutomation
{
    public class PaymentsSteps : BaseStep
    {
        public PaymentsSteps(SingleTestContext shared) : base(shared)
        { }
        private PaymentsPage paymentsPage => new PaymentsPage(shared.Driver.Value);
        EqualMonthlyPaymentsPage equalMonthlyPaymentsPage => new EqualMonthlyPaymentsPage(shared.Driver.Value); 
        public void WhenClickBillsAndPaymentsOptionsButton()
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));
            paymentsPage.ClickBillingAndPaymentOptionsBtn();


            PlanPage planPage = new PlanPage(shared.Driver.Value);
            planPage.AssertDirectDebitDisplayed();
            planPage.AssertPaperlessBillingDisplayed();
        }
        internal void WhenCLickMoreAboutEqualMonthlyPayments()
        {

            paymentsPage.ClickMoreAboutEqualMonthlyPayments();
        }

        internal void WhenClickSetUpEqualizer()
        {
            equalMonthlyPaymentsPage.ClickSetUpEqualizer();
        }

        internal void WhenClickSetUpDirectDebit()
        {
            equalMonthlyPaymentsPage.ClickSetUpDirectDebit();
        }
    }
}
