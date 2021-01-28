using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveElectricityAndCloseGas
{
    class WhenElectricityAndGasAreManual: WhenMovingElectricityAndClosingGas
    {
        protected override PaymentMethodType ScenarioElectricityPaymentMethodType { get; } = PaymentMethodType.Manual;

        protected override PaymentMethodType ScenarioGasPaymentMethodType { get; } = PaymentMethodType.Manual;

        protected override bool IsPRNDegistered { get; } = false;
	}
}
