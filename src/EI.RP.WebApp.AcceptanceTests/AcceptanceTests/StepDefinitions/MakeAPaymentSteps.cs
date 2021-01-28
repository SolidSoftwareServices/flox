using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
    public class MakeAPaymentSteps: BaseStep
    {
		public MakeAPaymentSteps(SingleTestContext shared) : base(shared)
        {
        }
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
    }
}
