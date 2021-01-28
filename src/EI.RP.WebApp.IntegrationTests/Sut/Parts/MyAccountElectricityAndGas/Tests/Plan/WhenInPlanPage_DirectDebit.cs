using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.Plan
{
    internal class WhenInPlanPage_DirectDebit : WhenInPlanPageTests
    {
	    protected override PaymentMethodType PaymentType => PaymentMethodType.DirectDebit;
    }
}
