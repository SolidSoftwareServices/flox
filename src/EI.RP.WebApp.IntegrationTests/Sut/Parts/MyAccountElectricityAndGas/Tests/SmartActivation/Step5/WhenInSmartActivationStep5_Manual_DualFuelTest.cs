using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step5
{
	[TestFixture]
	class WhenInSmartActivationStep5_Manual_DualFuelTest : WhenInSmartActivationStep5Test
	{
		protected override PaymentMethodType PaymentMethod => PaymentMethodType.Manual;
		protected override bool IsDualFuel => true;
		protected override BillingFrequencyType BillingFrequency => BillingFrequencyType.EveryMonth;
		protected override int BillingDayOfMonth => 12;
	}
}