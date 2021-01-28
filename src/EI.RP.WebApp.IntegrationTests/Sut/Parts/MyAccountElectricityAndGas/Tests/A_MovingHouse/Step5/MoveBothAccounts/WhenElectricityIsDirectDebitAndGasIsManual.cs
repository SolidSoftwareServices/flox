using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5.MoveBothAccounts
{
	internal class WhenElectricityIsDirectDebitAndGasIsManual : WhenMovingBothAccounts
	{
		protected override PaymentMethodType ScenarioElectricityPaymentMethodType { get; } = PaymentMethodType.DirectDebit;
		protected override PaymentMethodType ScenarioGasPaymentMethodType { get; } = PaymentMethodType.Manual;

		protected override bool IsPRNDegistered { get; } = true;
	}
}