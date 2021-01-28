using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5.MoveElectricityAndCloseGas
{
    class WhenElectricityAndGasAreDirectDebit: WhenMovingElectricityAndClosingGas
    {
        protected override PaymentMethodType ScenarioElectricityPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

        protected override PaymentMethodType ScenarioGasPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

        protected override bool IsPRNDegistered { get; } = false;

        [Ignore("NotApplicable")]
        [Test]
        public override async Task Ste5ReviewPage_ClickOnEdit_Payments_Then_Choose_Manual()
        {
        }
	}
}
