using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5Confirmation.OneAccount
{
	[TestFixture]
	class WhenMoveGasOnlyAndDirectDebitToDirectDebit : WhenConfirmationScreen
	{
		protected override PaymentMethodType ExistingPaymentMethodType => PaymentMethodType.DirectDebit;
		protected override PaymentMethodType ScenarioPaymentMethodType => PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered => false;

		protected override bool HasFreeElectricityAllowance => false;

		protected override MovingHouseType MovingHouseType => MovingHouseType.MoveGas;

		[Test]
		public async Task Step5ConfirmationPage_Test_CanSeeComponents()
		{
			CanSeeComponents();
		}

		private void CanSeeComponents()
		{
			Assert.IsNotNull(
				Sut.ConfirmationGasAccountNumber.TextContent, 
				"Expected to see Gas Account Number");

			Assert.IsNull(
				Sut.ConfirmationElectricityAccountNumber,
				"Did not expect to see Electricity Account Number");

			Assert.AreEqual(
				"Thank you, your account has been moved to your new address.",
				Sut.ThankYouAccountMoveNotice.TextContent.Trim());

			Assert.IsNull(
				Sut.FreeElectricityAllowanceNotice,
				"Should not see Free Electricity Allowance Notice for this scenario");

			Assert.AreEqual(
				"Your Gas bill will be paid by direct debit.",
				Sut.YourGasAccountWillBePaidByNotice.TextContent.Trim());

			Assert.IsNotNull(
				Sut.BackToMyAccountsLink,
				"expected Back to my accounts link");

			Assert.IsNull(Sut.DirectDebitChangeConfirmation);

			Assert.IsNull(Sut.ReadTermsAndConditions);

			Assert.IsNull(Sut.DirectDebitBillMessage);
			Assert.IsNull(Sut.BillAndPaymentLink);
		}
	}
}