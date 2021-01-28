using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Confirmation
{
	[TestFixture]
	internal class WhenInDualFuelTest : WhenInSmartActivationConfirmationTest
	{ 
	    protected override bool IsDualFuel => true;

		protected override PaymentMethodType PaymentMethod => PaymentMethodType.Manual;
	}
}