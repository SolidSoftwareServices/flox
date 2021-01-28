using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5.OneAccount
{
	[TestFixture]
	class WhenMoveGasOnly_ExistingPaymentIsDirectDebit : WhenMoveElectricityOnly_ExistingPaymentIsDirectDebit
	{
        protected override ClientAccountType ScenarioAccountType { get; } = ClientAccountType.Gas;
        protected override PaymentMethodType ScenarioPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

        protected override bool IsPRNDegistered { get; } = false;
	}
}