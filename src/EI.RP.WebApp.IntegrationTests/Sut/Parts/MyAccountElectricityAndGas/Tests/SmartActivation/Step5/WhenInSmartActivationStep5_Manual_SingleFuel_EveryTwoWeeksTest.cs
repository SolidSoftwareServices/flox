using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step5
{
	[TestFixture]
	class WhenInSmartActivationStep5_Manual_SingleFuel_EveryTwoWeeksTest : WhenInSmartActivationStep5Test
	{
		protected override PaymentMethodType PaymentMethod => PaymentMethodType.Manual;
		protected override bool IsDualFuel => false;
		protected override BillingFrequencyType BillingFrequency => BillingFrequencyType.EveryTwoMonths;
	}
}