using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Confirmation
{
	[TestFixture]
	internal class WhenInElectricityOnlyTest : WhenInSmartActivationConfirmationTest
	{ 
	    protected override bool IsDualFuel => false;

		protected override PaymentMethodType PaymentMethod => PaymentMethodType.Manual;
	}
}