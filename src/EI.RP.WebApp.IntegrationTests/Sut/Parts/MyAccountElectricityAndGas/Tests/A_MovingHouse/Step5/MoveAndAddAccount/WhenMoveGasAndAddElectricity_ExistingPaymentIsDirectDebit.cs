using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5.MoveAndAddAccount
{
	class WhenMoveGasAndAddElectricity_ExistingPaymentIsDirectDebit : WhenMoveElectricityAndAddGas_ExistingPaymentIsDirectDebit
	{
		protected override ClientAccountType ScenarioAccountType { get; } = ClientAccountType.Gas;
		protected override PaymentMethodType ScenarioPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered { get; } = true;
	}
}