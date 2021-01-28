using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step4
{
	[TestFixture]
    class WhenInElectricityOnlyTest : WhenInSmartActivationStep4Test
    {
		protected override PaymentMethodType PaymentMethod => PaymentMethodType.Manual;
    }
}