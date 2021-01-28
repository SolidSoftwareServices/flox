using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5Confirmation.OneAccount
{
	[TestFixture]
	class WhenMoveGasOnlyAndManual : WhenConfirmationScreen
	{
		protected override PaymentMethodType ScenarioPaymentMethodType => PaymentMethodType.Manual;

		protected override bool IsPRNDegistered => false;

		protected override bool HasFreeElectricityAllowance => false;

		protected override MovingHouseType MovingHouseType => MovingHouseType.MoveGas;

		[Test]
		public void Step5ConfirmationPage_Test_CanSeeComponents()
		{
			CanSeeComponents();
		}

		private void CanSeeComponents()
		{
			Assert.IsNull(
				Sut.ConfirmationElectricityAccountNumber,
				"Did not expect to see Electricity Account Number");

			Assert.AreEqual(
				"Thank you, your account has been moved to your new address.",
				Sut.ThankYouAccountMoveNotice.TextContent.Trim());

			Assert.AreEqual(
				"Thank you, your account has been moved to your new address.",
				Sut.ThankYouAccountMoveNotice.TextContent.Trim());

			Assert.IsNull(
				Sut.FreeElectricityAllowanceNotice,
				"Should not see Free Electricity Allowance Notice for this scenario");

			Assert.AreEqual(
				"Your Gas bill will be paid manually.",
				Sut.YourGasAccountWillBePaidByNotice.TextContent.Trim());

			Assert.IsNotNull(
				Sut.BackToMyAccountsLink,
				"expected Back to my accounts link");
		}
	}
}