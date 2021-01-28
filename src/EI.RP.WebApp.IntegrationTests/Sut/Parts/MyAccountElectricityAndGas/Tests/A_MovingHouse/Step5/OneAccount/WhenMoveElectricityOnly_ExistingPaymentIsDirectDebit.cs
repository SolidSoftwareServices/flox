using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5.OneAccount
{
	[TestFixture]
	class WhenMoveElectricityOnly_ExistingPaymentIsDirectDebit : WhenMovingOneAccount
	{
		protected override ClientAccountType ScenarioAccountType { get; } = ClientAccountType.Electricity;
		protected override PaymentMethodType ScenarioPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered { get; } = true;

		[Ignore("NotApplicable")]
		[Test]
		public override async Task Ste5ReviewPage_ClickOnEdit_Payments_Then_Choose_Manual()
		{
		}

	}
}