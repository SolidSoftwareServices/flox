using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5Confirmation.OneAccount
{
	[TestFixture]
	class WhenMoveGasOnlyAndDirectDebit : WhenConfirmationScreen
	{
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

			Assert.IsTrue(Sut.DirectDebitChangeConfirmation?.TextContent.Trim().Equals("You have changed your payment method to Direct Debit, your savings will be applied within 12 days."));

			Assert.IsTrue(Sut.ReadTermsAndConditions?.Href.Equals("https://www.electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity-and-gas-pricing"));
			Assert.IsTrue(Sut.ReadTermsAndConditions?.TextContent.Trim().Equals("Read T&Cs."));

			Assert.IsTrue(Sut.DirectDebitBillMessage?.TextContent.Trim().Equals("Your first Direct Debit payment will be made 14 days after your next bill is issued. To update your Direct Debit details at any time go to"));
			Assert.IsTrue(Sut.BillAndPaymentLink?.TextContent.Trim().Equals("Payments"));
		}

		[Test]
		public async Task CanNavigateToPaymentOptionsPage()
		{
			(await App.ClickOnElement(Sut.BillAndPaymentLink)).CurrentPageAs<ShowPaymentsHistoryPage>();
		}
	}
}