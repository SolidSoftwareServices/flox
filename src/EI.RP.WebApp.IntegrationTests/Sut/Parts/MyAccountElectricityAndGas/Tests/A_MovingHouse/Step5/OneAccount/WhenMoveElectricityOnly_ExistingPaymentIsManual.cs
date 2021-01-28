using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5.OneAccount
{
	class WhenMoveElectricityOnly_ExistingPaymentIsManual : WhenMovingOneAccount
	{

		protected override ClientAccountType ScenarioAccountType { get; } = ClientAccountType.Electricity;
		protected override PaymentMethodType ScenarioPaymentMethodType { get; }=PaymentMethodType.Manual;

		protected override bool IsPRNDegistered { get; } = false;
	}
}