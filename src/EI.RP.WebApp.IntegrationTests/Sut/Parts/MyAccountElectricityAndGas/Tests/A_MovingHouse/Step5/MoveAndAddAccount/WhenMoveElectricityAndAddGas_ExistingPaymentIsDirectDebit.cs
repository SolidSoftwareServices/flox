using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5.MoveAndAddAccount
{
	abstract class WhenMoveElectricityAndAddGas_ExistingPaymentIsDirectDebit : WhenMoveOneAccountAndAddOtherAccount
	{
		protected override ClientAccountType ScenarioAccountType { get; } = ClientAccountType.Electricity;
		protected override PaymentMethodType ScenarioPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered { get; } = true;

	}
}
