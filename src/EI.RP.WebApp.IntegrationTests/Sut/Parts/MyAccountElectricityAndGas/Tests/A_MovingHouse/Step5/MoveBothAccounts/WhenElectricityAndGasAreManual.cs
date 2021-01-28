using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5.MoveBothAccounts
{
	class WhenElectricityAndGasAreManual : WhenMovingBothAccounts
	{
		protected override PaymentMethodType ScenarioElectricityPaymentMethodType { get; } = PaymentMethodType.Manual;
		protected override PaymentMethodType ScenarioGasPaymentMethodType { get; } = PaymentMethodType.Manual;

		protected override bool IsPRNDegistered { get; } = false;

		[Test]
		public async Task HandlesUserPath_Cancel_ThenGoBack_ThenCancel_ThenCancel_ShowsLandingPage()
		{
			await Sut.ClickOnCancelConfirm();
			App.CurrentPageAs<Step0LandingPage>();
		}

	}
}