using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5Confirmation.OneAccount
{
	[TestFixture]
	class WhenMoveElectricityOnlyAndManualPaymentAndHasFreeAllowance : WhenConfirmationScreen
	{
		protected override PaymentMethodType ScenarioPaymentMethodType => PaymentMethodType.Manual;

		protected override bool IsPRNDegistered => false;

		protected override bool HasFreeElectricityAllowance => true;

		protected override MovingHouseType MovingHouseType => MovingHouseType.MoveElectricity;

		[Test]
		public void Step5ConfirmationPage_Test_CanSeeComponents()
		{
			CanSeeComponents();
		}

		private void CanSeeComponents()
		{
			Assert.IsNotNull(Sut.ConfirmationElectricityAccountNumber.TextContent);

			Assert.AreEqual(
				"Thank you, your account has been moved to your new address.",
				Sut.ThankYouAccountMoveNotice.TextContent.Trim());

			Assert.IsTrue(Sut.FreeElectricityAllowanceNotice.TextContent.Contains("Please note you will need to contact the Department of Social Protection to set up your Free Electricity Allowance at your new address."));

			Assert.AreEqual(
				"Your Electricity bill will be paid manually.",
				Sut.YourElectricityAccountWillBePaidByNotice.TextContent.Trim());

			Assert.IsNotNull(
				Sut.BackToMyAccountsLink,
				"expected Back to my accounts link");
		}
	}
}