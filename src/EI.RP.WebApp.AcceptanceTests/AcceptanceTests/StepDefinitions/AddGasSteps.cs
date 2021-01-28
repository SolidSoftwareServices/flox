using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
	public class AddGasSteps: BaseStep
    {
        public AddGasSteps(SingleTestContext shared) : base(shared)
        {
        }
		private PlanPage planPage => new PlanPage(shared.Driver.Value);
		private GasAccountSetupPage gasAccountSetupPage => new GasAccountSetupPage(shared.Driver.Value);

		public void WhenClickAddGasButtonOnCTA()
        {
	        planPage.ClickAddGasCTA();
            var gasAccountSetupPage = new GasAccountSetupPage(shared.Driver.Value);
            gasAccountSetupPage.AssertGasAccountSetupPage();
        }
        
        public void WhenFillInGassAccountSetup(IDictionary dict)
        {
            if (dict.ContainsKey("GPRN"))
            {
                gasAccountSetupPage.EnterGPRN(dict["GPRN"]);
            }
            if (dict.ContainsKey("Meter Reading"))
            {
                gasAccountSetupPage.EnterMeterReading(dict["Meter Reading"]);
            }
            if (dict.ContainsKey("Details Check"))
            {
                gasAccountSetupPage.ClickDetailsCheckbox(dict["Details Check"]);
            }
            if (dict.ContainsKey("General Terms Check"))
            {
                gasAccountSetupPage.ClickGeneralTermsCheckbox(dict["General Terms Check"]);
            }
            if (dict.ContainsKey("Debt Flag Check"))
            {
                gasAccountSetupPage.ClickDebtFlagCheckbox(dict["Debt Flag Check"]);
            }
            if (dict.ContainsKey("Price Plan Check"))
            {
                gasAccountSetupPage.ClickPricePlanCheckbox(dict["Price Plan Check"]);
            }
        }
        
        public void WhenClickSubmitOnGasAccountSetup()
        {
            gasAccountSetupPage.ClickSubmitBtn();
        }
        
        
        public void WhenClickYesConfirmAddressPage()
        {
            GasAccountConfirmAddressPage gasAccountConfirmAddressPage = new GasAccountConfirmAddressPage(shared.Driver.Value);
            gasAccountConfirmAddressPage.AssertGasAccountConfirmAddressPage();
            gasAccountConfirmAddressPage.ClickYesBtn();
        }
        
        public void WhenChooseGasAccountPayment()
        {
            GasAccountPaymentPage gasAccountPaymentPage = new GasAccountPaymentPage(shared.Driver.Value);
            gasAccountPaymentPage.AssertGasAccountPaymentPage();
            gasAccountPaymentPage.GasAccPaymentTickUseExistingDDCheckbox();

            gasAccountPaymentPage.GasAccPaymentClickGoBackBtn();
        }
    }
}
