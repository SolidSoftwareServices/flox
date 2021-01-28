using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step2.NegativeScenarios
{
    [TestFixture]
    abstract class WhenInMovingHomeNegativeScenarioPageTest<TPage> : MyAccountCommonTests<TPage> where TPage : ShowMovingHouseValidationErrorPage
    {
	    [Test]
        public abstract Task CanSeeComponents();
    }
}