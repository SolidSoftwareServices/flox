using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
	public class EnergyServicesSteps : BaseStep
    {
        public EnergyServicesSteps(SingleTestContext shared) : base(shared) { }
		private EnergyServicesAccountPage energyServicesAccountPage => new EnergyServicesAccountPage(shared.Driver.Value);

		public void ThenEnergyServicesAccountDetailsShouldBeDisplayed()
        {
            energyServicesAccountPage.AssertLastPaymentAmountDisplayed();
            energyServicesAccountPage.AssertPaymentDateDisplayed();
        }
        
        public void ThenContactUsShouldBeDisplayed()
        {
            energyServicesAccountPage.AssertContactUsDisplayed();
        }
    }
}
